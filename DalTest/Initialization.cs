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

    /// <summary>
    ///  Retrieve all existing calls and volunteers- I got help from gpt in this specific method, I sent 
    ///  him the func I wrote that had a problem with expired calls and he helped me fix it
    /// </summary>
    private static void createAssignment()
    {
        var allCalls = s_dal!.Call.ReadAll();
        var allVolunteers = s_dal!.Volunteer.ReadAll();

        // Remove the first two volunteers from the list
        
        var availableVolunteers = (allVolunteers.Skip(2)).ToList();

        // Assignments to be created
        int numAssignments = 50;

        // Track assigned calls to ensure each call appears in only one assignment
        var assignedCalls = new HashSet<int>();

        // Get the current system time
        DateTime currentTime = DateTime.Now;

        // Generate a diversity of assignments
        for (int i = 0; i < numAssignments; i++)
        {
            // Select a random call that has not been assigned yet
            var availableCalls = allCalls.Where(c => !assignedCalls.Contains(c.Id)).ToList();
            if (availableCalls.Count == 0) break; // Stop if no unassigned calls are left

            int callIndex = s_rand.Next(0, availableCalls.Count);
            var selectedCall = availableCalls[callIndex];

            // Select a random volunteer from the reduced list
            int volunteerIndex = s_rand.Next(0, availableVolunteers.Count());
            var selectedVolunteer = availableVolunteers[volunteerIndex];

            // Ensure treatment start time is within valid range
            DateTime treatmentStartTime = selectedCall.OpeningTime.AddHours(s_rand.Next(1, 72));
            if (selectedCall.MaxTimeToEnd.HasValue)
            {
                treatmentStartTime = treatmentStartTime < selectedCall.MaxTimeToEnd.Value
                    ? treatmentStartTime
                    : selectedCall.MaxTimeToEnd.Value.AddMinutes(-10); // Adjust if necessary
            }
            DateTime? treatmentEndTime = null;
            EndType status;

            // Decide assignment status based on diverse scenarios
            if (i % 10 == 0) // Expired call, no treatment
            {
                status = EndType.expired;
                treatmentEndTime = selectedCall.MaxTimeToEnd.HasValue
                    ? selectedCall.MaxTimeToEnd.Value.AddHours(-s_rand.Next(1, 48)) // Ensure end time is before MaxTimeToEnd
                    : currentTime.AddHours(-s_rand.Next(1, 48)); // Ensure end time is in the past
                if (treatmentEndTime >= currentTime)
                {
                    treatmentEndTime = currentTime.AddHours(-s_rand.Next(1, 48)); // Force it into the past
                }
            }
            else if (i % 3 == 0) // Call in treatment
            {
                status = EndType.treated;
            }
            else if (i % 2 == 0) // Completed by manager
            {
                status = EndType.manager;
                treatmentEndTime = treatmentStartTime.AddHours(s_rand.Next(1, 24));
                if (selectedCall.MaxTimeToEnd.HasValue)
                {
                    treatmentEndTime = treatmentEndTime <= selectedCall.MaxTimeToEnd.Value
                        ? treatmentEndTime
                        : selectedCall.MaxTimeToEnd.Value.AddHours(-1); // Ensure within valid range
                }
            }
            else // Self-cancellation
            {
                status = EndType.self;
                treatmentEndTime = selectedCall.MaxTimeToEnd;
            }

            // Add the call to the assigned set
            assignedCalls.Add(selectedCall.Id);

            // Create the assignment
            s_dal!.Assignment.Create(new Assignment(0, selectedCall.Id, selectedVolunteer.Id,
                                                   treatmentStartTime, treatmentEndTime, status));
        }
    }

    /// <summary>
    /// Creates calls in the system with realistic data such as descriptions, locations, and time ranges.
    /// </summary>
    private static void createCall()
    {
        string[] Calldescription =
            { "A volunteer is needed to drive the reserve soldier to his destination",
            "Need two volunteers to take care of the children",
            "Need a volunteer to come shop at the supermarket for the family",
            "We need a volunteer to come prepare a meal for the family.",
            "Need a volunteer to come help with house cleaning"};

        string[] CallAdresses = {
    "Jerusalem, Haneviim St 1, Israel", "Jerusalem, Yad Harutsim St 3, Israel",
    "Jerusalem, Emek Refaim St 8, Israel", "Jerusalem, Shlomtzion HaMalka St 9, Israel",
    "Jerusalem, Harav Kook St 6, Israel", "Jerusalem, HaPalmach St 12, Israel",
    "Jerusalem, Balfour St 4, Israel", "Jerusalem, Agron St 11, Israel",
    "Jerusalem, Shazar Blvd 7, Israel", "Jerusalem, King George St 32, Israel",
    "Jerusalem, Keren HaYesod St 27, Israel", "Jerusalem, Ramban St 9, Israel",
    "Jerusalem, Diskin St 10, Israel", "Jerusalem, HaRav Herzog Blvd 22, Israel",
    "Jerusalem, Zalman Shazar Blvd 15, Israel", "Jerusalem, Tchernichovsky St 33, Israel",
    "Jerusalem, Aza St 19, Israel", "Jerusalem, Ben Yehuda St 55, Israel",
    "Jerusalem, Rambam St 21, Israel", "Jerusalem, Malha Rd 12, Israel",
    "Jerusalem, HaPisga St 2, Israel", "Jerusalem, Kiryat Moshe St 5, Israel",
    "Jerusalem, HaNasi St 14, Israel", "Jerusalem, Aliash St 23, Israel",
    "Jerusalem, Gaza St 26, Israel", "Jerusalem, Ein Kerem St 31, Israel",
    "Jerusalem, Motza St 8, Israel", "Jerusalem, Rashi St 6, Israel",
    "Jerusalem, King David St 20, Israel", "Jerusalem, Hillel St 25, Israel",
    "Jerusalem, Yoel Solomon St 4, Israel", "Jerusalem, Ramat Sharet St 17, Israel",
    "Jerusalem, Ramat Beit HaKerem St 2, Israel", "Jerusalem, Yermiyahu St 3, Israel",
    "Jerusalem, Rabin Blvd 9, Israel", "Jerusalem, Malha Mall, Israel",
    "Jerusalem, Jaffa Rd 120, Israel", "Jerusalem, Agripas St 67, Israel",
    "Jerusalem, Ein Gedi St 10, Israel", "Jerusalem, HaTayasim St 3, Israel",
    "Jerusalem, Mesilat Yesharim St 5, Israel", "Jerusalem, Avraham Shalom Yehuda St 8, Israel",
    "Jerusalem, Begin Blvd 14, Israel", "Jerusalem, Shmu’el HaNavi St 22, Israel",
    "Jerusalem, Golda Meir Blvd 33, Israel", "Jerusalem, Menachem Begin St 40, Israel",
    "Jerusalem, Davidka Square 13, Israel", "Jerusalem, Zion Square 1, Israel",
    "Jerusalem, Eliashar St 5, Israel", "Jerusalem, Bezalel St 3, Israel",
    "Jerusalem, HaMasger St 6, Israel", "Jerusalem, Einstein St 9, Israel",
    "Jerusalem, Ben Gurion St 12, Israel", "Jerusalem, Alpert St 10, Israel",
    "Jerusalem, Abraham Lincoln St 6, Israel", "Jerusalem, Ein Kerem Valley, Israel",
    "Jerusalem, HaKibbutz HaMeuhad St 13, Israel", "Jerusalem, Mesilat Baruch St 20, Israel",
    "Jerusalem, HaMelitz St 14, Israel", "Jerusalem, HaPalmach St 45, Israel",
    "Jerusalem, HaKablan St 33, Israel", "Jerusalem, Rachel Imenu St 5, Israel",
    "Jerusalem, Ben Zakkai St 16, Israel", "Jerusalem, Nachalat Shiva St 8, Israel",
    "Jerusalem, Beit Yaakov St 7, Israel", "Jerusalem, HaHistadrut St 11, Israel",
    "Jerusalem, HaMagid St 6, Israel", "Jerusalem, Harav Kook St 9, Israel",
    "Jerusalem, Yeshayahu St 19, Israel", "Jerusalem, Har Nof Blvd 4, Israel"
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

    /// <summary>
    /// Creates volunteers in the system with realistic attributes such as names, addresses, and roles.
    /// </summary>
    private static void createVolunteer()
    {
        string[] VolNames ={ "Dani Levy", "Eli Amar", "Yair Cohen", "Ariela Levin",
         "Dina Klein", "Shira Israelof","yael cohen", "david azrad", "miri levi", "amir shalom",
        "ronit shahar", "itay ben-david", "maya peretz", "yoni abergil", "lior haddad","rivka azrad" }; //potential names

        string?[] VolAdresses = {"Jerusalem, King George St 1, Israel", "Jerusalem, Jaffa St 23, Israel",
        "Jerusalem, Agripas St 45, Israel","Jerusalem, Ben Yehuda St 67, Israel", "Jerusalem, HaNeviim St 89, Israel",
        "Jerusalem, Yad Harutsim St 101, Israel","Jerusalem, Emek Refaim St 15, Israel",
        "Jerusalem, Malha Rd 33, Israel", "Jerusalem, Harav Kook St 2, Israel", "Jerusalem, Hillel St 40, Israel",
        "Jerusalem, Davidka Square 5, Israel", "Jerusalem, Ramban St 21, Israel",
        "Jerusalem, Tchernichovsky St 37, Israel", "Jerusalem, HaPalmach St 11, Israel",
        "Jerusalem, Keren HaYesod St 90, Israel","Jerusalem, Hapisga St 30, Israel"}; //potential adresses

        double?[] VolLatitudes = {31.776, 31.771, 31.777, 31.779, 31.783, 31.791,  31.758,
        31.750, 31.768, 31.778, 31.761, 31.773,  31.759, 31.780, 31.762,31.78};
        //latitudes corresponding to the list of addresses

        double?[] VolLongitudes = {35.2218, 35.2137, 35.2212, 35.2135, 35.2143, 35.2007, 35.2072,
        35.1907, 35.2091, 35.2181, 35.2133, 35.2046,35.2150, 35.1984, 35.2075,35.21};
        //longitudes corresponding to the list of addresses

        for (int i = 0; i < 16; i++) //create 15 volunteers and 1 manager
        {
            int VolId;
            do
                VolId = s_rand.Next(200000000, 400000000);
            while (s_dal!.Volunteer!.Read(VolId) != null); //until find available id

            RoleType VulRole = (i == 0) ? RoleType.manager : RoleType.volunteer; //1 manager 15 volunteers

            string VolPhone = "05" + s_rand.Next(10000000, 99999999).ToString(); //random valid phone number

            string VolEmail = VolPhone + "@gmail.com"; //valid email with the volunteer phone number

            double VolMaxDis = s_rand.Next(); //random distance

            s_dal!.Volunteer.Create(new(VolId, VolNames[i], VolPhone, VolEmail, null, VolAdresses[i],
            VolLatitudes[i], VolLongitudes[i], VulRole, true, VolMaxDis, DistanceType.air));
            //add new Volunteer with right details to list
        }
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
    public static void Do(IDal dal) //stage 2
    {

        s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); // stage 2

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


