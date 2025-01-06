using BO;
using DO;
using System;
using System.Data;

namespace BlTest;

/// <summary>
/// Main program class containing the entry point and menu options for different sections (Admin, Call, Volunteer).
/// </summary>
internal class Program
{
    // The business logic layer (BL) API, which provides access to the application's functionality.
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    /// <summary>
    /// Enum for the main menu options. Represents different main menu choices for the user.
    /// </summary>
    public enum MainMenuOptions { Exit, Admin, Call, Volunteer }

    /// <summary>
    /// Enum for the admin menu options. Represents different actions available in the Admin menu.
    /// </summary>
    public enum AdminMenuOptions { Exit, GetClock, AdvanceClock, GetRiskRange, SetRiskRange, Reset, Initialize }

    /// <summary>
    /// Enum for the call menu options. Represents different actions available in the Call menu.
    /// </summary>
    public enum CallMenuOptions { Exit, CallsAmount, ReadAll, Read, Update, Delete, Create, GetClosedCallList, GetOpenCallList, EndTreatment, CancelTreatment, ChooseCallToTreat }

    /// <summary>
    /// Enum for the volunteer menu options. Represents different actions available in the Volunteer menu.
    /// </summary>
    public enum VolunteerMenuOptions { Exit, LogIn, ReadAll, Read, Update, Delete, Create }



/// <summary>
/// Main entry point for the program. Displays the main menu and handles user input to navigate through different options.
/// </summary>
static void Main(string[] args)
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\n--- Main Menu ---");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Admin");
            Console.WriteLine("2. Call");
            Console.WriteLine("3. Volunteer");

            // Parse user input to navigate to the selected menu
            if (Enum.TryParse(Console.ReadLine(), out MainMenuOptions choice))
            {
                switch (choice)
                {
                    case MainMenuOptions.Exit:
                        exit = true;  // Exit the program
                        break;

                    case MainMenuOptions.Admin:
                        AdminMenu();  // Go to the Admin menu
                        break;

                    case MainMenuOptions.Call:
                        CallMenu();  // Go to the Call menu
                        break;

                    case MainMenuOptions.Volunteer:
                        VolunteerMenu();  // Go to the Volunteer menu
                        break;

                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }
        }

        Console.WriteLine("Program exited successfully.");
    }

    /// <summary>
    /// Displays the Admin menu and allows the user to select various admin options like managing the clock and risk range.
    /// </summary>
    private static void AdminMenu()
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\n--- Admin Menu ---");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Get Clock");
            Console.WriteLine("2. Advance Clock");
            Console.WriteLine("3. Get Risk Range");
            Console.WriteLine("4. Set Risk Range");
            Console.WriteLine("5. Reset");
            Console.WriteLine("6. Initialize");

            // Parse user input for Admin menu selection
            if (Enum.TryParse(Console.ReadLine(), out AdminMenuOptions choice))
            {
                try
                {
                    switch (choice)
                    {
                        case AdminMenuOptions.Exit:
                            exit = true;  // Exit the Admin menu
                            break;

                        case AdminMenuOptions.GetClock:
                            // Display the current clock time
                            Console.WriteLine(s_bl.Admin.GetClock());
                            break;

                        case AdminMenuOptions.AdvanceClock:
                            // Allow the user to advance the clock by a chosen time unit
                            Console.Write("Enter time unit to advance:\n" +
                                "0. Minute\n1. Hour\n2. Day\n3. Month\n4. Year\n");
                            BO.TimeUnit timeUnit;
                            while (!Enum.TryParse(Console.ReadLine(), out timeUnit))
                            {
                                Console.WriteLine("Invalid Time Unit");
                            }
                            s_bl.Admin.AdvanceClock(timeUnit);
                            Console.WriteLine(s_bl.Admin.GetClock());  // Display updated clock
                            break;

                        case AdminMenuOptions.GetRiskRange:
                            // Display the current risk range
                            Console.WriteLine(s_bl.Admin.GetRiskRange());
                            break;

                        case AdminMenuOptions.SetRiskRange:
                            // Prompt for a new risk range and set it
                            Console.Write("Enter risk range (hh:mm:ss): ");
                            TimeSpan range;
                            while (!TimeSpan.TryParse(Console.ReadLine(), out range))
                                Console.Write("Invalid risk range\nEnter risk range (hh:mm:ss):  ");
                            s_bl.Admin.SetRiskRange(range);
                            break;

                        case AdminMenuOptions.Reset:
                            // Reset the system to initial state
                            s_bl.Admin.Reset();
                            break;

                        case AdminMenuOptions.Initialize:
                            // Initialize the system
                            s_bl.Admin.Intialize();
                            break;

                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    PrintException(ex);  // Handle any exceptions that occur
                }
            }
            else
            {
                Console.WriteLine("Invalid input.");  // Handle invalid input
            }
        }
    }


    /// <summary>
    /// Displays the Call menu and handles the corresponding actions based on user input.
    /// </summary>
    private static void CallMenu()
    {
        bool exit = false; // Flag to control the menu loop.

        while (!exit) // Loop until the user chooses to exit.
        {
            // Display the menu options.
            Console.WriteLine("\n--- Call Menu ---");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Calls Amount");
            Console.WriteLine("2. Read All");
            Console.WriteLine("3. Read");
            Console.WriteLine("4. Update");
            Console.WriteLine("5. Delete");
            Console.WriteLine("6. Create");
            Console.WriteLine("7. Get Closed Call List");
            Console.WriteLine("8. Get Open Call List");
            Console.WriteLine("9. End Treatment");
            Console.WriteLine("10. Cancel Treatment");
            Console.WriteLine("11. Choose Call To Treat");

            // Try parsing user input to a CallMenuOptions enum.
            if (Enum.TryParse(Console.ReadLine(), out CallMenuOptions choice))
            {
                try
                {
                    // Switch case to handle the selected menu option.
                    switch (choice)
                    {
                        case CallMenuOptions.Exit:
                            exit = true; // Set exit to true to break the loop.
                            break;

                        case CallMenuOptions.CallsAmount:
                            var amounts = s_bl.Call.CallsAmount().ToArray(); // Get the number of calls per status.
                            for (int i = 0; i < amounts.Length; i++) // Loop through each amount.
                                Console.WriteLine($"{(BO.Status)i}: {amounts[i]}"); // Print each status and amount.
                            break;

                        case CallMenuOptions.ReadAll:
                            ReadAllCallHelp(); // Call helper function to read all calls.
                            break;

                        case CallMenuOptions.Read:
                            Console.Write("Enter call ID: "); // Prompt for call ID.
                            if (int.TryParse(Console.ReadLine(), out int id)) // Try parsing the ID.
                                Console.WriteLine(s_bl.Call.Read(id)); // Read and print the call.
                            else
                                Console.WriteLine("Invalid Id."); // Handle invalid input.
                            break;

                        case CallMenuOptions.Update:
                            BO.Call callUpdate = CallCreateUpdate(true); // Get the call to update.
                            s_bl.Call.Update(callUpdate); // Update the call.
                            break;

                        case CallMenuOptions.Delete:
                            Console.Write("Enter call ID to delete: "); // Prompt for call ID to delete.
                            if (int.TryParse(Console.ReadLine(), out int deleteId)) // Try parsing the ID.
                            {
                                s_bl.Call.Delete(deleteId); // Delete the call.
                                Console.WriteLine("Call deleted successfully."); // Confirmation message.
                            }
                            else
                                Console.WriteLine("Invalid Id."); // Handle invalid input.
                            break;

                        case CallMenuOptions.Create:
                            BO.Call callCreate = CallCreateUpdate(false); // Get the call to create.
                            s_bl.Call.Create(callCreate); // Create the new call.
                            break;

                        case CallMenuOptions.GetClosedCallList:
                            // Collect parameters for filtering the closed call list.
                            HelpGetCallList(out int volunteerId, out BO.CallType? callTypeFilter);
                            BO.FieldsClosedCallInList? sortField = sortFilterClose(); // Get sort filter for closed calls.
                            var closeCallList = s_bl.Call.GetClosedCallInList(volunteerId, callTypeFilter, sortField); // Get the list of closed calls.
                            Console.WriteLine("Call Closed list for volunteer: "); // Print header.
                            foreach (var call in closeCallList) // Print each closed call.
                                Console.WriteLine(call);
                            break;

                        case CallMenuOptions.GetOpenCallList:
                            // Collect parameters for filtering the open call list.
                            HelpGetCallList(out int volId, out BO.CallType? filter);
                            BO.FieldsOpenCallInList? sort = sortFilterOpen(); // Get sort filter for open calls.
                            var openCallList = s_bl.Call.GetOpenCallInList(volId, filter, sort); // Get the list of open calls.
                            Console.WriteLine("Call Open list for volunteer: "); // Print header.
                            foreach (var call in openCallList) // Print each open call.
                                Console.WriteLine(call);
                            break;

                        case CallMenuOptions.EndTreatment:
                            Console.Write("Enter volunteer ID to end treatment of current call in treatment: "); // Prompt for volunteer ID.
                            if (int.TryParse(Console.ReadLine(), out int volunteerId2)) // Try parsing the ID.
                            {
                                var assignmentProgress = s_bl.Volunteer.Read(volunteerId2).IsProgress; // Check if the volunteer has ongoing treatment.
                                if (assignmentProgress == null)
                                    Console.WriteLine("No Call In Treatment Of Volunteer"); // Handle case where no treatment is in progress.
                                else
                                {
                                    s_bl.Call.EndTreatment(volunteerId2, assignmentProgress.Id); // End the treatment for the volunteer.
                                    Console.WriteLine("Treatment ended successfully."); // Confirmation message.
                                }
                            }
                            else
                                Console.WriteLine("Invalid Id."); // Handle invalid ID input.
                            break;

                        case CallMenuOptions.CancelTreatment:
                            Console.Write("Enter your ID : "); // Prompt for the requester's ID.
                            if (int.TryParse(Console.ReadLine(), out int requesterId)) // Try parsing the ID.
                            {
                                if (s_bl.Volunteer.Read(requesterId).Role == BO.RoleType.manager) // Check if the requester is a manager.
                                {
                                    Console.Write("Enter volunteer ID to cancel treatment of current call in treatment: "); // Prompt for volunteer ID.
                                    if (!int.TryParse(Console.ReadLine(), out requesterId)) // Try parsing the ID again.
                                    {
                                        Console.WriteLine("Invalid Id.");
                                        break;
                                    }
                                }

                                var assignmentProgress = s_bl.Volunteer.Read(requesterId).IsProgress; // Check if the volunteer has ongoing treatment.
                                if (assignmentProgress == null)
                                    Console.WriteLine("No Call In Treatment Of Volunteer"); // Handle case where no treatment is in progress.
                                else
                                {
                                    s_bl.Call.CancelTreatment(requesterId, assignmentProgress.Id); // Cancel the treatment.
                                    Console.WriteLine("Treatment canceled successfully."); // Confirmation message.
                                }
                            }
                            else
                                Console.WriteLine("Invalid Id."); // Handle invalid ID input.
                            break;

                        case CallMenuOptions.ChooseCallToTreat:
                            // Collect parameters for choosing a call to treat.
                            HelpGetCallList(out int chooserId, out BO.CallType? callType);
                            BO.FieldsOpenCallInList? toSort = sortFilterOpen(); // Get sort filter for open calls.
                            Console.WriteLine("Choose a call to treat:");
                            var openCall = s_bl.Call.GetOpenCallInList(chooserId, callType, toSort); // Get the list of open calls.
                            foreach (var call in openCall) // Print each open call.
                                Console.WriteLine(call);

                            Console.Write("Enter call ID to treat: "); // Prompt for call ID to treat.
                            if (int.TryParse(Console.ReadLine(), out int treatId)) // Try parsing the ID.
                            {
                                s_bl.Call.ChooseCallToTreat(chooserId, treatId); // Treat the selected call.
                                Console.WriteLine("Call is now being treated."); // Confirmation message.
                            }
                            else
                                Console.WriteLine("Invalid Id."); // Handle invalid ID input.
                            break;

                        default:
                            Console.WriteLine("Invalid option."); // Handle invalid menu option.
                            break;
                    }
                }
                catch (Exception ex) // Catch any exceptions and print them.
                {
                    PrintException(ex); // Print exception details.
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number between 0 and 12."); // Handle invalid input.
            }
        }
    }


    /// <summary>
    /// Prints details of an exception, including its type, message, and any inner exception details.
    /// </summary>
    private static void PrintException(Exception ex)
    {
        Console.WriteLine($"Exception: {ex.GetType().Name}");
        Console.WriteLine($"Message: {ex.Message}");
        if (ex.InnerException != null) Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
    }


    /// <summary>
    /// Displays the Volunteer menu and handles the corresponding actions based on user input.
    /// </summary>
    private static void VolunteerMenu()
    {
        bool exit = false; // Flag to control the menu loop.

        while (!exit) // Loop until the user chooses to exit.
        {
            // Display the menu options.
            Console.WriteLine("\n--- Volunteer Menu ---");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Log In");
            Console.WriteLine("2. Read All");
            Console.WriteLine("3. Read");
            Console.WriteLine("4. Update");
            Console.WriteLine("5. Delete");
            Console.WriteLine("6. Create");

            // Try parsing user input to a VolunteerMenuOptions enum.
            if (Enum.TryParse(Console.ReadLine(), out VolunteerMenuOptions choice))
            {
                try
                {
                    // Switch case to handle the selected menu option.
                    switch (choice)
                    {
                        case VolunteerMenuOptions.Exit:
                            exit = true; // Set exit to true to break the loop.
                            break;

                        case VolunteerMenuOptions.LogIn:
                            BO.RoleType role = LogInHelp(); // Get the role of the volunteer based on login.
                            Console.Write("Volunteer Role is: " + role); // Display the volunteer's role.
                            break;

                        case VolunteerMenuOptions.ReadAll:
                            ReadAllVolunteerHelp(); // Call helper function to read all volunteers.
                            break;

                        case VolunteerMenuOptions.Read:
                            Console.Write("Enter Volunteer ID to Read: "); // Prompt for volunteer ID.
                            if (int.TryParse(Console.ReadLine(), out int readId)) // Try parsing the ID.
                            {
                                var volunteer = s_bl.Volunteer.Read(readId); // Read the volunteer with the given ID.
                                Console.WriteLine(volunteer); // Print the volunteer details.
                            }
                            else
                                Console.WriteLine("Invalid ID format."); // Handle invalid ID input.
                            break;

                        case VolunteerMenuOptions.Update:
                            HelpUpdateVolunteer(); // Call helper function to update volunteer details.
                            Console.WriteLine("New volunteer updated successfully."); // Confirmation message.
                            break;

                        case VolunteerMenuOptions.Delete:
                            Console.Write("Enter Volunteer ID to Delete: "); // Prompt for volunteer ID to delete.
                            if (int.TryParse(Console.ReadLine(), out int deleteId)) // Try parsing the ID.
                            {
                                s_bl.Volunteer.Delete(deleteId); // Delete the volunteer with the given ID.
                                Console.WriteLine($"Volunteer with ID {deleteId} deleted successfully."); // Confirmation message.
                            }
                            else
                                Console.WriteLine("Invalid ID format."); // Handle invalid ID input.
                            break;

                        case VolunteerMenuOptions.Create:
                            BO.Volunteer newVol = HelpCreateVolunteer(); // Create a new volunteer object using a helper function.
                            s_bl.Volunteer.Create(newVol); // Create the new volunteer.
                            Console.WriteLine("New volunteer created successfully."); // Confirmation message.
                            break;

                        default:
                            Console.WriteLine("Option not implemented."); // Handle unimplemented options.
                            break;
                    }
                }
                catch (Exception ex) // Catch any exceptions and print them.
                {
                    PrintException(ex); // Print exception details.
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number between 0 and 6."); // Handle invalid input.
            }
        }
    }


    /// <summary>
    /// Creates or updates a call based on user input.
    /// </summary>
    /// <param name="type">Indicates whether the method is for creating or updating a call. 
    /// True indicates an update, false indicates creation.</param>
    /// <returns>A <see cref="BO.Call"/> object with the details entered by the user.</returns>
    private static BO.Call CallCreateUpdate(bool type)
    {
        Console.WriteLine("Enter call details:");

        int id = 0; // Default value for the call ID.

        // If the method is used for updating a call, prompt for the call ID.
        if (type == true)
        {
            Console.WriteLine("Enter call Id:");
            if (!int.TryParse(Console.ReadLine(), out id)) // Try parsing the ID entered by the user.
                Console.WriteLine("Invalid Id."); // Handle invalid input.
        }

        // Prompt the user to select the call type from predefined options.
        Console.Write("Call Type\n0: Transportation\n1: Babysitting\n2: Shopping\n3: Food\n4: Cleaning:\n");
        BO.CallType callType; // Variable to hold the call type.

        // Keep prompting until a valid call type is entered.
        while (!Enum.TryParse(Console.ReadLine(), out callType))
        {
            Console.WriteLine("Invalid Call Type."); // Handle invalid input for call type.
        }

        // Prompt for the verbal description of the call.
        Console.Write("Verbal Description:\n");
        string? description = Console.ReadLine();

        // Prompt for the address of the call.
        Console.Write("Address: ([Street Name] [Building/House Number], [City], [Country])\n");
        string? address = Console.ReadLine();

        // Prompt for the maximum time to end the call (optional).
        Console.Write("Max Time To End (yyyy-MM-dd HH:mm) [optional]:\n");
        DateTime? maxTimeToEnd = DateTime.TryParse(Console.ReadLine(), out DateTime maxEndTime) ? maxEndTime : null;

        // If max time to end is not provided, notify the user.
        if (maxTimeToEnd == null)
        {
            Console.WriteLine("Invalid max time to end, Create call without Max time to end...");
        }

        // Create a new BO.Call object with the gathered details.
        BO.Call call = new BO.Call()
        {
            Id = id, // Call ID.
            TheCallType = callType, // Call type.
            VerbalDescription = description, // Description of the call.
            Address = address, // Address where the call will take place.
            Latitude = 0, // Default latitude (could be modified later).
            Longitude = 0, // Default longitude (could be modified later).
            OpeningTime = DateTime.Now, // Time when the call is created or updated.
            MaxTimeToEnd = maxTimeToEnd, // Maximum time to end the call (optional).
            status = BO.Status.treatment, // Default status is 'treatment'.
            listAssignForCall = null // No assignments initially.
        };

        // Return the created or updated call object.
        return call;
    }


    /// <summary>
    /// Prompts the user to log in by entering their name and password, and returns the volunteer's role type.
    /// </summary>
    /// <returns>A <see cref="BO.RoleType"/> representing the volunteer's role.</returns>
    private static BO.RoleType LogInHelp()
    {
        Console.Write("Enter Volunteer Id to Log In: ");
        int.TryParse(Console.ReadLine(), out int id); // Get volunteer name input.

        Console.Write("Enter Volunteer Password to Log In: ");
        string? password = Console.ReadLine(); // Get volunteer password input.

        // Call the LogIn method from the business layer and return the role type.
        return s_bl.Volunteer.LogIn(id, password);
    }

    /// <summary>
    /// Prompts the user for a volunteer ID and a filter option for the call list.
    /// The volunteer ID is stored in <paramref name="volunteerId"/> and the filter in <paramref name="callTypeFilter"/>.
    /// </summary>
    /// <param name="volunteerId">Output parameter for the volunteer's ID.</param>
    /// <param name="callTypeFilter">Output parameter for the optional call type filter.</param>
    private static void HelpGetCallList(out int volunteerId, out BO.CallType? callTypeFilter)
    {
        volunteerId = -1; // Default value for volunteer ID.
        callTypeFilter = null; // Default value for call type filter.

        // Prompt for the volunteer ID.
        Console.WriteLine("\nEnter Volunteer Id: ");
        while (!int.TryParse(Console.ReadLine(), out volunteerId)) // Ensure a valid integer ID is entered.
        {
            Console.WriteLine("Invalid Volunteer Id. Please enter a valid number:");
        }

        // Ask if the user wants to filter the list of calls.
        Console.WriteLine("Do you want to filter the list? (1 for Yes, 0 for No): ");
        if (int.TryParse(Console.ReadLine(), out int filterInput) && filterInput == 1)
        {
            // Menu for selecting a call type filter.
            Console.WriteLine("\nChoose a Call Type to filter by:");
            Console.WriteLine("1. Transportation");
            Console.WriteLine("2. Babysitting");
            Console.WriteLine("3. Shopping");
            Console.WriteLine("4. Food");
            Console.WriteLine("5. Cleaning");
            Console.WriteLine("6. None");
            Console.WriteLine("7. All");

            // Switch-case to handle different filter options.
            switch (Console.ReadLine())
            {
                case "1":
                    callTypeFilter = BO.CallType.Transportation; // Filter by Transportation.
                    break;
                case "2":
                    callTypeFilter = BO.CallType.Babysitting; // Filter by Babysitting.
                    break;
                case "3":
                    callTypeFilter = BO.CallType.Shopping; // Filter by Shopping.
                    break;
                case "4":
                    callTypeFilter = BO.CallType.food; // Filter by Food.
                    break;
                case "5":
                    callTypeFilter = BO.CallType.Cleaning; // Filter by Cleaning.
                    break;
                case "6":
                    callTypeFilter = BO.CallType.None; // No filter applied.
                    break;
                case "7":
                    callTypeFilter = null; // No filtering.
                    break;
                default:
                    Console.WriteLine("Invalid input. No filter applied."); // Handle invalid input.
                    break;
            }
        }
    }


    /// <summary>
    /// Prompts the user to choose whether to sort a closed call list, and if so, which field to sort by.
    /// </summary>
    /// <returns>A <see cref="BO.FieldsClosedCallInList"/> value representing the selected sorting field, or null if no sorting is applied.</returns>
    static BO.FieldsClosedCallInList? sortFilterClose()
    {
        BO.FieldsClosedCallInList? sortField = null; // Default value, no sorting applied.

        Console.WriteLine("Do you want to sort the list? (1 for Yes, 0 for No): ");
        if (int.TryParse(Console.ReadLine(), out int sortInput) && sortInput == 1) // Check if user wants to sort.
        {
            // Menu for sorting by FieldsClosedCallInList.
            Console.WriteLine("\nChoose a field to sort by:");
            Console.WriteLine("1. Id");
            Console.WriteLine("2. Call Type");
            Console.WriteLine("3. Address");
            Console.WriteLine("4. Opening Time");
            Console.WriteLine("5. Entry Time");
            Console.WriteLine("6. Actual End Time");
            Console.WriteLine("7. End Type");

            // Get user's choice for sorting field.
            switch (Console.ReadLine())
            {
                case "1":
                    sortField = BO.FieldsClosedCallInList.Id; // Sort by Id.
                    break;
                case "2":
                    sortField = BO.FieldsClosedCallInList.TheCallType; // Sort by Call Type.
                    break;
                case "3":
                    sortField = BO.FieldsClosedCallInList.Address; // Sort by Address.
                    break;
                case "4":
                    sortField = BO.FieldsClosedCallInList.OpeningTime; // Sort by Opening Time.
                    break;
                case "5":
                    sortField = BO.FieldsClosedCallInList.EntryTime; // Sort by Entry Time.
                    break;
                case "6":
                    sortField = BO.FieldsClosedCallInList.ActualEndTime; // Sort by Actual End Time.
                    break;
                case "7":
                    sortField = BO.FieldsClosedCallInList.TheEndType; // Sort by End Type.
                    break;
                default:
                    Console.WriteLine("Invalid input. No sorting applied."); // Invalid choice handling.
                    break;
            }
        }
        return sortField; // Return selected sorting field, or null if no sorting.
    }

    /// <summary>
    /// Prompts the user to choose whether to sort an open call list, and if so, which field to sort by.
    /// </summary>
    /// <returns>A <see cref="BO.FieldsOpenCallInList"/> value representing the selected sorting field, or null if no sorting is applied.</returns>
    static BO.FieldsOpenCallInList? sortFilterOpen()
    {
        BO.FieldsOpenCallInList? sortField = null; // Default value, no sorting applied.

        Console.WriteLine("Do you want to sort the list? (1 for Yes, 0 for No): ");
        if (int.TryParse(Console.ReadLine(), out int sortInput) && sortInput == 1) // Check if user wants to sort.
        {
            // Menu for sorting by FieldsOpenCallInList.
            Console.WriteLine("\nChoose a field to sort by:");
            Console.WriteLine("1. Id");
            Console.WriteLine("2. Call Type");
            Console.WriteLine("3. Verbal Description");
            Console.WriteLine("4. Address");
            Console.WriteLine("5. Opening Time");
            Console.WriteLine("6. Max Time To End");
            Console.WriteLine("7. Distance");

            // Get user's choice for sorting field.
            switch (Console.ReadLine())
            {
                case "1":
                    sortField = BO.FieldsOpenCallInList.Id; // Sort by Id.
                    break;
                case "2":
                    sortField = BO.FieldsOpenCallInList.TheCallType; // Sort by Call Type.
                    break;
                case "3":
                    sortField = BO.FieldsOpenCallInList.VerbalDescription; // Sort by Verbal Description.
                    break;
                case "4":
                    sortField = BO.FieldsOpenCallInList.Address; // Sort by Address.
                    break;
                case "5":
                    sortField = BO.FieldsOpenCallInList.OpeningTime; // Sort by Opening Time.
                    break;
                case "6":
                    sortField = BO.FieldsOpenCallInList.MaxTimeToEnd; // Sort by Max Time To End.
                    break;
                case "7":
                    sortField = BO.FieldsOpenCallInList.Distance; // Sort by Distance.
                    break;
                default:
                    Console.WriteLine("Invalid input. No sorting applied."); // Invalid choice handling.
                    break;
            }
        }

        return sortField; // Return selected sorting field, or null if no sorting.
    }


    /// <summary>
    /// Prompts the user for input to create a new volunteer and returns a BO.Volunteer object.
    /// </summary>
    /// <returns>A new BO.Volunteer object created from the user input.</returns>
    static BO.Volunteer HelpCreateVolunteer()
    {
        // Prompt user for volunteer information
        Console.Write("Enter Volunteer Id: ");
        if (!int.TryParse(Console.ReadLine(), out int VolIdCreate))
            Console.WriteLine("Invalid ID format.");

        Console.Write("Enter Volunteer Name: ");
        string name = Console.ReadLine();

        Console.Write("Enter Address: ([Street Name] [Building/House Number], [City], [Country])\n ");
        string address = Console.ReadLine();

        Console.Write("Enter Phone Number: ");
        string phoneNumber = Console.ReadLine();

        Console.Write("Enter Email: ");
        string email = Console.ReadLine();

        // Optional max distance input
        double? maxDistance = null;
        Console.Write("Enter max Distance to choose call:(optional) ");
        if (double.TryParse(Console.ReadLine(), out double tmp))
            maxDistance = tmp;

        // Create and return new volunteer object
        var newVolunteer = new BO.Volunteer
        {
            Id = VolIdCreate,
            Name = name,
            Address = address,
            PhoneNumber = phoneNumber,
            Email = email,
            MaxDistance = maxDistance,
            Role = BO.RoleType.volunteer,
            Active = true // Default to active
        };

        return newVolunteer;
    }

    /// <summary>
    /// Prompts the user to input updated volunteer details and updates the corresponding volunteer record.
    /// </summary>
    static void HelpUpdateVolunteer()
    {
        // Prompt user for volunteer ID to update
        Console.WriteLine("Enter volunteer ID");
        if (!int.TryParse(Console.ReadLine(), out int idToUpdate))
        {
            Console.WriteLine("Invalid ID");
            return;
        }

        // Validate user's identity before updating
        Console.WriteLine("Please enter your ID");
        if (!int.TryParse(Console.ReadLine(), out int idPerson))
        {
            Console.WriteLine("Invalid ID");
            return;
        }

        Console.WriteLine("Updating details for volunteer: ");

        // Prompt for updated volunteer information
        Console.WriteLine("Enter Name :");
        string fullName = Console.ReadLine();

        Console.WriteLine("Phone Number :");
        string phoneNumber = Console.ReadLine();

        Console.WriteLine("Email:");
        string email = Console.ReadLine();

        Console.WriteLine("Role (manager- 0, volunteer- 1):");
        if (!Enum.TryParse(Console.ReadLine(), out BO.RoleType roleUpdate))
        {
            roleUpdate = BO.RoleType.volunteer; // Default to volunteer role
        }

        Console.WriteLine("Active (true/false):");
        if (!bool.TryParse(Console.ReadLine(), out bool activeUP))
            activeUP = true; // Default to active

        Console.WriteLine("Password :");
        string passwordNew = Console.ReadLine();

        Console.WriteLine("Address: ([Street Name] [Building/House Number], [City], [Country])\n");
        string fullAddress = Console.ReadLine();

        // Optional max distance input
        double? maxDistance = null;
        Console.Write("Enter max Distance to choose call:(optional) ");
        if (double.TryParse(Console.ReadLine(), out double tmp))
            maxDistance = tmp;

        // Create volunteer object with updated details
        BO.Volunteer volunteerToUpdate = new BO.Volunteer
        {
            Id = idToUpdate,
            Name = fullName,
            PhoneNumber = phoneNumber,
            Email = email,
            TheDistanceType = BO.DistanceType.air, // Default to air distance
            Role = roleUpdate,
            Active = activeUP,
            Password = passwordNew,
            Address = fullAddress,
            Latitude = 0, // Default values for location
            Longitude = 0,
            MaxDistance = maxDistance,
            TotalHandled = 0, // Default values for tracking
            TotalCanceled = 0,
            TotalExpired = 0,
            IsProgress = null,
        };

        // Update the volunteer in the system
        s_bl.Volunteer.Update(idPerson, volunteerToUpdate);
    }

    /// <summary>
    /// Prompts the user to filter and sort the volunteer list, then calls the corresponding function to retrieve and display the list.
    /// </summary>
    static void ReadAllVolunteerHelp()
    {
        // Prompt for active status filter (optional)
        Console.WriteLine("Filter by active status? (Enter 'true' for Active, 'false' for Inactive, or press Enter to skip):");
        bool? activeFilter = null;
        if (bool.TryParse(Console.ReadLine(), out bool activeOption))
            activeFilter = activeOption;

        // Prompt for sorting the list based on specific fields
        Console.WriteLine("Sort by field? (Enter: 1 for Id, 2 for Name, 3 for TotalHandled, 4 for TotalCanceled, 5 for TotalExpired):");
        BO.FieldsVolunteerInList sortField = BO.FieldsVolunteerInList.Id; // Default sorting by Id
        if (int.TryParse(Console.ReadLine(), out int sortFieldOption))
        {
            sortField = sortFieldOption switch
            {
                1 => BO.FieldsVolunteerInList.Id,
                2 => BO.FieldsVolunteerInList.Name,
                3 => BO.FieldsVolunteerInList.TotalHandled,
                4 => BO.FieldsVolunteerInList.TotalCanceled,
                5 => BO.FieldsVolunteerInList.TotalExpired,
                _ => BO.FieldsVolunteerInList.Id // Default to Id if input is invalid
            };
        }

        // Call function to get volunteers based on filters and sort field
        var volList = s_bl.Volunteer.ReadAll(activeFilter, sortField);

        // Display the results
        foreach (var volunteer in volList)
            Console.WriteLine(volunteer);
    }

    /// <summary>
    /// Prompts the user to filter and sort the call list, then calls the corresponding function to retrieve and display the list.
    /// </summary>
    static void ReadAllCallHelp()
    {
        // Prompt for field filter (optional)
        Console.WriteLine("Filter by field? (Enter: 1 for Id, 2 for CallId, 3 for TheCallType, 4 for OpeningTime, 5 for TimeToEnd, 6 for LastVolunteer, 7 for CompletionTreatment, 8 for Status, 9 for TotalAssignments, or press Enter to skip):");
        BO.FieldsCallInList? filterField = null;
        object? filterValue = null;

        if (int.TryParse(Console.ReadLine(), out int filterFieldOption))
        {
            filterField = filterFieldOption switch
            {
                1 => BO.FieldsCallInList.Id,
                2 => BO.FieldsCallInList.CallId,
                3 => BO.FieldsCallInList.TheCallType,
                4 => BO.FieldsCallInList.OpeningTime,
                5 => BO.FieldsCallInList.TimeToEnd,
                6 => BO.FieldsCallInList.LastVolunteer,
                7 => BO.FieldsCallInList.CompletionTreatment,
                8 => BO.FieldsCallInList.status,
                9 => BO.FieldsCallInList.TotalAssignments,
                _ => null
            };

            if (filterField.HasValue)
            {
                // Prompt for the filter value
                Console.WriteLine("Enter filter value:");
                var inputValue = Console.ReadLine();

                // Set the corresponding filter value based on the selected field
                filterValue = filterField switch
                {
                    BO.FieldsCallInList.Id => int.TryParse(inputValue, out int id) ? id : null,
                    BO.FieldsCallInList.CallId => int.TryParse(inputValue, out int callId) ? callId : null,
                    BO.FieldsCallInList.TheCallType => Enum.TryParse(typeof(BO.CallType), inputValue, out var callType) ? callType : null,
                    BO.FieldsCallInList.OpeningTime => DateTime.TryParse(inputValue, out DateTime openingTime) ? openingTime : null,
                    BO.FieldsCallInList.TimeToEnd => TimeSpan.TryParse(inputValue, out TimeSpan timeToEnd) ? timeToEnd : null,
                    BO.FieldsCallInList.LastVolunteer => inputValue, // LastVolunteer is a string
                    BO.FieldsCallInList.CompletionTreatment => TimeSpan.TryParse(inputValue, out TimeSpan completionTreatment) ? completionTreatment : null,
                    BO.FieldsCallInList.status => Enum.TryParse(typeof(BO.Status), inputValue, out var status) ? status : null,
                    BO.FieldsCallInList.TotalAssignments => int.TryParse(inputValue, out int totalAssignments) ? totalAssignments : null,
                    _ => null
                };
            }
        }

        // Prompt for sorting the list based on specific fields
        Console.WriteLine("Sort by field? (Enter: 1 for Id, 2 for CallId, 3 for TheCallType, 4 for OpeningTime, 5 for TimeToEnd, 6 for LastVolunteer, 7 for CompletionTreatment, 8 for Status, 9 for TotalAssignments, or press Enter to skip):");
        BO.FieldsCallInList? sortField = null;

        if (int.TryParse(Console.ReadLine(), out int sortFieldOption))
        {
            sortField = sortFieldOption switch
            {
                1 => BO.FieldsCallInList.Id,
                2 => BO.FieldsCallInList.CallId,
                3 => BO.FieldsCallInList.TheCallType,
                4 => BO.FieldsCallInList.OpeningTime,
                5 => BO.FieldsCallInList.TimeToEnd,
                6 => BO.FieldsCallInList.LastVolunteer,
                7 => BO.FieldsCallInList.CompletionTreatment,
                8 => BO.FieldsCallInList.status,
                9 => BO.FieldsCallInList.TotalAssignments,
                _ => null
            };
        }

        // Call function to get calls based on filter and sort field
        var allCalls = s_bl.Call.ReadAll(filterField, filterValue, sortField);

        // Display the results
        foreach (var call in allCalls)
            Console.WriteLine(call);
    }
}

