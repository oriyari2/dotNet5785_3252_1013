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
    var assignments = s_dal.Assignment.ReadAll(s => (s.VolunteerId== id) && (s.ActualEndTime==null));

        var volunteer = s_dal.Volunteer.Read(id)??
           throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist");
        double volLon = (double)volunteer.Longitude;
        double volLat = (double)volunteer.Latitude;
        AdminImplementation admin = new();
        BO.CallInProgress? callInProgress =
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
              Distance = volunteer?.Address != null ? VolunteerManager.CalculateDistance(volLon, volLat, call.Longitude, call.Latitude) : 0,
  
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
        string emailPattern = @"^[a-zA-Z0-9._%+-]+@(gmail\.com|hotmail\.com|yahoo\.com|walla\.co\.il|outlook\.com|g\.jct\.ac\.il)$";
        if (!Regex.IsMatch(email, emailPattern))
            throw new BO.BlInvalidValueExeption("Invalid Email");
    }


    internal static void IsValidPhoneNumber(string phoneNumber)
    {
        // אם הקלט ריק או מכיל רווחים
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new BO.BlInvalidValueExeption("Invalid Phone Number ");
        // בדיקה שהטלפון מתחיל ב-05, מכיל בדיוק 10 ספרות וכולל רק ספרות
        if( phoneNumber.Length != 10 || !phoneNumber.StartsWith("05") || phoneNumber.All(char.IsDigit)==false)
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


    /// <summary>
    /// מחזירה את המרחק בין שתי כתובות.
    /// </summary>
    public static double GetDistance(string address1, string address2)
    {
        var coord1 = GetCoordinates(address1);
        var coord2 = GetCoordinates(address2);
        return CalculateDistance(coord1[0], coord1[1], coord2[0], coord2[1]);
    }

    internal static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadius = 6371000; // Earth's radius in meters

        // Convert latitude and longitude from degrees to radians
        double lat1Rad = lat1 * Math.PI / 180;
        double lon1Rad = lon1 * Math.PI / 180;
        double lat2Rad = lat2 * Math.PI / 180;
        double lon2Rad = lon2 * Math.PI / 180;

        // Differences in latitude and longitude
        double deltaLat = lat2Rad - lat1Rad;
        double deltaLon = lon2Rad - lon1Rad;

        // Haversine formula
        double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // Final distance in meters
        return EarthRadius * c;

    }

    public static double[] GetCoordinates(string address)//לטפל בחריגות!!!!!
    {
        // Checking if the address is null or empty
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Address cannot be empty or null.", nameof(address));
        }

        string apiKey = "https://geocode.maps.co/search?q=address&api_key=67627d3f6df90712511821ksqbfefac";  // החלף במפתח האמיתי שלך
        string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key={apiKey}";
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

        // Creating a synchronous HTTP request
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        try
        {
            // Sending the request and getting the response synchronously
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                // Checking if the response status is OK
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Error in request: {response.StatusCode}");
                }

                // Reading the response body as a string
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string jsonResponse = reader.ReadToEnd();

                    // Deserializing the JSON response to extract location data
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var results = JsonSerializer.Deserialize<LocationResult[]>(jsonResponse, options);

                    // If no results are found, throwing an exception
                    if (results == null || results.Length == 0)
                    {
                        throw new Exception("No coordinates found for the given address.");
                    }

                    // Returning the latitude and longitude as an array
                    return new double[] { double.Parse(results[0].Lat), double.Parse(results[0].Lon) };
                }
            }
        }
        catch (WebException ex)
        {
            // Handling web exceptions (e.g., network issues)
            throw new Exception("Error sending web request: " + ex.Message);
        }
        catch (Exception ex)
        {
            // Handling general exceptions
            throw new Exception("General error: " + ex.Message);
        }
    }

    /// <summary>
    /// Class to represent the structure of the geocoding response(latitude and longitude)
    /// </summary>
    private class LocationResult
    {
        // Latitude as string in the JSON response
        public string Lat { get; set; }
        // Longitude as string in the JSON response
        public string Lon { get; set; }
    }
    #endregion
}









