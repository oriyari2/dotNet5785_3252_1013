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

namespace Helpers
{
    /// <summary>
    /// The VolunteerManager class handles various operations for managing volunteer-related data.
    /// </summary>
    internal static class VolunteerManager
    {
        internal static ObserverManager Observers = new(); //stage 5 
        private static IDal s_dal = DalApi.Factory.Get;  // Initialize DAL to interact with data layer

        /// <summary>
        /// Calculates the total number of assignments for a volunteer based on the end type.
        /// </summary>
        /// <param name="id">Volunteer ID</param>
        /// <param name="endType">End type of the assignment</param>
        /// <returns>The total number of assignments.</returns>
        internal static int TotalCall(int id, DO.EndType endType)
        {
            var tempAssignments = s_dal.Assignment.ReadAll(s => (s.VolunteerId == id) && (s.TheEndType == endType)); // Get assignments
            var toReturn = tempAssignments == null ? 0 : tempAssignments.Count(); // Count assignments
            return toReturn;
        }

        /// <summary>
        /// Checks if a volunteer has an ongoing call and returns the call's details.
        /// </summary>
        /// <param name="id">Volunteer ID</param>
        /// <returns>Details of the call in progress, if any</returns>
        internal static BO.CallInProgress callProgress(int id)
        {
            var assignments = s_dal.Assignment.ReadAll(s => (s.VolunteerId == id) && (s.ActualEndTime == null));  // Get
                                                                                                                  //
                                                                                                                  //
                                                                                                                  //
                                                                                                                  //
                                                                                                                  //
                                                                                                                  //
                                                                                                                  // assignments
            var volunteer = s_dal.Volunteer.Read(id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist"); // Get volunteer details
            double volLon = (double)volunteer.Longitude;
            double volLat = (double)volunteer.Latitude;
            AdminImplementation admin = new(); // Instantiate admin for risk range checking
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
                  status = (AdminManager.Now - call.MaxTimeToEnd) <= admin.GetRiskRange()
                            ? BO.Status.treatment
                            : BO.Status.riskTreatment
              }).FirstOrDefault();

            return callInProgress;
        }

        /// <summary>
        /// Retrieves the current active call for a given volunteer ID.
        /// </summary>
        /// <param name="id">Volunteer ID</param>
        /// <returns>The ID of the current call or null if no call is in progress.</returns>
        internal static int? GetCurrentCall(int id)
        {
            var assignment = s_dal.Assignment.ReadAll(t => t.ActualEndTime == null && id == t.VolunteerId).FirstOrDefault();  // Get ongoing assignments
            return assignment?.CallId;  // Return current call ID
        }

        /// <summary>
        /// Retrieves the type of the call based on the call ID.
        /// </summary>
        /// <param name="id">Call ID</param>
        /// <returns>The call type</returns>
        internal static BO.CallType GetCallType(int id)
        {
            var call = s_dal.Call.Read(id); // Get the call by ID
            return (BO.CallType)call.TheCallType;  // Return call type
        }

        #region isValidInput
        /// <summary>
        /// Validates if the given email is in a valid format.
        /// </summary>
        /// <param name="email">Email to be validated</param>
        internal static void IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BO.BlInvalidValueExeption("Invalid Email");  // Check for empty email
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@(gmail\.com|hotmail\.com|yahoo\.com|walla\.co\.il|outlook\.com|g\.jct\.ac\.il)$";  // Email regex pattern
            if (!Regex.IsMatch(email, emailPattern))
                throw new BO.BlInvalidValueExeption("Invalid Email");  // Throw exception if email doesn't match pattern
        }

        /// <summary>
        /// Validates if the given phone number follows a valid format.
        /// </summary>
        /// <param name="phoneNumber">Phone number to be validated</param>
        internal static void IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new BO.BlInvalidValueExeption("Invalid Phone Number ");  // Check for empty phone number
            if (phoneNumber.Length != 10 || !phoneNumber.StartsWith("05") || phoneNumber.All(char.IsDigit) == false)
                throw new BO.BlInvalidValueExeption("Invalid Phone Number ");  // Check for valid phone format
        }

        /// <summary>
        /// Validates if the given ID is a valid Israeli ID.
        /// </summary>
        /// <param name="id">ID to be validated</param>
        internal static void IsValidID(int id)
        {
            string idStr = id.ToString();  // Convert ID to string for validation
            if (idStr.Length != 9)
                throw new BO.BlInvalidValueExeption("Invalid Id");  // Check if ID has 9 digits
            int sum = 0;
            for (int i = 0; i < 8; i++)  // Calculate checksum for ID
            {
                int digit = idStr[i] - '0';  // Convert character to digit
                if (i % 2 == 0)
                {
                    sum += digit;
                }
                else
                {
                    int doubled = digit * 2;
                    sum += doubled > 9 ? doubled - 9 : doubled;  // Adjust for values greater than 9
                }
            }
            int checkDigit = (10 - (sum % 10)) % 10;  // Calculate checksum
            if (checkDigit != (id % 10))  // Validate checksum
                throw new BO.BlInvalidValueExeption("Invalid Id");
        }
        #endregion

        #region Passwors
        /// <summary>
        /// Generates a strong random password.
        /// </summary>
        /// <returns>A randomly generated password string.</returns>
        public static string GenerateStrongPassword()
        {
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

            StringBuilder password = new StringBuilder();  // Create a StringBuilder for password construction
            password.Append(lowerCase[new Random().Next(lowerCase.Length)]);  // Add a random lowercase character
            password.Append(upperCase[new Random().Next(upperCase.Length)]);  // Add a random uppercase character
            password.Append(digits[new Random().Next(digits.Length)]);  // Add a random digit
            password.Append(specialChars[new Random().Next(specialChars.Length)]);  // Add a random special character

            string allChars = lowerCase + upperCase + digits + specialChars;
            for (int i = 4; i < 8; i++)  // Add 4 more random characters
            {
                password.Append(allChars[new Random().Next(allChars.Length)]);  // Add random characters from the combined set
            }

            return new string(password.ToString().OrderBy(c => Guid.NewGuid()).ToArray());  // Shuffle the password and return it
        }

        internal static int GetShift => 3;  // Shift value for encryption

        /// <summary>
        /// Encrypts a password by shifting each character.
        /// </summary>
        /// <param name="password">Password to be encrypted</param>
        /// <returns>Encrypted password</returns>
        internal static string EncryptPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new BO.BlInvalidValueExeption("Password cannot be null or empty.");
            }

            char[] encrypted = new char[password.Length];
            for (int i = 0; i < password.Length; i++)
            {
                encrypted[i] = (char)(password[i] + GetShift);  // Shift each character
            }
            return new string(encrypted);  // Return the encrypted password
        }

        /// <summary>
        /// Decrypts an encrypted password by reversing the shift.
        /// </summary>
        /// <param name="encryptedPassword">Encrypted password</param>
        /// <returns>Decrypted password</returns>
        internal static string DecryptPassword(string encryptedPassword)
        {
            if (string.IsNullOrWhiteSpace(encryptedPassword))
            {
                throw new BO.BlInvalidValueExeption("Encrypted password cannot be null or empty.");
            }

            char[] decrypted = new char[encryptedPassword.Length];
            for (int i = 0; i < encryptedPassword.Length; i++)
            {
                decrypted[i] = (char)(encryptedPassword[i] - GetShift);  // Reverse the shift for decryption
            }
            return new string(decrypted);  // Return the decrypted password
        }

        /// <summary>
        /// Validates the strength of the password.
        /// </summary>
        /// <param name="password">Password to be validated</param>
        public static void ValidateStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                throw new BO.BlInvalidValueExeption("The password must be at least 8 characters long.");
            }
            if (!Regex.IsMatch(password, @"\d"))
            {
                throw new BO.BlInvalidValueExeption("The password must contain at least one numeric digit.");
            }
            if (Regex.IsMatch(password, @"(.)\1\1"))
            {
                throw new BO.BlInvalidValueExeption("The password must not contain sequences of more than 2 identical characters.");
            }
        }
        #endregion

        #region distance
        /// <summary>
        /// Returns the distance between two addresses based on their coordinates.
        /// </summary>
        /// <param name="address1">The first address</param>
        /// <param name="address2">The second address</param>
        /// <returns>The distance between the two addresses in meters</returns>
        public static double GetDistance(string address1, string address2)
        {
            var coord1 = GetCoordinates(address1);  // Get coordinates of address1
            var coord2 = GetCoordinates(address2);  // Get coordinates of address2
            return CalculateDistance(coord1[0], coord1[1], coord2[0], coord2[1]);  // Calculate and return distance
        }

        /// <summary>
        /// Calculates the distance between two sets of latitude and longitude coordinates.
        /// </summary>
        /// <param name="lat1">Latitude of the first point</param>
        /// <param name="lon1">Longitude of the first point</param>
        /// <param name="lat2">Latitude of the second point</param>
        /// <param name="lon2">Longitude of the second point</param>
        /// <returns>The distance in meters</returns>
        internal static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadius = 6371000;  // Radius of Earth in meters
            double lat1Rad = lat1 * Math.PI / 180;  // Convert latitude to radians
            double lon1Rad = lon1 * Math.PI / 180;  // Convert longitude to radians
            double lat2Rad = lat2 * Math.PI / 180;  // Convert latitude to radians
            double lon2Rad = lon2 * Math.PI / 180;  // Convert longitude to radians

            double deltaLat = lat2Rad - lat1Rad;  // Latitude difference
            double deltaLon = lon2Rad - lon1Rad;  // Longitude difference

            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);  // Haversine formula

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));  // Calculate angle

            return EarthRadius * c;  // Return distance
        }

        /// <summary>
        /// Gets the coordinates (latitude and longitude) for a given address using a geocoding API.
        /// </summary>
        /// <param name="address">Address to be geocoded</param>
        /// <returns>Array of latitude and longitude values</returns>
        public static double[] GetCoordinates(string address) // Handle exceptions for invalid addresses
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Address cannot be empty or null.", nameof(address));  // Check for null or empty address
            }

            string apiKey = "https://geocode.maps.co/search?q=address&api_key=67627d3f6df90712511821ksqbfefac";  // Replace with your actual API key
            string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key={apiKey}";  // Construct URL
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);  // Create web request

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);  // Create another web request
                request.Method = "GET";  // Set request method to GET

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())  // Get response
                {
                    if (response.StatusCode != HttpStatusCode.OK)  // Check for errors
                    {
                        throw new Exception($"Error in request: {response.StatusCode}");
                    }

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))  // Read the response stream
                    {
                        string jsonResponse = reader.ReadToEnd();  // Get the response body

                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };  // Set JSON options
                        var results = JsonSerializer.Deserialize<LocationResult[]>(jsonResponse, options);  // Deserialize JSON response

                        if (results == null || results.Length == 0)  // If no results found
                        {
                            throw new Exception("No coordinates found for the given address.");
                        }

                        return new double[] { double.Parse(results[0].Lat), double.Parse(results[0].Lon) };  // Return coordinates
                    }
                }
            }
            catch (WebException ex)
            {
                throw new Exception("Error sending web request: " + ex.Message);  // Handle web exceptions
            }
            catch (Exception ex)
            {
                throw new Exception("General error: " + ex.Message);  // Handle other exceptions
            }
        }

        /// <summary>
        /// Class to represent the structure of the geocoding response (latitude and longitude).
        /// </summary>
        private class LocationResult
        {
            public string Lat { get; set; }  // Latitude as string in the JSON response


            public string Lon { get; set; } // Longitude as string in the JSON response
        }
        #endregion
    }
}

