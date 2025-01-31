using DalApi;
using System.Text.Json;
using BlApi;
using BlImplementation;
using System.Text.RegularExpressions;
using System.Text;
using System.Net;
using BO;
using DO;

namespace Helpers;

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
        IEnumerable<DO.Assignment> tempAssignments;
        lock (AdminManager.BlMutex)
            tempAssignments = s_dal.Assignment.ReadAll(s => (s.VolunteerId == id) && (s.TheEndType == endType)); // Get assignments
        var toReturn = tempAssignments == null ? 0 : tempAssignments.Count(); // Count assignments
        return toReturn;
    }

    /// <summary>
    /// Checks if a volunteer has an ongoing call and returns the call's details.
    /// </summary>
    /// <param name="id">Volunteer ID</param>
    /// <returns>Details of the call in progress, if any</returns>
    internal static BO.CallInProgress? callProgress(int id)
    {
        IEnumerable<Assignment> assignments;
        DO.Volunteer? volunteer;
        lock (AdminManager.BlMutex)
        {
            assignments = s_dal.Assignment.ReadAll(s => (s.VolunteerId == id) && (s.ActualEndTime == null) && s.TheEndType != DO.EndType.expired);  // Get assignments
            volunteer = s_dal.Volunteer.Read(id);
        }
        if(volunteer==null)
             throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does not exist"); // Get volunteer details

         
            // בדיקה האם קיימות קורדינטות למתנדב
            double? volLon = volunteer.Longitude;
            double? volLat = volunteer.Latitude;

            AdminImplementation admin = new(); // Instantiate admin for risk range checking
        lock (AdminManager.BlMutex)
        {
            BO.CallInProgress? callInProgress =
             (from item in assignments
              let call = s_dal.Call.Read(item.CallId)
              let tmpstatus = CallManager.CheckStatus(item, call)
              let callLon = call.Longitude
              let callLat = call.Latitude
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

                  // חישוב מרחק רק אם לכל הנתונים יש ערכים
                  Distance = (volLon.HasValue && volLat.HasValue && callLon.HasValue && callLat.HasValue)
                            ? VolunteerManager.CalculateDistance((double)volLat, (double)volLon, (double)callLat, (double)callLon)
                            : 0,

                  status = tmpstatus
              }).FirstOrDefault();
            if (callInProgress!= null && callInProgress.status == BO.Status.expired)
                return null;
            return callInProgress;
        }
    }

    /// <summary>
    /// Retrieves the current active call for a given volunteer ID.
    /// </summary>
    /// <param name="id">Volunteer ID</param>
    /// <returns>The ID of the current call or null if no call is in progress.</returns>
    internal static int? GetCurrentCall(int id)
    {
        BO.CallInProgress? call =  callProgress(id);
        if (call != null)
            return call.CallId;
        else
            return null;
    }


    /// <summary>
    /// Retrieves the type of the call based on the call ID.
    /// </summary>
    /// <param name="id">Call ID</param>
    /// <returns>The call type</returns>
    internal static BO.CallType GetCallType(int id)
    {
        DO.Call? call;
        lock (AdminManager.BlMutex)
            call = s_dal.Call.Read(id); // Get the call by ID
        return (BO.CallType)call.TheCallType;  // Return call type
    }

    #region moveFromImplementation
    internal static BO.Volunteer Read(int id)
    {
        // Try to read the volunteer from the database
        DO.Volunteer? doVolunteer;
        lock (AdminManager.BlMutex)
            doVolunteer = s_dal.Volunteer.Read(id);
       if(doVolunteer==null)
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist");

        // Map the data object (DO) to the business object (BO)
        return new BO.Volunteer()
        {
            Id = id,
            Name = doVolunteer.Name,
            PhoneNumber = doVolunteer.PhoneNumber,
            Email = doVolunteer.Email,
            Password = VolunteerManager.DecryptPassword(doVolunteer.Password), // Decrypt password for the volunteer
            Address = doVolunteer.Address,
            Latitude = doVolunteer.Latitude,
            Longitude = doVolunteer.Longitude,
            Role = (BO.RoleType)doVolunteer.Role,
            Active = doVolunteer.Active,
            MaxDistance = doVolunteer.MaxDistance,
            TheDistanceType = (BO.DistanceType)doVolunteer.TheDistanceType,
            TotalHandled = VolunteerManager.TotalCall(id, DO.EndType.treated), // Get total handled calls
            TotalCanceled = VolunteerManager.TotalCall(id, DO.EndType.self), // Get total canceled calls
            TotalExpired = VolunteerManager.TotalCall(id, DO.EndType.expired), // Get total expired calls
            IsProgress = VolunteerManager.callProgress(id) // Check if the volunteer has any ongoing call
        };
    }

    internal static void Update(int id, BO.Volunteer volunteer)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        BO.Volunteer asker = Read(id); // Get the volunteer that is trying to update
        if (asker.Id != id && asker.Role != BO.RoleType.manager)
            throw new BO.BlUserCantUpdateItemExeption("The asker can't update this Volunteer");

        if (volunteer.Active == false && volunteer.IsProgress != null)
            throw new BO.BlUserCantUpdateItemExeption("This volunteer cant become not active becouse he has call in progress");
        BO.Volunteer oldVolunteer = Read(volunteer.Id); // Get the current volunteer data

        // Validate the updated data
        VolunteerManager.IsValidEmail(volunteer.Email); // Check if the email is valid
        VolunteerManager.IsValidID(volunteer.Id); // Check if the ID is valid
        VolunteerManager.IsValidPhoneNumber(volunteer.PhoneNumber); // Check if the phone number is valid

        string password = volunteer.Password;
        if (password != oldVolunteer.Password)
        {
            VolunteerManager.ValidateStrongPassword(password); // Validate if the password is strong
        }
        password = VolunteerManager.EncryptPassword(password); // Encrypt the password

        // Prevent updating restricted fields
        if (asker.Role != BO.RoleType.manager && volunteer.Role != BO.RoleType.volunteer)
            throw new BO.BlUserCantUpdateItemExeption("Volunteer Can't change the role of the volunteer");

        if (volunteer.TotalCanceled != oldVolunteer.TotalCanceled ||
            volunteer.TotalExpired != oldVolunteer.TotalExpired ||
            volunteer.TotalHandled != oldVolunteer.TotalHandled)
            throw new BO.BlUserCantUpdateItemExeption("Total calls can't be modified!");

        try
        {
            double[]? coordinates = null;

            // אם הכתובת לא השתנתה ויש כבר קואורדינטות קיימות, נשמור אותן
            if (oldVolunteer.Address == volunteer.Address && oldVolunteer.Latitude != null && oldVolunteer.Longitude != null)
            {
                coordinates = new double[] { (double)oldVolunteer.Latitude, (double)oldVolunteer.Longitude };
            }

            // יצירת אובייקט המתנדב עם הקואורדינטות הרלוונטיות
            DO.Volunteer doVolunteer = new(
                volunteer.Id,
                volunteer.Name,
                volunteer.PhoneNumber,
                volunteer.Email,
                password,
                volunteer.Address,
                coordinates != null ? coordinates[0] : null,
                coordinates != null ? coordinates[1] : null,
                (DO.RoleType)volunteer.Role,
                volunteer.Active,
                volunteer.MaxDistance,
                (DO.DistanceType)volunteer.TheDistanceType
            );

            // עדכון המתנדב בבסיס הנתונים
            lock (AdminManager.BlMutex)
            {
                s_dal.Volunteer.Update(doVolunteer);
            }

            // אם הכתובת השתנתה, נשלח לעדכון הקואורדינטות מחדש
            if (coordinates == null)
            {
                _ = VolunteerManager.GetCoordinates(doVolunteer);
            }


            // Create the updated data object (DO) for the volunteer

        }
        catch (DO.DalDoesNotExistException ex)
        {
            // Handle case when the volunteer does not exist
            throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteer.Id} does not exist", ex);
        }
        VolunteerManager.Observers.NotifyItemUpdated(volunteer.Id);//update current volunteer and obserervers etc.
        VolunteerManager.Observers.NotifyListUpdated();//update list of volunteers and obserervers etc.
    }


    #endregion

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

    #region Passwords
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
    public static async Task<double[]> GetCoordinatesAsync(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Address cannot be empty or null.", nameof(address));
        }

        string apiKey = "https://geocode.maps.co/search?q=address&api_key=67627d3f6df90712511821ksqbfefac";
        string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key={apiKey}";

        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode(); // Throws if not 200-299

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = JsonSerializer.Deserialize<LocationResult[]>(jsonResponse, options);

            if (results == null || results.Length == 0)
            {
                throw new Exception("No coordinates found for the given address.");
            }

            return new double[] { double.Parse(results[0].Lat), double.Parse(results[0].Lon) };
        }
        catch (HttpRequestException ex)
        {
            throw new Exception("Error sending web request: " + ex.Message);
        }
        catch (Exception ex)
        {
            throw new Exception("General error: " + ex.Message);
        }
    }



    internal static async Task GetCoordinates(DO.Volunteer doVolunteer)
    {
        if (doVolunteer.Address is not null)
        {
            double[]? loc = await GetCoordinatesAsync(doVolunteer.Address);
            if (loc is not null)
                doVolunteer = doVolunteer with { Latitude = loc[0], Longitude = loc[1] };
            else
                doVolunteer = doVolunteer with { Latitude = null, Longitude = null };
            lock (AdminManager.BlMutex)
                s_dal.Volunteer.Update(doVolunteer);
            Observers.NotifyListUpdated();
            Observers.NotifyItemUpdated(doVolunteer.Id);
        }
    }

    internal static async Task GetCoordinates(DO.Call doCall)
    {
        if (doCall.Address is not null)
        {
            double[]? loc = await GetCoordinatesAsync(doCall.Address);
            if (loc is not null)
            {
                doCall = doCall with { Latitude = loc[0], Longitude = loc[1] };
                lock (AdminManager.BlMutex)
                    s_dal.Call.Update(doCall);
                CallManager.Observers.NotifyListUpdated();
                CallManager.Observers.NotifyItemUpdated(doCall.Id);
            }
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

    #region Simulation
    private static readonly Random s_rand = new();
    private static int s_simulatorCounter = 0;
    private static int s_Counter = 0; //for the func to choose completely random volunteers every iteration

    internal static void SimulateAssignForVolunteer()
    {
        Thread.CurrentThread.Name = $"Simulator{++s_simulatorCounter}";

        IEnumerable<DO.Volunteer> DoVolList;
        lock (AdminManager.BlMutex) //stage 7
            DoVolList = s_dal.Volunteer.ReadAll(v => v.Active == true).ToList();
        foreach (DO.Volunteer doVolunteer in DoVolList)
        {
            CallInProgress? currentCall = callProgress(doVolunteer.Id);
            s_Counter++;
            if (currentCall == null)
            {
                if (s_Counter % 3 == 0)
                {
                    var availableCalls = CallManager.GetOpenCallInList(doVolunteer.Id, null, null);
                    int cntOpenCall = availableCalls.Count();
                    if (cntOpenCall != 0)
                    {
                        int callId = availableCalls.Skip(s_rand.Next(0, cntOpenCall)).First()!.Id;
                        CallManager.ChooseCallToTreat(doVolunteer.Id, callId, true);
                    }
                }
            }
            else
            {
                DateTime maxTime = currentCall.OpeningTime.AddHours(currentCall.Distance).AddMonths(1);
                if (AdminManager.Now > maxTime)
                {
                    CallManager.EndTreatment(doVolunteer.Id, currentCall.Id, true);
                }
                else if (s_Counter % 10 == 0)
                {
                    CallManager.CancelTreatment(doVolunteer.Id, currentCall.Id, true);
                }
            }

        }
    }
    #endregion

}

