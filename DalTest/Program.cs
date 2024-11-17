using Dal;
using DalApi;
using DO;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace DalTest
{
    internal class Program
    {
        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); // Initialize the IVolunteer data access layer (DAL) with a VolunteerImplementation instance.
        private static ICall? s_dalCall = new CallImplementation(); // Initialize the ICall data access layer (DAL) with a CallImplementation instance.
        private static IAssignment? s_dalAssignment = new AssignmentImplementation(); // Initialize the IAssignment data access layer (DAL) with an AssignmentImplementation instance.
        private static IConfig? s_dalConfig = new ConfigImplementation(); // Initialize the IConfig data access layer (DAL) with a ConfigImplementation instance.
        private enum ChooseMain { exit, volunteer, call, assignment, initialization, print, config, reset }; // Enum to define the main menu options.
        private enum SubMenu { exit, create, read, readAll, update, delete, deleteAll } // Enum to define the sub-menu options for CRUD operations.
        private enum ConfigMenu // Enum to define configuration menu options.
        {
            exit, plusMinute, plusHour, plusDay, plusYear, plusMonth, getClock,
            getRiskRange, setRiskRange, reset
        }

        private static ConfigMenu configInputChoose() // Method to handle user input for configuration menu options.
        {
            ConfigMenu choose;
            Console.WriteLine("Please enter one of the following options:exit, plusMinute, plusHour," + // Prompt user for input in configuration menu.
                "plusDay, plusMonth, plusYear,\ngetClock, getRiskRange, setRiskRange, reset ");
            while (!Enum.TryParse(Console.ReadLine(), out choose)) ; // Validate input and parse it to the corresponding ConfigMenu enum.
            return choose; // Return the valid menu choice.
        }

        /*
         The function will return what action the user wants to perform on the entity.
        */
        private static SubMenu inputChoose() // Method to handle user input for the sub-menu options.
        {
            SubMenu choose;
            Console.WriteLine("Please enter one of the following options:exit, create, read," + // Prompt user for input in sub-menu for CRUD actions.
                " readAll, update, delete, deleteAll");
            while (!Enum.TryParse(Console.ReadLine(), out choose)) ; // Validate input and parse it to the corresponding SubMenu enum.
            return choose; // Return the valid sub-menu choice.
        }

        private static void setRiskRange() // Method to set the risk range configuration.
        {
            TimeSpan riskRange; // Variable to store the parsed risk range.
            Console.WriteLine("please enter risk range (in format of dd.hh:mm:ss)"); // Prompt user for input in TimeSpan format.
            while (!TimeSpan.TryParse(Console.ReadLine(), out riskRange)) // Validate input and parse it as a TimeSpan.
                Console.WriteLine("Invalid input. Please enter a valid risk range (in format of dd.hh:mm:ss):"); // Prompt user again if input is invalid.
            s_dalConfig.RiskRange = riskRange; // Set the valid risk range to the configuration object.
        }

        //volunteer function
        /*
         creates new volunteer
        */  
        private static Volunteer volunteerCreate()
        {
            // Prompt for ID input
            Console.WriteLine("Enter ID (integer):");
            int id;
            // Read user input and try to convert it to an integer. If invalid, ask again.
            while (!int.TryParse(Console.ReadLine(), out id))
                Console.WriteLine("Invalid input. Please enter a valid integer ID:");

            // Prompt for Name input
            Console.WriteLine("Enter Name:");
            string name = Console.ReadLine() ?? "";

            // Prompt for Phone Number input
            Console.WriteLine("Enter Phone Number:");
            string phoneNumber = Console.ReadLine() ?? "";

            // Prompt for Email input
            Console.WriteLine("Enter Email:");
            string email = Console.ReadLine() ?? "";

            // Prompt for optional Password input
            Console.WriteLine("Enter Password (optional):");
            string? password = Console.ReadLine();

            // Prompt for optional Address input
            Console.WriteLine("Enter Address (optional):");
            string? address = Console.ReadLine();

            // Prompt for optional Latitude input, and try parsing it as a double
            Console.WriteLine("Enter Latitude (optional, decimal):");
            double? latitude = double.TryParse(Console.ReadLine(), out double lat) ? lat : null;

            // Prompt for optional Longitude input, and try parsing it as a double
            Console.WriteLine("Enter Longitude (optional, decimal):");
            double? longitude = double.TryParse(Console.ReadLine(), out double lon) ? lon : null;

            // Prompt for Role Type input and validate it by trying to parse it as an Enum
            Console.WriteLine("Enter Role Type (manager, volunteer):");
            RoleType role;
            while (!Enum.TryParse(Console.ReadLine(), out role))
                Console.WriteLine("Invalid role type. Please enter a valid role type:");

            // Prompt for Active status (true/false) input and validate it
            Console.WriteLine("Is the Volunteer Active? (true/false):");
            bool active;
            while (!bool.TryParse(Console.ReadLine(), out active))
                Console.WriteLine("Invalid input. Please enter true or false:");

            // Prompt for optional Max Distance input, and try parsing it as a double
            Console.WriteLine("Enter Max Distance (optional, decimal):");
            double? maxDistance = double.TryParse(Console.ReadLine(), out double tempMaxDistance) ? tempMaxDistance : null;

            // Prompt for Distance Type input and validate it by trying to parse it as an Enum
            Console.WriteLine("Enter Distance Type (air, walking, driving):");
            DistanceType theDistanceType;
            while (!Enum.TryParse(Console.ReadLine(), out theDistanceType))
                Console.WriteLine("Invalid distance type. Please enter a valid distance type:");

            // Create and return a new Volunteer object with all the gathered data
            Volunteer vol = new Volunteer(id, name, phoneNumber, email, password, address, latitude, longitude, role, active, maxDistance, theDistanceType);
            return vol;

        }

        /*
        A method that Chat GPT wrote. We sent him to implement the read method with the implementation
        of read in the IVolunteer class.
        */
        private static int volunteerRead()
        {
            Console.WriteLine("Enter volunteer ID:");  // Prompt the user to enter the volunteer ID
            int id;  // Declare a variable to hold the volunteer ID
            while (!int.TryParse(Console.ReadLine(), out id))  // Check if the input is a valid integer
                Console.WriteLine("Invalid input. Please enter a valid ID:");  // Prompt again if the input is invalid

            Volunteer? volunteer = s_dalVolunteer?.Read(id);  // Try to read the volunteer with the provided ID

            if (volunteer != null)  // If a volunteer is found, display their information
                Console.WriteLine(volunteer);
            else  // If no volunteer is found, inform the user
                Console.WriteLine("Volunteer not found.");
            return id;
        }

        private static void volunteerReadAll()
        {
            var listVolunteer = s_dalVolunteer.ReadAll();  // Get all volunteers from the data source
            foreach (Volunteer vol in listVolunteer)  // Iterate through each volunteer in the list
                Console.WriteLine(vol);  // Print each volunteer's information
        }


        private static void volunteerDelete()
        {
            Console.WriteLine("Enter volunteer ID:");  // Prompt the user to enter the volunteer ID
            int id;  // Declare an integer variable to store the volunteer ID
            while (!int.TryParse(Console.ReadLine(), out id))  // Check if the input can be parsed into an integer
                Console.WriteLine("Invalid input. Please enter a valid ID:");  // Prompt the user to enter a valid ID if parsing fails
            s_dalVolunteer?.Delete(id);  // Call the Delete method on the volunteer data access layer to remove the volunteer with the specified ID

        }

        private static void volunteerUpdate()
        {
            int id = volunteerRead();
            if (id != 0)
                s_dalVolunteer?.Update(volunteerCreate());
        }

        //call function
        private static Call callCreate(int id=0)
        {
            // Prompting the user to enter the call type
            Console.WriteLine("Enter Call type (Transportation, Babysitting, Shopping, food, Cleaning):");
            CallType callType; // Declaring a variable to hold the call type
            while (!CallType.TryParse(Console.ReadLine(), out callType)) // Validating the input to ensure it's a valid CallType enumeration
                Console.WriteLine("Invalid input. Please enter a valid call type:"); // Informing the user about the invalid input
            

            // Prompting for the description of the call
            Console.WriteLine("Enter description of the call:");
            string VerbalDescription = Console.ReadLine() ?? ""; // Storing the call description, using an empty string if null

            // Prompting for the address
            Console.WriteLine("Enter Address:");
            string Address = Console.ReadLine() ?? ""; // Storing the address, using an empty string if null

            // Prompting for the latitude (optional)
            Console.WriteLine("Enter Latitude (decimal):");
            double latitude; // Declaring a variable for the latitude
            while (!double.TryParse(Console.ReadLine(), out latitude))// Validating if the latitude is a valid decimal number
                Console.WriteLine("Invalid input. Please enter a valid latitude:"); // Asking for valid latitude input
            

            // Prompting for the longitude (optional)
            Console.WriteLine("Enter Longitude (decimal):");
            double longitude; // Declaring a variable for the longitude
            while (!double.TryParse(Console.ReadLine(), out longitude))// Validating if the longitude is a valid decimal number
                Console.WriteLine("Invalid input. Please enter a valid longitude:"); // Asking for valid longitude input

            // Creating the call object with the provided details
            Call call = new Call(id, callType, VerbalDescription, Address, latitude, longitude, DateTime.Now, null);

            // Returning the created call object
            return call;

        }

        private static int callRead()
        {
            Console.WriteLine("Enter call ID:");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
                Console.WriteLine("Invalid input. Please enter a valid ID:");
            
            Call? call = s_dalCall?.Read(id);

            if (call != null)
                Console.WriteLine(call);
            else
                Console.WriteLine("Call not found.");
            return id;
        }

        private static void callReadAll()
        {
            var listCall = s_dalCall.ReadAll(); // Call the function that retrieves all records

            foreach (Call call in listCall) // Loop through each call in the list
                Console.WriteLine(call); // Print the details of each call to the screen
        }

        private static void callDelete()
        {
            Console.WriteLine("Enter call ID:"); // Prompt the user to enter a call ID
            int id; // Declare an integer variable to store the call ID
            while (!int.TryParse(Console.ReadLine(), out id)) // Check if the input can be parsed into an integer
                Console.WriteLine("Invalid input. Please enter a valid ID:"); // Inform the user of invalid input
            
            s_dalCall?.Delete(id); // Call the Delete method on the data access layer (DAL) with the parsed ID
        }

        private static void callUpdate()
        {
            // Prompting the user to enter the call id need to update
            Console.WriteLine("Enter Call id (integer):");
            int callId; // Declaring a variable to hold the call id
                                                
            while (!int.TryParse(Console.ReadLine(), out callId)) // Validating the input to ensure it's a valid CallType enumeration
                Console.WriteLine("Invalid input. Please enter a valid call id:"); // Informing the user about the invalid input
            Call call = callCreate(callId); // Create or modify a call object.
            Console.WriteLine(s_dalCall?.Read(call.Id)); // Display current details.
            s_dalCall?.Update(call); // Update details in the database.

        }
        private static void assignmentUpdate()
        {
            // Prompting the user to enter the Assignment id need to update
            Console.WriteLine("Enter Assignment id (integer):");
            int assignmentId; // Declaring a variable to hold the call id
            while (!int.TryParse(Console.ReadLine(), out assignmentId))// Validating the input to ensure it's a valid assignmentId 
                Console.WriteLine("Invalid input. Please enter a valid call id:"); // Informing the user about the invalid input
            
            Assignment assignment = assignmentCreate(assignmentId); // Create or modify a call object.
            Console.WriteLine(s_dalAssignment?.Read(assignment.Id)); // Display current details.
            s_dalAssignment?.Update(assignment); // Update details in the database.

        }

        //assignment function

        private static Assignment assignmentCreate(int id=0)
        {
            Console.WriteLine("Enter Call ID (integer):");  // Prompt the user to enter a Call ID as an integer
            int CallId;  // Declare a variable to store the Call ID
            while (!int.TryParse(Console.ReadLine(), out CallId))  // Try to convert the input to an integer
                Console.WriteLine("Invalid input. Please enter a valid integer ID:");  // If conversion fails, ask for a valid input

            Console.WriteLine("Enter Volunteer ID (integer):");  // Prompt the user to enter a Volunteer ID as an integer
            int VolunteerId;  // Declare a variable to store the Volunteer ID
            while (!int.TryParse(Console.ReadLine(), out VolunteerId))  // Try to convert the input to an integer
                Console.WriteLine("Invalid input. Please enter a valid integer ID:");  // If conversion fails, ask for a valid input
           
            Assignment assignment = new Assignment(id, CallId, VolunteerId, DateTime.Now, null, null);  // Create a new Assignment object with the provided data
            return assignment;  // Return the created assignment object

        }

        private static void assignmentRead()
        {
            Console.WriteLine("Enter assignment ID:"); // Prompt the user to input an assignment ID.
            int id; // Declare a variable to store the assignment ID.
            while (!int.TryParse(Console.ReadLine(), out id)) // Validate the input and repeat until a valid integer is entered.
                Console.WriteLine("Invalid input. Please enter a valid ID:"); // Notify the user of invalid input.

            Assignment? assignment = s_dalAssignment?.Read(id); // Attempt to fetch the assignment using the given ID.

            if (assignment != null) // Check if the assignment exists.
                Console.WriteLine(assignment); // Print the assignment details if found.
            else
                Console.WriteLine("Assignment not found."); // Inform the user if the assignment does not exist.

        }

        private static void assignmentReadAll()
        {
            var listAssignment = s_dalAssignment.ReadAll(); // Retrieve a list of all assignments from the data access layer.

            foreach (Assignment assignment in listAssignment) // Loop through each assignment in the list.
                Console.WriteLine(assignment); // Print the details of each assignment.
        }

        private static void assignmentDelete()
        {
            Console.WriteLine("Enter assignment ID:"); // Prompt the user to input an assignment ID for deletion.
            int id; // Declare a variable to store the assignment ID.
            while (!int.TryParse(Console.ReadLine(), out id)) // Validate the input and repeat until a valid integer is entered.
                Console.WriteLine("Invalid input. Please enter a valid ID:"); // Notify the user of invalid input.
            s_dalAssignment?.Delete(id); // Attempt to delete the assignment with the specified ID from the data access layer.

        }

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
                                                s_dalVolunteer?.Create(vol); // Save volunteer to the database.
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
                                                Console.WriteLine(s_dalVolunteer?.Read(vol.Id)); // Display current details.
                                                s_dalVolunteer?.Update(vol); // Update details in the database.
                                                break;
                                            }
                                        case SubMenu.delete: // Delete a volunteer.
                                            {
                                                volunteerDelete(); // Call the delete method.
                                                break;
                                            }
                                        case SubMenu.deleteAll: // Delete all volunteers.
                                            {
                                                s_dalVolunteer?.DeleteAll(); // Clear all data from the database.
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
                                                s_dalCall?.Create(newCall); // Save call to the database.
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
                                                s_dalCall?.DeleteAll(); // Clear all data from the database.
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
                                                s_dalAssignment?.Create(assignment); // Save assignment to the database.
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
                                                s_dalAssignment?.DeleteAll(); // Clear all data from the database.
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
                                Initialization.Do(s_dalCall, s_dalAssignment, s_dalVolunteer, s_dalConfig); // Perform initialization tasks.
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
                                                s_dalConfig.Clock = s_dalConfig.Clock.AddMinutes(1);
                                                break;
                                            }
                                        case ConfigMenu.plusHour: // Increment the clock by one hour.
                                            {
                                                s_dalConfig.Clock = s_dalConfig.Clock.AddHours(1);
                                                break;
                                            }
                                        case ConfigMenu.plusDay: // Increment the clock by one day.
                                            {
                                                s_dalConfig.Clock = s_dalConfig.Clock.AddDays(1);
                                                break;
                                            }
                                        case ConfigMenu.plusMonth: // Increment the clock by one month.
                                            {
                                                s_dalConfig.Clock = s_dalConfig.Clock.AddMonths(1);
                                                break;
                                            }
                                        case ConfigMenu.plusYear: // Increment the clock by one year.
                                            {
                                                s_dalConfig.Clock = s_dalConfig.Clock.AddYears(1);
                                                break;
                                            }
                                        case ConfigMenu.getClock: // Display the current clock time.
                                            {
                                                Console.WriteLine(s_dalConfig?.Clock);
                                                break;
                                            }
                                        case ConfigMenu.getRiskRange: // Display the current risk range.
                                            {
                                                Console.WriteLine(s_dalConfig?.RiskRange);
                                                break;
                                            }
                                        case ConfigMenu.setRiskRange: // Modify the risk range.
                                            {
                                                setRiskRange();
                                                break;
                                            }
                                        case ConfigMenu.reset: // Reset configuration to default values.
                                            {
                                                s_dalConfig?.Reset();
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
                                s_dalVolunteer?.DeleteAll(); // Clear volunteer data.
                                s_dalCall?.DeleteAll(); // Clear call data.
                                s_dalAssignment?.DeleteAll(); // Clear assignment data.
                                s_dalConfig?.Reset(); // Reset configuration.
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
