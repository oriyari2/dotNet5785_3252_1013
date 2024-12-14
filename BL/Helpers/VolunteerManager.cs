﻿using DalApi;
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

namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get;
    private static readonly HttpClient client = new HttpClient
    {
        DefaultRequestHeaders =
    {
        UserAgent = { new ProductInfoHeaderValue("GeocodingDistanceProject", "1.0") }
    }
    };
    internal static int TotalCall(int id, DO.EndType endType) => s_dal.Assignment.ReadAll(s => (s.VolunteerId == id) && (s.TheEndType == endType)).Count();
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

              Distance = volunteer?.Address != null ? CalculateDistance(GetCoordinates(volunteer.Address,out double longi,out double lat), 
              GetCoordinates(call.Address ,out double longi1, out double lat1)) : 0,

              status = (ClockManager.Now - call.MaxTimeToEnd) <= admin.GetRiskRange()
                      ? BO.Status.treatment
                      : BO.Status.riskTreatment
          }).First();

        return callInProgress;
    }


    /// <summary>
    /// מקבלת כתובת ומחזירה קואורדינטות של קו רוחב וקו אורך.
    /// </summary>
    internal static void GetCoordinates(string address, out double latitude, out double longitude)
    {
        if (string.IsNullOrEmpty(address))
            throw new BO.InvalidValueExeption("Address must be a non-empty string");

        string baseUrl = "https://nominatim.openstreetmap.org/search";
        string requestUrl = $"{baseUrl}?q={Uri.EscapeDataString(address)}&format=json&addressdetails=1";

        try
        {
            var response = client.GetStringAsync(requestUrl).Result; // סינכרוני
            var data = JArray.Parse(response);

            if (data.Count == 0)
                throw new BO.OurSystemExeption("No coordinates found for the provided address");

            latitude = double.Parse(data[0]["lat"].ToString());
            longitude = double.Parse(data[0]["lon"].ToString());
        }
        catch (Exception ex)
        {
            throw new BO.OurSystemExeption($"Failed to fetch coordinates: {ex.Message}");
        }
    }

    /// <summary>
    /// מחשבת את המרחק האווירי בין שתי נקודות קואורדינטות.
    /// </summary>
    internal static double CalculateDistance((double Latitude, double Longitude) point1, (double Latitude, double Longitude) point2)
    {
        const double EarthRadiusKm = 6371;

        double lat1Rad = DegreeToRadian(point1.Latitude);
        double lon1Rad = DegreeToRadian(point1.Longitude);
        double lat2Rad = DegreeToRadian(point2.Latitude);
        double lon2Rad = DegreeToRadian(point2.Longitude);

        double dlat = lat2Rad - lat1Rad;
        double dlon = lon2Rad - lon1Rad;

        double a = Math.Pow(Math.Sin(dlat / 2), 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * Math.Pow(Math.Sin(dlon / 2), 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c;
    }

    /// <summary>
    /// ממירה מעלות לרדיאנים.
    /// </summary>
    internal static double DegreeToRadian(double degree)
    {
        return degree * Math.PI / 180.0;
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

    internal static void IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new BO.InvalidValueExeption("Invalid Email");
        // Regular expression to match a valid email address
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if( Regex.IsMatch(email, emailPattern)==false)
            throw new BO.InvalidValueExeption("Invalid Email");
    }

    //internal static void IsNumericField(string input)
    //{
    //    // אם הקלט ריק או מכיל רק רווחים
    //    if (string.IsNullOrWhiteSpace(input))
    //        throw new BO.InvalidValueExeption("");
    //    // מנסה להמיר את הקלט למספר שלם (או מספר נקודה צפה) לפי הצורך
    //    if (int.TryParse(input, out _) || double.TryParse(input, out _) == false)
    //        throw new BO.InvalidValueExeption("Invalid Phone Number"); ;
    //}

    internal static void IsValidPhoneNumber(string phoneNumber)
    {
        // אם הקלט ריק או מכיל רווחים
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new BO.InvalidValueExeption("Invalid Phone Number ");
        // בדיקה שהטלפון מתחיל ב-05, מכיל בדיוק 10 ספרות וכולל רק ספרות
        if( phoneNumber.Length == 10 && phoneNumber.StartsWith("05") && phoneNumber.All(char.IsDigit)==false)
            throw new BO.InvalidValueExeption("Invalid Phone Number "); ;
    }

    internal static void IsValidID(int id)
    {
        // הופכים את האינט לסטרינג כדי לבדוק את אורך תעודת הזהות
        string idStr = id.ToString();

        // תעודת זהות חייבת להיות באורך 9 תווים (ללא ספרת ביקורת)
        if (idStr.Length != 9)
            throw new BO.InvalidValueExeption("Invalid Id");
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
            throw new BO.InvalidValueExeption("Invalid Id");
    }


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
            throw new BO.InvalidVlueExeption("Password cannot be null or empty.");
        }

        char[] encrypted = new char[password.Length];
        for (int i = 0; i < password.Length; i++)
        {
            encrypted[i] = (char)(password[i] + 3);
        }
        return new string(encrypted);
    }

    // פונקציה לפענוח סיסמה
    internal static string DecryptPassword(string encryptedPassword)
    {
        if (string.IsNullOrWhiteSpace(encryptedPassword))
        {
            throw new BO.InvalidVlueExeption("Encrypted password cannot be null or empty.");
        }

        char[] decrypted = new char[encryptedPassword.Length];
        for (int i = 0; i < encryptedPassword.Length; i++)
        {
            decrypted[i] = (char)(encryptedPassword[i] - 3);
        }
        return new string(decrypted);
    }

    public static void ValidateStrongPassword(string password)
    {
        // בדיקת אורך
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            throw new BO.InvalidValueExeption("The password must be at least 8 characters long.");
        }
        // בדיקת נוכחות של מספר
        if (!Regex.IsMatch(password, @"\d"))
        {
            throw new BO.InvalidValueExeption("The password must contain at least one numeric digit.");
        }
        // בדיקת רצפים של יותר מ-2 תווים מאותו סוג
        if (Regex.IsMatch(password, @"(.)\1\1"))
        {
            throw new BO.InvalidValueExeption("The password must not contain sequences of more than 2 identical characters.");
        }
    }

};



