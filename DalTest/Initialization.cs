namespace DalTest;
using Dal;
using DalApi;
using DO;
using System;
using System.Data;

/// <summary>
/// Class responsible for initializing the data in the system, including calls, volunteers, and assignments.
/// </summary>
public static class Initialization
{
    private static IDal? s_dal; //stage 2
    private static readonly Random s_rand = new();

    private static void createVolunteer()
    {
        string[] VolNames ={ "Dani Levy", "Eli Amar", "Yair Cohen", "Ariela Levin",
         "Dina Klein", "Shira Israelof","yael cohen", "david azrad", "miri levi", "amir shalom",
        "ronit shahar", "itay ben-david", "maya peretz", "yoni abergil", "lior haddad","rivka azrad" }; //potential names

        string?[] VolAdresses = {
    "King George 1, Jerusalem, Israel",
    "Jaffa 23, Jerusalem, Israel",
    "Agripas 45, Jerusalem, Israel",
    "Ben Yehuda 67, Jerusalem, Israel",
    "HaNeviim 89, Jerusalem, Israel",
    "Yad Harutsim 101, Jerusalem, Israel",
    "Emek Refaim 15, Jerusalem, Israel",
    "Malha Rd 33, Jerusalem, Israel",
    "Harav Kook 2, Jerusalem, Israel",
    "Hillel 40, Jerusalem, Israel",
    "Davidka Square 5, Jerusalem, Israel",
    "Ramban 21, Jerusalem, Israel",
    "Tchernichovsky 37, Jerusalem, Israel",
    "HaPalmach 11, Jerusalem, Israel",
    "Keren HaYesod 90, Jerusalem, Israel",
    "Hapisga 30, Jerusalem, Israel"}; //potential adresses

        double?[] VolLatitudes = {31.776, 31.771, 31.777, 31.779, 31.783, 31.791,  31.758,
        31.750, 31.768, 31.778, 31.761, 31.773,  31.759, 31.780, 31.762,31.78};
        //latitudes corresponding to the list of addresses

        double?[] VolLongitudes = {35.2218, 35.2137, 35.2212, 35.2135, 35.2143, 35.2007, 35.2072,
        35.1907, 35.2091, 35.2181, 35.2133, 35.2046,35.2150, 35.1984, 35.2075,35.21};
        //longitudes corresponding to the list of addresses

        int[] VolIds = {
    218372043, // 
    318207149, // 
    211722251, // 
    235246881,
    208979633, // 
    208773994, // 
    207249780,
    322638370, // 
    316529080, // 
    241441864,
    331295121,
    215041013,
    214363350,
    215089632,
    327887360,
    326549441
    // 
};

        for (int i = 0; i < 16; i++) //create 15 volunteers and 1 manager
        {

            RoleType VulRole = (i == 0) ? RoleType.manager : RoleType.volunteer; //1 manager 15 volunteers

            string VolPhone = "05" + s_rand.Next(10000000, 99999999).ToString(); //random valid phone number

            string VolEmail = VolPhone + "@gmail.com"; //valid email with the volunteer phone number

            double VolMaxDis = s_rand.Next(); //random distance
            string Password = "pS" + VolPhone + "*";
            Password = EncryptPassword(Password);

            s_dal!.Volunteer.Create(new(VolIds[i], VolNames[i], VolPhone, VolEmail, Password, VolAdresses[i],
            VolLatitudes[i], VolLongitudes[i], VulRole, true, VolMaxDis, DistanceType.air));
            //add new Volunteer with right details to list
        }
        s_dal!.Volunteer.Create(new(309366532, "Rivka Azrad", "0508926188", "rivkaazrad@gmail.com", "0508926188*", "Hapisga 10, Jerusalem, Israel",
            31.7683, 35.2137, RoleType.volunteer, false, 60000000, DistanceType.air));
        s_dal!.Volunteer.Create(new(214763252, "Orya Cohen", "0504185888", "aazrad@gmail.com", "0508926188*", "Hapisga 15, Jerusalem, Israel",
            31.7683, 35.2137, RoleType.volunteer, false, 70000000, DistanceType.air));
    }

    private static void createAssignment()
    {
        var allCalls = s_dal!.Call.ReadAll();
        var allVolunteers = s_dal!.Volunteer.ReadAll();

        // Remove the first two volunteers from the list
        var availableVolunteers = allVolunteers.Skip(2).ToList();

        // Keep track of assignments per volunteer and calls in treatment
        var assignmentsPerVolunteer = new Dictionary<int, List<int>>(); // volunteerId -> list of callIds
        var callsInTreatment = new HashSet<int>(); // callIds currently in treatment
        var assignedCalls = new HashSet<int>(); // all assigned calls

        // Get the current system time
        DateTime currentTime = DateTime.Now;

        // Select 5 volunteers who will have multiple assignments
        var specialVolunteers = availableVolunteers.Take(5).ToList();
        var regularVolunteers = availableVolunteers.Skip(5).ToList();

        // First, create assignments for special volunteers (with multiple calls)
        foreach (var volunteer in specialVolunteers)
        {
            assignmentsPerVolunteer[volunteer.Id] = new List<int>();

            // Create one active treatment
            var activeCall = allCalls.FirstOrDefault(c =>
                !assignedCalls.Contains(c.Id) &&
                c.MaxTimeToEnd > currentTime);

            if (activeCall != null)
            {
                DateTime treatmentStartTime = activeCall.OpeningTime.AddHours(s_rand.Next(1, 24));

                s_dal!.Assignment.Create(new Assignment(0, activeCall.Id, volunteer.Id,
                    treatmentStartTime, null, null));

                assignmentsPerVolunteer[volunteer.Id].Add(activeCall.Id);
                assignedCalls.Add(activeCall.Id);
                callsInTreatment.Add(activeCall.Id);
            }

            // Create 3 additional completed/expired assignments
            for (int i = 0; i < 3; i++)
            {
                var call = allCalls.FirstOrDefault(c =>
                    !assignedCalls.Contains(c.Id));

                if (call != null)
                {
                    DateTime treatmentStartTime = call.OpeningTime.AddHours(s_rand.Next(1, 24));
                    DateTime? treatmentEndTime;
                    EndType? status;

                    // Alternate between treated and expired
                    if (call.MaxTimeToEnd <= currentTime)
                    {
                        status = EndType.expired;
                        treatmentEndTime = call.MaxTimeToEnd;
                    }
                    else
                    {
                        status = EndType.treated;
                        treatmentEndTime = treatmentStartTime.AddHours(s_rand.Next(1, 24));
                        if (call.MaxTimeToEnd.HasValue && treatmentEndTime > call.MaxTimeToEnd.Value)
                        {
                            treatmentEndTime = call.MaxTimeToEnd.Value.AddHours(-1);
                        }
                    }

                    s_dal!.Assignment.Create(new Assignment(0, call.Id, volunteer.Id,
                        treatmentStartTime, treatmentEndTime, status));

                    assignmentsPerVolunteer[volunteer.Id].Add(call.Id);
                    assignedCalls.Add(call.Id);
                }
            }
        }

        // Create single assignments for regular volunteers
        foreach (var volunteer in regularVolunteers)
        {
            var call = allCalls.FirstOrDefault(c =>
                !assignedCalls.Contains(c.Id));

            if (call != null)
            {
                DateTime treatmentStartTime = call.OpeningTime.AddHours(s_rand.Next(1, 24));
                DateTime? treatmentEndTime;
                EndType? status;

                if (call.MaxTimeToEnd <= currentTime)
                {
                    status = EndType.expired;
                    treatmentEndTime = call.MaxTimeToEnd;
                }
                else if (s_rand.Next(2) == 0)
                {
                    status = EndType.treated;
                    treatmentEndTime = treatmentStartTime.AddHours(s_rand.Next(1, 24));
                    if (call.MaxTimeToEnd.HasValue && treatmentEndTime > call.MaxTimeToEnd.Value)
                    {
                        treatmentEndTime = call.MaxTimeToEnd.Value.AddHours(-1);
                    }
                }
                else
                {
                    status = null;
                    treatmentEndTime = null;
                }

                s_dal!.Assignment.Create(new Assignment(0, call.Id, volunteer.Id,
                    treatmentStartTime, treatmentEndTime, status));

                assignedCalls.Add(call.Id);
            }
        }
    }    /// <summary>
    private static void createCall()
    {
        string[] Calldescription =
            { "Need a volunteer to to drive the reserve soldier to his destination",
            "Need a volunteer to take care of the children",
            "Need a volunteer to come shop at the supermarket for the family",
            "Need a volunteer to come prepare a meal for the family.",
            "Need a volunteer to come help with house cleaning"};

        string[] CallAdresses = {
   "Haneviim 1, Jerusalem, Israel",
    "Yad Harutsim 3, Jerusalem, Israel",
    "Emek Refaim 8, Jerusalem, Israel",
    "Yad Harutsim 9, Jerusalem, Israel",
    "Harav Kook 6, Jerusalem, Israel",
    "HaPalmach 12, Jerusalem, Israel",
    "Balfour 4, Jerusalem, Israel",
    "Agron 11, Jerusalem, Israel",
    "Shazar 7, Jerusalem, Israel",
    "King George 32, Jerusalem, Israel",
    "Keren HaYesod 27, Jerusalem, Israel",
    "Ramban 9, Jerusalem, Israel",
    "Diskin 10, Jerusalem, Israel",
    "HaRav Herzog 22, Jerusalem, Israel",
    "Zalman Shazar 15, Jerusalem, Israel",
    "Tchernichovsky 33, Jerusalem, Israel",
    "Aza 19, Jerusalem, Israel",
    "Ben Yehuda 55, Jerusalem, Israel",
    "Rambam 21, Jerusalem, Israel",
    "Malha Rd 12, Jerusalem, Israel",
    "HaPisga 2, Jerusalem, Israel",
    "Kiryat Moshe 5, Jerusalem, Israel",
    "HaNasi 14, Jerusalem, Israel",
    "Aliash 23, Jerusalem, Israel",
    "Gaza 26, Jerusalem, Israel",
    "Ein Kerem 31, Jerusalem, Israel",
    "Motza 8, Jerusalem, Israel",
    "Rashi 6, Jerusalem, Israel",
    "King David 20, Jerusalem, Israel",
    "Hillel 25, Jerusalem, Israel",
    "Yoel Solomon 4, Jerusalem, Israel",
    "Ramat Beit HaKerem 4, Jerusalem, Israel",
    "Ramat Beit HaKerem 2, Jerusalem, Israel",
    "Yermiyahu 3, Jerusalem, Israel",
    "Rabin 9, Jerusalem, Israel",
    "Malha Mall, Jerusalem, Israel",
    "Jaffa Rd 120, Jerusalem, Israel",
    "Agripas 67, Jerusalem, Israel",
    "Ein Gedi 10, Jerusalem, Israel",
    "HaTayasim 3, Jerusalem, Israel",
    "Mesilat Yesharim 5, Jerusalem, Israel",
    "Avraham Shalom Yehuda 8, Jerusalem, Israel",
    "Begin 14, Jerusalem, Israel",
    "Shmu’el HaNavi 22, Jerusalem, Israel",
    "Golda Meir 33, Jerusalem, Israel",
    "Menachem Begin 40, Jerusalem, Israel",
    "Davidka Square 13, Jerusalem, Israel",
    "Zion Square 1, Jerusalem, Israel",
    "Eliashar 5, Jerusalem, Israel",
    "Bezalel 3, Jerusalem, Israel",
    "HaMasger 6, Jerusalem, Israel",
    "Einstein 9, Jerusalem, Israel",
    "Ben Gurion 12, Jerusalem, Israel",
    "Alpert 10, Jerusalem, Israel",
    "Abraham Lincoln 6, Jerusalem, Israel",
    "Ein Kerem Valley, Jerusalem, Israel",
    "HaKibbutz HaMeuhad 13, Jerusalem, Israel",
    "HaPalmach 10, Jerusalem, Israel",
    "HaMelitz 14, Jerusalem, Israel",
    "HaPalmach 45, Jerusalem, Israel",
    "HaKablan 33, Jerusalem, Israel",
    "Rachel Imenu 5, Jerusalem, Israel",
    "Ben Zakkai 16, Jerusalem, Israel",
    "Nachalat Shiva 8, Jerusalem, Israel",
    "Beit Yaakov 7, Jerusalem, Israel",
    "HaHistadrut 11, Jerusalem, Israel",
    "HaMagid 6, Jerusalem, Israel",
    "Harav Kook 9, Jerusalem, Israel",
    "Yeshayahu 19, Jerusalem, Israel",
    "Har Nof 4, Jerusalem, Israel",
    "HaNeviim 15, Jerusalem, Israel",
    "Beit Hakerem 12, Jerusalem, Israel",
    "Azza 21, Jerusalem, Israel",
    "Bar Ilan 35, Jerusalem, Israel"

};

        double[] CallLatitudes = {
    31.778, 31.773, 31.762, 31.770, 31.767, 31.759, 31.764, 31.771,
31.780, 31.776, 31.763, 31.758, 31.770, 31.777, 31.771, 31.779,
31.762, 31.758, 31.773, 31.750, 31.765, 31.768, 31.766, 31.764,
31.757, 31.755, 31.770, 31.775, 31.761, 31.778, 31.768, 31.753,
31.767, 31.770, 31.759, 31.752, 31.771, 31.761, 31.776, 31.769,
31.770, 31.760, 31.764, 31.762, 31.775, 31.759, 31.772, 31.766,
31.767, 31.761, 31.773, 31.775, 31.754, 31.751, 31.759, 31.762,
31.766, 31.768, 31.770, 31.761, 31.764, 31.763, 31.767, 31.758,
31.773, 31.756, 31.772, 31.764, 31.757, 31.765
};// Array containing the latitudes of the addrsses

        double[] CallLongitudes = {
    35.216, 35.213, 35.217, 35.214, 35.212, 35.211, 35.210, 35.217,
35.218, 35.213, 35.212, 35.209, 35.215, 35.213, 35.208, 35.214,
35.216, 35.214, 35.207, 35.206, 35.215, 35.209, 35.208, 35.207,
35.213, 35.211, 35.208, 35.212, 35.217, 35.218, 35.210, 35.216,
35.211, 35.219, 35.215, 35.214, 35.217, 35.218, 35.216, 35.212,
35.215, 35.217, 35.213, 35.211, 35.214, 35.209, 35.210, 35.207,
35.208, 35.209, 35.212, 35.215, 35.216, 35.210, 35.214, 35.217,
35.208, 35.209, 35.215, 35.217, 35.213, 35.211, 35.208, 35.215,
35.218, 35.212, 35.210, 35.215, 35.219, 35.214
};// Array containing the longitudes of the addresses

        for (int i = 0; i < 70; i++) // Loop through 70 iterations
        {
            string address = CallAdresses[i]; // Get address from the list
            double longitude = CallLongitudes[i]; // Get longitude from the list
            double latitude = CallLatitudes[i]; // Get latitude from the list
            string? description = ""; // Initialize description as an empty string
            CallType callType = CallType.food; // Default call type is food
            DateTime start = new DateTime(s_dal!.Config.Clock.Year - 1, i % 12 + 1, i % 28 + 1); // Set call start date to one year before the current year
            DateTime end = new DateTime(s_dal!.Config.Clock.Year + 1, i % 12 + 1, i % 28 + 1); // Set call end date to one year after the current year
            if (i % 10 == 0) // If the current index is divisible by 10
            {
                end = start; // Set the end date to the same as the start date (expired)
            }
            if (i < 10) // If index is less than 10
            {
                callType = CallType.food; // Call type is food
                description = Calldescription[3]; // Set description from the array
            }
            else if (i >= 10 && i < 30) // If index is between 10 and 29
            {
                callType = CallType.Cleaning; // Call type is Cleaning
                description = Calldescription[4]; // Set description from the array
            }
            else if (i >= 30 && i < 45) // If index is between 30 and 44
            {
                callType = CallType.Transportation; // Call type is Transportation
                description = Calldescription[0]; // Set description from the array
            }
            else if (i >= 45 && i < 55) // If index is between 45 and 54
            {
                callType = CallType.Babysitting; // Call type is Babysitting
                description = Calldescription[1]; // Set description from the array
            }
            else if (i >= 55 && i < 70) // If index is between 55 and 69
            {
                callType = CallType.Shopping; // Call type is Shopping
                description = Calldescription[2]; // Set description from the array
            }

            s_dal!.Call.Create(new(0, callType, description, address, latitude, longitude, start, end)); // Create a new call entry in the database
        }
    }

    internal static int GetShift => 3; // הסטת תווים עבור ההצפנה

    // פונקציה להצפנת סיסמה
    internal static string EncryptPassword(string password)
    {
 
        char[] encrypted = new char[password.Length];
        for (int i = 0; i < password.Length; i++)
        {
            encrypted[i] = (char)(password[i] + GetShift);
        }
        return new string(encrypted);
    }

    /// <summary>
    /// Initializes the system by resetting configuration values, deleting existing data,
    /// and populating the database with sample volunteers, calls, and assignments.
    /// </summary>
    /// <param name="dalCall">The data access layer for call operations.</param>
    /// <param name="dalAssignment">The data access layer for assignment operations.</param>
    /// <param name="dalVolunteer">The data access layer for volunteer operations.</param>
    /// <param name="dalConfig">The data access layer for configuration settings.</param>
    /// <exception cref="NullReferenceException">Thrown if any of the provided DAL objects is null.</exception>
    public static void Do() //stage 4
    {

        s_dal = DalApi.Factory.Get; //stage 4
        Console.WriteLine("Reset Configuration values and List values...");

        s_dal.ResetDB();//stage 2
        Console.WriteLine("Initializing volunteer list ..."); // Log message for volunteer list initialization
        createVolunteer(); // Create and populate the volunteer list

        Console.WriteLine("Initializing Call list ..."); // Log message for call list initialization
        createCall(); // Create and populate the call list

        Console.WriteLine("Initializing Assignment list ..."); // Log message for assignment list initialization
        createAssignment(); // Create and populate the assignment list
    }

}


