using DalApi;
using Microsoft.VisualBasic;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using BlApi;
using BlImplementation;
using System.Data.Common;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Text;
using System.Net;
using System.Globalization;

namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = DalApi.Factory.Get;
    internal static int TotalCall(int id, DO.EndType endType)
    {
      var tempAssignments=  s_dal.Assignment.ReadAll(s => (s.VolunteerId == id) && (s.TheEndType == endType)); 
      var toReturn= tempAssignments==null?0: tempAssignments.Count();
        return toReturn;
    }
    internal static BO.CallInProgress callProgress(int id)
    {
        var assignments = s_dal.Assignment.ReadAll(s => (s.VolunteerId == id) && (s.TheEndType == null));
        var volunteer = s_dal.Volunteer.Read(id)??
           throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist");
        AdminImplementation admin = new();
        BO.CallInProgress callInProgress =
         (from item in assignments
          let call = s_dal.Call.Read(item.CallId)
          select new BO.CallInProgress
          {
              Id = item.Id,
              CallId = item.CallId,
              TheCallType = (BO.CallType)call.TheCallType,
              VerbalDescription = call.VerbalDescription,
              Address = call.Address,
              OpeningTime = call.OpeningTime,
              MaxTimeToEnd = call.MaxTimeToEnd,
              EntryTime = item.EntryTime,

              Distance = volunteer?.Address != null ? VolunteerManager.GetDistance(volunteer.Address, call.Address) : 0,
  
              status = (ClockManager.Now - call.MaxTimeToEnd) <= admin.GetRiskRange()
                        ? BO.Status.treatment
                        : BO.Status.riskTreatment
          }).FirstOrDefault();

           return callInProgress;
    }

    internal static int? GetCurrentCall(int id)
    {
        var assignment = s_dal.Assignment.ReadAll(t => t.ActualEndTime == null && id == t.VolunteerId).FirstOrDefault();
        return assignment?.CallId;

    }

    internal static BO.CallType GetCallType(int id)
    {
        var call = s_dal.Call.Read(id);
        return (BO.CallType)call.TheCallType;
    }

    #region isValidInput
    internal static void IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new BO.BlInvalidValueExeption("Invalid Email");
        // Regular expression to match a valid email address
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if( Regex.IsMatch(email, emailPattern)==false)
            throw new BO.BlInvalidValueExeption("Invalid Email");
    }


    internal static void IsValidPhoneNumber(string phoneNumber)
    {
        // אם הקלט ריק או מכיל רווחים
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new BO.BlInvalidValueExeption("Invalid Phone Number ");
        // בדיקה שהטלפון מתחיל ב-05, מכיל בדיוק 10 ספרות וכולל רק ספרות
        if( phoneNumber.Length == 10 && phoneNumber.StartsWith("05") && phoneNumber.All(char.IsDigit)==false)
            throw new BO.BlInvalidValueExeption("Invalid Phone Number "); ;
    }

    internal static void IsValidID(int id)
    {
        // הופכים את האינט לסטרינג כדי לבדוק את אורך תעודת הזהות
        string idStr = id.ToString();

        // תעודת זהות חייבת להיות באורך 9 תווים (ללא ספרת ביקורת)
        if (idStr.Length != 9)
            throw new BO.BlInvalidValueExeption("Invalid Id");
        // חישוב ספרת הביקורת (כפי שנעשה בתעודת זהות ישראלית)
        int sum = 0;
        for (int i = 0; i < 8; i++)
        {
            int digit = idStr[i] - '0';  // המרת תו למספר
            if (i % 2 == 0)
            {
                sum += digit;
            }
            else
            {
                int doubled = digit * 2;
                sum += doubled > 9 ? doubled - 9 : doubled;  // חיבור הספרות של התוצאה
            }
        }
        // חישוב ספרת הביקורת
        int checkDigit = (10 - (sum % 10)) % 10;
        // השוואה עם ספרת הביקורת בתעודת הזהות
        if( checkDigit == (id % 10)==false)
            throw new BO.BlInvalidValueExeption("Invalid Id");
    }
    #endregion

    #region Passwors
    public static string GenerateStrongPassword()
    {
        // מאגר התווים האפשריים
        const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string specialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

        // יוצר אובייקט StringBuilder כדי לבנות את הסיסמה
        StringBuilder password = new StringBuilder();

        // מגריל לפחות אות קטנה, אות גדולה, מספר ותו מיוחד
        password.Append(lowerCase[new Random().Next(lowerCase.Length)]);
        password.Append(upperCase[new Random().Next(upperCase.Length)]);
        password.Append(digits[new Random().Next(digits.Length)]);
        password.Append(specialChars[new Random().Next(specialChars.Length)]);

        // מגריל עוד 4 תווים אקראיים מהמאגר הכללי
        string allChars = lowerCase + upperCase + digits + specialChars;
        for (int i = 4; i < 8; i++)
        {
            password.Append(allChars[new Random().Next(allChars.Length)]);
        }

        // מערבב את התווים בתוך הסיסמה כדי למנוע סידור קבוע
        return new string(password.ToString().OrderBy(c => Guid.NewGuid()).ToArray());
    }

    internal static int GetShift => 3; // הסטת תווים עבור ההצפנה

    // פונקציה להצפנת סיסמה
    internal static string EncryptPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new BO.BlInvalidValueExeption("Password cannot be null or empty.");
        }

        char[] encrypted = new char[password.Length];
        for (int i = 0; i < password.Length; i++)
        {
            encrypted[i] = (char)(password[i] + GetShift);
        }
        return new string(encrypted);
    }

    // פונקציה לפענוח סיסמה
    internal static string DecryptPassword(string encryptedPassword)
    {
        if (string.IsNullOrWhiteSpace(encryptedPassword))
        {
            throw new BO.BlInvalidValueExeption("Encrypted password cannot be null or empty.");
        }

        char[] decrypted = new char[encryptedPassword.Length];
        for (int i = 0; i < encryptedPassword.Length; i++)
        {
            decrypted[i] = (char)(encryptedPassword[i] - GetShift);
        }
        return new string(decrypted);
    }

    public static void ValidateStrongPassword(string password)
    {
        // בדיקת אורך
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            throw new BO.BlInvalidValueExeption("The password must be at least 8 characters long.");
        }
        // בדיקת נוכחות של מספר
        if (!Regex.IsMatch(password, @"\d"))
        {
            throw new BO.BlInvalidValueExeption("The password must contain at least one numeric digit.");
        }
        // בדיקת רצפים של יותר מ-2 תווים מאותו סוג
        if (Regex.IsMatch(password, @"(.)\1\1"))
        {
            throw new BO.BlInvalidValueExeption("The password must not contain sequences of more than 2 identical characters.");
        }
    }

    #endregion

    #region distance
   
        private const string ApiKey = "67627d3f6df90712511821ksqbfefac"; // מפתח ה-API שלך

        /// <summary>
        /// מחשבת את המרחק בין שתי קואורדינטות באמצעות נוסחת Haversine.
        /// </summary>
        internal static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadius = 6371000; // רדיוס כדור הארץ במטרים

            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadius * c;
        }

        /// <summary>
        /// מביאה את הקואורדינטות של כתובת נתונה (רוחב ואורך).
        /// </summary>
        public static double[] GetCoordinates(string address)
        {
        //string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key={ApiKey}";
        string apiKey = $"https://geocode.maps.co/search?q=address&api_key=67627d3f6df90712511821ksqbfefac";
        string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key={apiKey}";
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
        using (HttpClient client = new HttpClient())
            {
                var response = client.GetStringAsync(url).Result;

                var results = JsonSerializer.Deserialize<LocationResult[]>(response);
                if (results != null && results.Length > 0)
                {
                    return new double[]
                    {
                    double.Parse(results[0].Lat, CultureInfo.InvariantCulture),
                    double.Parse(results[0].Lon, CultureInfo.InvariantCulture)
                    };
                }
            }
            throw new Exception("No coordinates found.");
        }

        /// <summary>
        /// מחזירה את המרחק בין שתי כתובות.
        /// </summary>
        public static double GetDistance(string address1, string address2)
        {
            var coord1 = GetCoordinates(address1);
            var coord2 = GetCoordinates(address2);
            return CalculateDistance(coord1[0], coord1[1], coord2[0], coord2[1]);
        }

        private class LocationResult
        {
            public string Lat { get; set; }
            public string Lon { get; set; }
        }

    #endregion
}









