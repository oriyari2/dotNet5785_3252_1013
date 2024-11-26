using Dal;
using DalApi;
using DO;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace DalTest
{
    /// <summary>
    /// Main program class for managing volunteers, calls, assignments, and configurations.
    /// Provides a menu-based interface to interact with the data access layers (DALs).
    /// </summary>
    internal class Program
    {
        static readonly IDal s_dal = new Dal.DalXml();

        // Enums for main menu and sub-menu options.
        private enum ChooseMain { exit, volunteer, call, assignment, initialization, print, config, reset };
        private enum SubMenu { exit, create, read, readAll, update, delete, deleteAll }
        private enum ConfigMenu
        {
            exit, plusMinute, plusHour, plusDay, plusYear, plusMonth, getClock,
            getRiskRange, setRiskRange, reset
        }

        /// <summary>
        /// Prompts the user to select a configuration menu option and validates the input.
        /// </summary>
        /// <returns>Selected <see cref="ConfigMenu"/> option.</returns>
        private static ConfigMenu configInputChoose()
        {
            ConfigMenu choose;
            Console.WriteLine("Please enter one of the following options:exit, plusMinute, plusHour," +
                "plusDay, plusMonth, plusYear,\ngetClock, getRiskRange, setRiskRange, reset ");
            while (!Enum.TryParse(Console.ReadLine(), out choose)) ;
            return choose;
        }

        /// <summary>
        /// Prompts the user to select a sub-menu option  for CRUD operations and validates the input.
        /// </summary>
        /// <returns>Selected <see cref="SubMenu"/> option.</returns>
        private static SubMenu inputChoose()
        {
            SubMenu choose;
            Console.WriteLine("Please enter one of the following options: exit, create, read," +
                " readAll, update, delete, deleteAll");
            while (!Enum.TryParse(Console.ReadLine(), out choose)) ;
            return choose;
        }

        /// <summary>
        /// Prompts the user to input a risk range in the format of a <see cref="TimeSpan"/> and sets it.
        /// </summary>
        private static void setRiskRange()
        {
            TimeSpan riskRange;
            Console.WriteLine("Please enter risk range (in format of dd.hh:mm:ss)");
            while (!TimeSpan.TryParse(Console.ReadLine(), out riskRange))
                Console.WriteLine("Invalid input. Please enter a valid risk range (in format of dd.hh:mm:ss):");
            s_dal!.Config.RiskRange = riskRange;
        }

        /// <summary>
        /// Creates a new <see cref="Volunteer"/> object by prompting the user for required and optional details.
        /// </summary>
        /// <returns>Newly created <see cref="Volunteer"/> object.</returns>
        private static Volunteer volunteerCreate()
        {
            Console.WriteLine("Enter ID (integer):");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
                Console.WriteLine("Invalid input. Please enter a valid integer ID:");

            Console.WriteLine("Enter Name:");
            string name = Console.ReadLine() ?? "";

            Console.WriteLine("Enter Phone Number:");
            string phoneNumber = Console.ReadLine() ?? "";

            Console.WriteLine("Enter Email:");
            string email = Console.ReadLine() ?? "";

            Console.WriteLine("Enter Password (optional):");
            string? password = Console.ReadLine();

            Console.WriteLine("Enter Address (optional):");
            string? address = Console.ReadLine();

            Console.WriteLine("Enter Latitude (optional, decimal):");
            double? latitude = double.TryParse(Console.ReadLine(), out double lat) ? lat : null;

            Console.WriteLine("Enter Longitude (optional, decimal):");
            double? longitude = double.TryParse(Console.ReadLine(), out double lon) ? lon : null;

            Console.WriteLine("Enter Role Type (manager, volunteer):");
            RoleType role;
            while (!Enum.TryParse(Console.ReadLine(), out role))
                Console.WriteLine("Invalid role type. Please enter a valid role type:");

            Console.WriteLine("Is the Volunteer Active? (true/false):");
            bool active;
            while (!bool.TryParse(Console.ReadLine(), out active))
                Console.WriteLine("Invalid input. Please enter true or false:");

            Console.WriteLine("Enter Max Distance (optional, decimal):");
            double? maxDistance = double.TryParse(Console.ReadLine(), out double tempMaxDistance) ? tempMaxDistance : null;

            Console.WriteLine("Enter Distance Type (air, walking, driving):");
            DistanceType theDistanceType;
            while (!Enum.TryParse(Console.ReadLine(), out theDistanceType))
                Console.WriteLine("Invalid distance type. Please enter a valid distance type:");

            return new Volunteer(id, name, phoneNumber, email, password, address, latitude, longitude, role, active, maxDistance, theDistanceType);
        }

        /// <summary>
        /// Reads a volunteer's details by their ID and displays the information.
        /// </summary>
        /// <returns>The ID of the volunteer, if found.</returns>
        private static int volunteerRead()
        {
            Console.WriteLine("Enter volunteer ID:");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
                Console.WriteLine("Invalid input. Please enter a valid ID:");

            Volunteer? volunteer = s_dal!.Volunteer.Read(id);

            if (volunteer != null)
                Console.WriteLine(volunteer);
            else
                Console.WriteLine("Volunteer not found.");
            return id;
        }

        /// <summary>
        /// Reads and displays details of all volunteers.
        /// </summary>
        private static void volunteerReadAll()
        {
            var listVolunteer = s_dal!.Volunteer.ReadAll();
            foreach (Volunteer vol in listVolunteer)
                Console.WriteLine(vol);
        }

        /// <summary>
        /// Deletes a volunteer record based on their ID.
        /// </summary>
        private static void volunteerDelete()
        {
            Console.WriteLine("Enter volunteer ID:");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
                Console.WriteLine("Invalid input. Please enter a valid ID:");
            s_dal!.Volunteer?.Delete(id);
        }

        /// <summary>
        /// Updates an existing volunteer's details after fetching their current record by ID.
        /// </summary>
        private static void volunteerUpdate()
        {
            int id = volunteerRead();
            if (id != 0)
                s_dal!.Volunteer?.Update(volunteerCreate());
        }

        /// <summary>
        /// Creates a new <see cref="Call"/> object by prompting the user for details.
        /// </summary>
        /// <param name="id">Optional call ID for updating an existing record.</param>
        /// <returns>Newly created <see cref="Call"/> object.</returns>
        private static Call callCreate(int id = 0)
        {
            Console.WriteLine("Enter Call type (Transportation, Babysitting, Shopping, food, Cleaning):");
            CallType callType;
            while (!CallType.TryParse(Console.ReadLine(), out callType))
                Console.WriteLine("Invalid input. Please enter a valid call type:");

            Console.WriteLine("Enter description of the call:");
            string VerbalDescription = Console.ReadLine() ?? "";

            Console.WriteLine("Enter Address:");
            string Address = Console.ReadLine() ?? "";

            Console.WriteLine("Enter Latitude (decimal):");
            double latitude;
            while (!double.TryParse(Console.ReadLine(), out latitude))
                Console.WriteLine("Invalid input. Please enter a valid latitude:");

            Console.WriteLine("Enter Longitude (decimal):");
            double longitude;
            while (!double.TryParse(Console.ReadLine(), out longitude))
                Console.WriteLine("Invalid input. Please enter a valid longitude:");

            return new Call(id, callType, VerbalDescription, Address, latitude, longitude, DateTime.Now, null);
        }


        /// <summary>
        /// Reads a call ID from the user and retrieves the corresponding call.
        /// </summary>
        /// <returns>The ID of the call entered by the user.</returns>
        private static int callRead()
        {
            Console.WriteLine("Enter call ID:");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
                Console.WriteLine("Invalid input. Please enter a valid ID:");

            Call? call = s_dal!.Call.Read(id);

            if (call != null)
                Console.WriteLine(call);
            else
                Console.WriteLine("Call not found.");
            return id;
        }

        /// <summary>
        /// Reads and displays all calls from the database.
        /// </summary>
        private static void callReadAll()
        {
            var listCall = s_dal!.Call.ReadAll(); // Call the function that retrieves all records

            foreach (Call call in listCall) // Loop through each call in the list
                Console.WriteLine(call); // Print the details of each call to the screen
        }

        /// <summary>
        /// Deletes a call from the database based on user input.
        /// </summary>
        private static void callDelete()
        {
            Console.WriteLine("Enter call ID:"); // Prompt the user to enter a call ID
            int id; // Declare an integer variable to store the call ID
            while (!int.TryParse(Console.ReadLine(), out id)) // Check if the input can be parsed into an integer
                Console.WriteLine("Invalid input. Please enter a valid ID:"); // Inform the user of invalid input

            s_dal!.Call?.Delete(id); // Call the Delete method on the data access layer (DAL) with the parsed ID
        }

        /// <summary>
        /// Updates an existing call's details.
        /// </summary>
        private static void callUpdate()
        {
            Console.WriteLine("Enter Call id (integer):");
            int callId;

            while (!int.TryParse(Console.ReadLine(), out callId))
                Console.WriteLine("Invalid input. Please enter a valid call id:");

            Call call = callCreate(callId); // Create or modify a call object.
            Console.WriteLine(s_dal!.Call?.Read(call.Id)); // Display current details.
            s_dal!.Call?.Update(call); // Update details in the database.
        }

        /// <summary>
        /// Updates an existing assignment's details.
        /// </summary>
        private static void assignmentUpdate()
        {
            Console.WriteLine("Enter Assignment id (integer):");
            int assignmentId;

            while (!int.TryParse(Console.ReadLine(), out assignmentId))
                Console.WriteLine("Invalid input. Please enter a valid call id:");

            Assignment assignment = assignmentCreate(assignmentId); // Create or modify a call object.
            Console.WriteLine(s_dal!.Assignment?.Read(assignment.Id)); // Display current details.
            s_dal!.Assignment?.Update(assignment); // Update details in the database.
        }

        /// <summary>
        /// Creates a new assignment with user-provided data.
        /// </summary>
        /// <param name="id">Optional assignment ID (default is 0).</param>
        /// <returns>A new Assignment object.</returns>
        private static Assignment assignmentCreate(int id = 0)
        {
            Console.WriteLine("Enter Call ID (integer):");
            int CallId;
            while (!int.TryParse(Console.ReadLine(), out CallId))
                Console.WriteLine("Invalid input. Please enter a valid integer ID:");

            Console.WriteLine("Enter Volunteer ID (integer):");
            int VolunteerId;
            while (!int.TryParse(Console.ReadLine(), out VolunteerId))
                Console.WriteLine("Invalid input. Please enter a valid integer ID:");

            Assignment assignment = new Assignment(id, CallId, VolunteerId, DateTime.Now, null, null);
            return assignment;
        }

        /// <summary>
        /// Reads and displays a specific assignment based on user input.
        /// </summary>
        private static void assignmentRead()
        {
            Console.WriteLine("Enter assignment ID:");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
                Console.WriteLine("Invalid input. Please enter a valid ID:");

            Assignment? assignment = s_dal!.Assignment?.Read(id);

            if (assignment != null)
                Console.WriteLine(assignment);
            else
                Console.WriteLine("Assignment not found.");
        }

        /// <summary>
        /// Reads and displays all assignments from the database.
        /// </summary>
        private static void assignmentReadAll()
        {
            var listAssignment = s_dal!.Assignment.ReadAll();

            foreach (Assignment assignment in listAssignment)
                Console.WriteLine(assignment);
        }

        /// <summary>
        /// Deletes a specific assignment based on user input.
        /// </summary>
        private static void assignmentDelete()
        {
            Console.WriteLine("Enter assignment ID:");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
                Console.WriteLine("Invalid input. Please enter a valid ID:");

            s_dal!.Assignment?.Delete(id);
        }

        /// <summary>
        /// Main entry point of the application, provides a menu for various operations.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>

        static void Main(string[] args)
        {
            ChooseMain choose; // Variable to store the main menu choice.
            do
            {
                Console.WriteLine("Please enter one of the following options:exit, volunteer, call," +
                    "assignment, initialization, print, config, reset"); // Prompt the user for an input.

                while (!Enum.TryParse(Console.ReadLine(), out choose)) ; // Validate if the input matches the enum values.

                try
                {
                    switch (choose) // Main menu logic.
                    {
                        case ChooseMain.exit: // If 'exit', break out of the loop.
                            break;
                        case ChooseMain.volunteer: // Volunteer management submenu.
                            {
                                SubMenu chooseNew = inputChoose(); // Get user choice for volunteer actions.
                                while (chooseNew != SubMenu.exit) // Repeat until 'exit' is chosen.
                                {
                                    switch (chooseNew) // Handle volunteer-related actions.
                                    {
                                        case SubMenu.exit: // Exit the volunteer submenu.
                                            break;
                                        case SubMenu.create: // Create a new volunteer.
                                            {
                                                Volunteer vol = volunteerCreate(); // Call method to create a volunteer.
                                                s_dal!.Volunteer?.Create(vol); // Save volunteer to the database.
                                                break;
                                            }
                                        case SubMenu.read: // Read a volunteer's details.
                                            {
                                                volunteerRead(); // Display volunteer details.
                                                break;
                                            }
                                        case SubMenu.readAll: // Read all volunteers.
                                            {
                                                volunteerReadAll(); // Display all volunteer details.
                                                break;
                                            }
                                        case SubMenu.update: // Update a volunteer's details.
                                            {
                                                Volunteer vol = volunteerCreate(); // Create or modify a volunteer object.
                                                Console.WriteLine(s_dal!.Volunteer?.Read(vol.Id)); // Display current details.
                                                s_dal!.Volunteer?.Update(vol); // Update details in the database.
                                                break;
                                            }
                                        case SubMenu.delete: // Delete a volunteer.
                                            {
                                                volunteerDelete(); // Call the delete method.
                                                break;
                                            }
                                        case SubMenu.deleteAll: // Delete all volunteers.
                                            {
                                                s_dal!.Volunteer?.DeleteAll(); // Clear all data from the database.
                                                break;
                                            }
                                        default: // Default case if no valid option is chosen.
                                            break;
                                    }
                                    chooseNew = inputChoose(); // Prompt for the next volunteer action.
                                }
                                break;
                            }
                        case ChooseMain.call: // Call management submenu.
                            {
                                SubMenu chooseNew = inputChoose(); // Get user choice for call actions.
                                while (chooseNew != SubMenu.exit) // Repeat until 'exit' is chosen.
                                {
                                    switch (chooseNew) // Handle call-related actions.
                                    {
                                        case SubMenu.exit: // Exit the call submenu.
                                            break;
                                        case SubMenu.create: // Create a new call.
                                            {
                                                Call newCall = callCreate(0); // Call method to create a call.
                                                s_dal!.Call?.Create(newCall); // Save call to the database.
                                                break;
                                            }
                                        case SubMenu.read: // Read a call's details.
                                            {
                                                callRead(); // Display call details.
                                                break;
                                            }
                                        case SubMenu.readAll: // Read all calls.
                                            {
                                                callReadAll(); // Display all call details.
                                                break;
                                            }
                                        case SubMenu.update: // Update a call's details.
                                            {
                                                callUpdate();
                                                break;
                                            }
                                        case SubMenu.delete: // Delete a call.
                                            {
                                                callDelete(); // Call the delete method.
                                                break;
                                            }
                                        case SubMenu.deleteAll: // Delete all calls.
                                            {
                                                s_dal!.Call?.DeleteAll(); // Clear all data from the database.
                                                break;
                                            }
                                        default: // Default case if no valid option is chosen.
                                            break;
                                    }
                                    chooseNew = inputChoose(); // Prompt for the next call action.
                                }
                                break;
                            }
                        case ChooseMain.assignment: // Assignment management submenu.
                            {
                                SubMenu chooseNew = inputChoose(); // Get user choice for assignment actions.
                                while (chooseNew != SubMenu.exit) // Repeat until 'exit' is chosen.
                                {
                                    switch (chooseNew) // Handle assignment-related actions.
                                    {
                                        case SubMenu.exit: // Exit the assignment submenu.
                                            break;
                                        case SubMenu.create: // Create a new assignment.
                                            {
                                                Assignment assignment = assignmentCreate(0); // Create an assignment object.
                                                s_dal!.Assignment?.Create(assignment); // Save assignment to the database.
                                                break;
                                            }
                                        case SubMenu.read: // Read an assignment's details.
                                            {
                                                assignmentRead(); // Display assignment details.
                                                break;
                                            }
                                        case SubMenu.readAll: // Read all assignments.
                                            {
                                                assignmentReadAll(); // Display all assignment details.
                                                break;
                                            }
                                        case SubMenu.update: // Update an assignment's details.
                                            {
                                                assignmentUpdate();
                                                break;
                                            }
                                        case SubMenu.delete: // Delete an assignment.
                                            {
                                                assignmentDelete(); // Call the delete method.
                                                break;
                                            }
                                        case SubMenu.deleteAll: // Delete all assignments.
                                            {
                                                s_dal!.Assignment?.DeleteAll(); // Clear all data from the database.
                                                break;
                                            }
                                        default: // Default case if no valid option is chosen.
                                            break;
                                    }
                                    chooseNew = inputChoose(); // Prompt for the next assignment action.
                                }
                                break;
                            }
                        case ChooseMain.initialization: // Initialize the system.
                            {
                                Initialization.Do(s_dal);
                                break;
                            }
                        case ChooseMain.print: // Print all data.
                            {
                                volunteerReadAll(); // Print all volunteers.
                                callReadAll(); // Print all calls.
                                assignmentReadAll(); // Print all assignments.
                                break;
                            }
                        case ChooseMain.config: // Configuration submenu.
                            {
                                ConfigMenu chooseNew = configInputChoose(); // Get user choice for configuration actions.
                                while (chooseNew != ConfigMenu.exit) // Repeat until 'exit' is chosen.
                                {
                                    switch (chooseNew) // Handle configuration-related actions.
                                    {
                                        case ConfigMenu.exit: // Exit the configuration submenu.
                                            break;
                                        case ConfigMenu.plusMinute: // Increment the clock by one minute.
                                            {
                                                s_dal!.Config.Clock = s_dal!.Config.Clock.AddMinutes(1);
                                                break;
                                            }
                                        case ConfigMenu.plusHour: // Increment the clock by one hour.
                                            {
                                                s_dal!.Config.Clock = s_dal!.Config.Clock.AddHours(1);
                                                break;
                                            }
                                        case ConfigMenu.plusDay: // Increment the clock by one day.
                                            {
                                                s_dal!.Config.Clock = s_dal!.Config.Clock.AddDays(1);
                                                break;
                                            }
                                        case ConfigMenu.plusMonth: // Increment the clock by one month.
                                            {
                                                s_dal!.Config.Clock = s_dal!.Config.Clock.AddMonths(1);
                                                break;
                                            }
                                        case ConfigMenu.plusYear: // Increment the clock by one year.
                                            {
                                                s_dal!.Config.Clock = s_dal!.Config.Clock.AddYears(1);
                                                break;
                                            }
                                        case ConfigMenu.getClock: // Display the current clock time.
                                            {
                                                Console.WriteLine(s_dal!.Config?.Clock);
                                                break;
                                            }
                                        case ConfigMenu.getRiskRange: // Display the current risk range.
                                            {
                                                Console.WriteLine(s_dal!.Config?.RiskRange);
                                                break;
                                            }
                                        case ConfigMenu.setRiskRange: // Modify the risk range.
                                            {
                                                setRiskRange();
                                                break;
                                            }
                                        case ConfigMenu.reset: // Reset configuration to default values.
                                            {
                                                s_dal!.Config?.Reset();
                                                break;
                                            }
                                        default: // Default case if no valid option is chosen.
                                            break;
                                    }
                                    chooseNew = configInputChoose(); // Prompt for the next configuration action.
                                }
                                break;
                            }
                        case ChooseMain.reset: // Reset all data.
                            {
                                s_dal.ResetDB();
                                break;
                            }
                        default: // Default case if no valid option is chosen.
                            break;
                    }
                }
                catch (Exception ex) // Handle any exceptions.
                {
                    Console.WriteLine(ex); // Display error details.
                }
            } while (choose != ChooseMain.exit); // Repeat until 'exit' is chosen
        }
    }
}
