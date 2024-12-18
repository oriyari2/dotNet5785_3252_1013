using BO;
using DO;

namespace BlTest;

internal class Program
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public enum MainMenuOptions { Exit, Admin, Call, Volunteer }

    public enum AdminMenuOptions { Exit, GetClock, AdvanceClock, GetRiskRange, SetRiskRange, Reset, Initialize }

    public enum CallMenuOptions { Exit, CallsAmount, ReadAll, Read, Update, Delete, Create, GetClosedCallList, GetOpenCallList, EndTreatment, CancelTreatment, ChooseCallToTreat }

    public enum VolunteerMenuOptions { Exit, LogIn, ReadAll, Read, Update, Delete, Create }



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

            if (Enum.TryParse(Console.ReadLine(), out MainMenuOptions choice))
            {
                switch (choice)
                {
                    case MainMenuOptions.Exit:
                        exit = true;
                        break;

                    case MainMenuOptions.Admin:
                        AdminMenu();
                        break;

                    case MainMenuOptions.Call:
                        CallMenu();
                        break;

                    case MainMenuOptions.Volunteer:
                        VolunteerMenu();
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

            if (Enum.TryParse(Console.ReadLine(), out AdminMenuOptions choice))
            {
                try
                {
                    switch (choice)
                    {
                        case AdminMenuOptions.Exit:
                            exit = true;
                            break;

                        case AdminMenuOptions.GetClock:
                            Console.WriteLine(s_bl.Admin.GetClock());
                            break;

                        case AdminMenuOptions.AdvanceClock:
                            Console.Write("Enter time unit to advance:\n" +
                                "0. Minute\n1. Hour\n2. Day\n3. Month\n4. Year\n");
                            BO.TimeUnit timeUnit;
                            while (!Enum.TryParse(Console.ReadLine(), out timeUnit))
                            {
                                Console.WriteLine("Invalid Time Unit");
                            }
                            s_bl.Admin.AdvanceClock(timeUnit);
                            Console.WriteLine(s_bl.Admin.GetClock());
                            break;

                        case AdminMenuOptions.GetRiskRange:
                            Console.WriteLine(s_bl.Admin.GetRiskRange());
                            break;

                        case AdminMenuOptions.SetRiskRange:
                            Console.Write("Enter risk range (hh:mm:ss): ");
                            TimeSpan range;
                            while (!TimeSpan.TryParse(Console.ReadLine(), out range))
                                Console.Write("Invalid risk range ");
                            s_bl.Admin.SetRiskRange(range);
                            break;

                        case AdminMenuOptions.Reset:
                            s_bl.Admin.Reset();
                            break;

                        case AdminMenuOptions.Initialize:
                            s_bl.Admin.Intialize();
                            break;

                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    PrintException(ex);
                }
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
        }
    }

    private static void CallMenu()
    {
        bool exit = false;

        while (!exit)
        {
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

            if (Enum.TryParse(Console.ReadLine(), out CallMenuOptions choice))
            {
                try
                {
                    switch (choice)
                    {
                        case CallMenuOptions.Exit:
                            exit = true;
                            break;

                        case CallMenuOptions.CallsAmount:
                            var amounts = s_bl.Call.CallsAmount().ToArray();
                            for (int i = 0; i < amounts.Length; i++)
                                Console.WriteLine($"{(BO.Status)i}: {amounts[i]}");
                            break;

                        case CallMenuOptions.ReadAll:
                            var allCalls = s_bl.Call.ReadAll(null, null, null);
                            foreach (var call in allCalls)
                                Console.WriteLine(call);
                            break;

                        case CallMenuOptions.Read:
                            Console.Write("Enter call ID: ");
                            if (int.TryParse(Console.ReadLine(), out int id))
                                Console.WriteLine(s_bl.Call.Read(id));
                            else
                                Console.WriteLine("Invalid Id.");
                            break;

                        case CallMenuOptions.Update:
                            Console.Write("Enter call ID to update: ");
                            if (int.TryParse(Console.ReadLine(), out int updateId))
                            {
                                var callToUpdate = s_bl.Call.Read(updateId);
                                //Console.Write("New Description: ");
                                //string newDescription = Console.ReadLine();
                                //callToUpdate.Description = newDescription;
                                //s_bl.Call.Update(callToUpdate);
                                //Console.WriteLine("Call updated successfully.");
                            }
                            else
                                Console.WriteLine("Invalid Id.");
                            break;

                        case CallMenuOptions.Delete:
                            Console.Write("Enter call ID to delete: ");
                            if (int.TryParse(Console.ReadLine(), out int deleteId))
                            {
                                s_bl.Call.Delete(deleteId);
                                Console.WriteLine("Call deleted successfully.");
                            }
                            else
                                Console.WriteLine("Invalid Id.");
                            break;

                        case CallMenuOptions.Create:
                            CallMenuCreate();
                            break;

                        case CallMenuOptions.GetClosedCallList:
                            HelpGetCallList(
                                out int volunteerId,
                                out BO.CallType? callTypeFilter);
                            BO.FieldsClosedCallInList? sortField = sortFilterClose();
                            var closeCallList = s_bl.Call.GetClosedCallInList(volunteerId, callTypeFilter, sortField);
                            Console.WriteLine("Call Closed list for volunteer: ");
                            foreach (var call in closeCallList)
                                Console.WriteLine(call);
                            break;

                        case CallMenuOptions.GetOpenCallList:
                            HelpGetCallList(
                                out int volId,
                                out BO.CallType? filter);
                            BO.FieldsOpenCallInList? sort = sortFilterOpen();
                            var openCallList = s_bl.Call.GetOpenCallInList(volId, filter, sort);
                            Console.WriteLine("Call Open list for volunteer: ");
                            foreach (var call in openCallList)
                                Console.WriteLine(call);
                            break;

                        case CallMenuOptions.EndTreatment:
                            Console.Write("Enter volunteer ID to end treatment of current call in treatment: ");
                            if (int.TryParse(Console.ReadLine(), out int volunteerId2))
                            {
                                var assignmentProgress = s_bl.Volunteer.Read(volunteerId2).IsProgress;
                                if (assignmentProgress == null)
                                    Console.WriteLine("No Call In Treatmet Of Volunteer");
                                else
                                {
                                    s_bl.Call.EndTreatment(volunteerId2, assignmentProgress.Id);
                                    Console.WriteLine("Treatment ended successfully.");
                                }
                            }
                            else
                                Console.WriteLine("Invalid Id.");
                            break;

                        case CallMenuOptions.CancelTreatment:
                            Console.Write("Enter your ID : ");
                            if (int.TryParse(Console.ReadLine(), out int requesterId))
                            {

                                if (s_bl.Volunteer.Read(requesterId).Role == BO.RoleType.manager)
                                {
                                    Console.Write("Enter volunteer ID to cancel treatment of current call in treatment: ");
                                    if (!int.TryParse(Console.ReadLine(), out requesterId))
                                    {
                                        Console.WriteLine("Invalid Id.");
                                        break;
                                    }
                                }
                                
                                var assignmentProgress = s_bl.Volunteer.Read(requesterId).IsProgress;
                                if (assignmentProgress == null)
                                    Console.WriteLine("No Call In Treatmet Of Volunteer");
                                else
                                {
                                    s_bl.Call.CancelTreatment(requesterId, assignmentProgress.Id);
                                    Console.WriteLine("Treatment canceled successfully.");
                                }
                            }
                            else
                                Console.WriteLine("Invalid Id.");
                        break;

                        case CallMenuOptions.ChooseCallToTreat:
                           
                            HelpGetCallList(out int chooserId, out BO.CallType? callType);
                            BO.FieldsOpenCallInList? toSort = sortFilterOpen();
                            Console.WriteLine("Choose a call to treat:");
                            var openCall = s_bl.Call.GetOpenCallInList(chooserId, callType, toSort);
                            foreach (var call in openCall)
                                Console.WriteLine(call);

                            Console.Write("Enter call ID to treat: ");
                            if (int.TryParse(Console.ReadLine(), out int treatId))
                            {
                                s_bl.Call.ChooseCallToTreat(chooserId,treatId);
                                Console.WriteLine("Call is now being treated.");
                            }
                            else
                                Console.WriteLine("Invalid Id.");
                            break;

                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    PrintException(ex);
                }
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
        }
    }





    private static void VolunteerMenu()
    {
        bool exit = false;

        while (!exit)
        {
            Console.WriteLine("\n--- Volunteer Menu ---");
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Log In");
            Console.WriteLine("2. Read All");
            Console.WriteLine("3. Read");
            Console.WriteLine("4. Update");
            Console.WriteLine("5. Delete");
            Console.WriteLine("6. Create");

            if (Enum.TryParse(Console.ReadLine(), out VolunteerMenuOptions choice))
            {
                try
                {
                    switch (choice)
                    {
                        case VolunteerMenuOptions.Exit:
                            exit = true;
                            break;

                        case VolunteerMenuOptions.LogIn:
                            BO.RoleType role = LogInHelp();
                            Console.Write("Volunteer Role is: ", role);
                            break;

                        case VolunteerMenuOptions.ReadAll:
                            Console.WriteLine("Reading all volunteers...");
                            var volList = s_bl.Volunteer.ReadAll(null);
                            foreach (var volunteer in volList)
                                Console.WriteLine(volunteer);
                            break;

                        case VolunteerMenuOptions.Read:
                            Console.Write("Enter Volunteer ID to Read: ");
                            if (int.TryParse(Console.ReadLine(), out int readId))
                            {
                                var volunteer = s_bl.Volunteer.Read(readId);
                                Console.WriteLine(volunteer);
                            }
                            else
                                Console.WriteLine("Invalid ID format.");
                            break;

                        case VolunteerMenuOptions.Update:
                            Console.Write("Enter Volunteer ID to Update: ");
                            //if (int.TryParse(Console.ReadLine(), out int updateId))
                            //{
                            //    var existingVolunteer = s_bl.Volunteer.Read(updateId);
                            //    Console.WriteLine($"Existing Volunteer: {existingVolunteer}");

                            //    Console.Write("Enter New Name: ");
                            //    string newName = Console.ReadLine();

                            //    existingVolunteer.Name = newName;
                            //    s_bl.Volunteer.Update(existingVolunteer);

                            //    Console.WriteLine("Volunteer updated successfully.");
                            //}
                            //else
                            //    Console.WriteLine("Invalid ID format.");
                            break;

                        case VolunteerMenuOptions.Delete:
                            Console.Write("Enter Volunteer ID to Delete: ");
                            if (int.TryParse(Console.ReadLine(), out int deleteId))
                            {
                                s_bl.Volunteer.Delete(deleteId);
                                Console.WriteLine($"Volunteer with ID {deleteId} deleted successfully.");
                            }
                            else
                                Console.WriteLine("Invalid ID format.");
                            break;

                        case VolunteerMenuOptions.Create:
                            Console.Write("Enter Volunteer Name: ");
                            string name = Console.ReadLine();

                            Console.Write("Enter Address: ");
                            string address = Console.ReadLine();

                            Console.Write("Enter Phone Number: ");
                            string phoneNumber = Console.ReadLine();

                            var newVolunteer = new BO.Volunteer
                            {
                                Name = name,
                                Address = address,
                                PhoneNumber = phoneNumber
                            };

                            s_bl.Volunteer.Create(newVolunteer);
                            Console.WriteLine("New volunteer created successfully.");
                            break;

                        default:
                            Console.WriteLine("Option not implemented.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    PrintException(ex);
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number between 0 and 6.");
            }
        }

    }

    private static void PrintException(Exception ex)
    {
        Console.WriteLine($"Exception: {ex.GetType().Name}");
        Console.WriteLine($"Message: {ex.Message}");
        if (ex.InnerException != null)
            Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
    }
    
    private static void CallMenuCreate()
    {
        Console.WriteLine("Enter new call details:\n");
        Console.Write("Call Type\n0: Transportation\n1: Babysitting\n2: Shopping\n3: food\n4: Cleaning:\n");
        BO.CallType callType;
        while (!Enum.TryParse(Console.ReadLine(), out callType))
        {
            Console.WriteLine("Invalid Call Type.");
        }
        Console.Write("Verbal Description:\n");
        string? description = Console.ReadLine();

        Console.Write("Address: \n");
        string? address = Console.ReadLine();

        Console.Write("Max Time To End (yyyy-MM-dd HH:mm) [optional]:\n");
        DateTime? maxTimeToEnd = DateTime.TryParse(Console.ReadLine(), out DateTime maxEndTime) ? maxEndTime : null;
        BO.Call call = new BO.Call()
        {
            Id = 0,

            TheCallType = callType,

            VerbalDescription = description,

            Address = address,

            Latitude = 0,

            Longitude = 0,

            OpeningTime = DateTime.Now,

            MaxTimeToEnd = maxTimeToEnd,

            status = BO.Status.treatment,

            listAssignForCall = null
        };
        s_bl.Call.Create(call);
    }

    private static BO.RoleType LogInHelp()
    {
        Console.Write("Enter Volunteer Name to Log In: ");
        string? name = Console.ReadLine();
        Console.Write("Enter Volunteer Password to Log In: ");
        string? password = Console.ReadLine();
        return s_bl.Volunteer.LogIn(name, password);
    }


    private static void HelpGetCallList(out int volunteerId, out BO.CallType? callTypeFilter)
    {
        volunteerId = -1;
        callTypeFilter = null;
        Console.WriteLine("\nEnter Volunteer Id: ");
        while (!int.TryParse(Console.ReadLine(), out volunteerId))
        {
            Console.WriteLine("Invalid Volunteer Id. Please enter a valid number:");
        }

        Console.WriteLine("Do you want to filter the list? (1 for Yes, 0 for No): ");
        if (int.TryParse(Console.ReadLine(), out int filterInput) && filterInput == 1)
        {
            // תפריט סינון לפי CallType
            Console.WriteLine("\nChoose a Call Type to filter by:");
            Console.WriteLine("1. Transportation");
            Console.WriteLine("2. Babysitting");
            Console.WriteLine("3. Shopping");
            Console.WriteLine("4. Food");
            Console.WriteLine("5. Cleaning");
            Console.WriteLine("6. None");
            Console.WriteLine("7. All");

            switch (Console.ReadLine())
            {
                case "1":
                    callTypeFilter = BO.CallType.Transportation;
                    break;
                case "2":
                    callTypeFilter = BO.CallType.Babysitting;
                    break;
                case "3":
                    callTypeFilter = BO.CallType.Shopping;
                    break;
                case "4":
                    callTypeFilter = BO.CallType.food;
                    break;
                case "5":
                    callTypeFilter = BO.CallType.Cleaning;
                    break;
                case "6":
                    callTypeFilter = BO.CallType.None;
                    break;
                case "7":
                    callTypeFilter = null; // ללא סינון
                    break;
                default:
                    Console.WriteLine("Invalid input. No filter applied.");
                    break;
            }
        }
    }


    static BO.FieldsClosedCallInList? sortFilterClose()
    {
        BO.FieldsClosedCallInList? sortField = null;
        Console.WriteLine("Do you want to sort the list? (1 for Yes, 0 for No): ");
        if (int.TryParse(Console.ReadLine(), out int sortInput) && sortInput == 1)
        {
            // תפריט מיון לפי FieldsClosedCallInList
            Console.WriteLine("\nChoose a field to sort by:");
            Console.WriteLine("1. Id");
            Console.WriteLine("2. Call Type");
            Console.WriteLine("3. Address");
            Console.WriteLine("4. Opening Time");
            Console.WriteLine("5. Entry Time");
            Console.WriteLine("6. Actual End Time");
            Console.WriteLine("7. End Type");

            // קליטת הבחירה של המשתמש
            switch (Console.ReadLine())
            {
                case "1":
                    sortField = BO.FieldsClosedCallInList.Id;
                    break;
                case "2":
                    sortField = BO.FieldsClosedCallInList.TheCallType;
                    break;
                case "3":
                    sortField = BO.FieldsClosedCallInList.Address;
                    break;
                case "4":
                    sortField = BO.FieldsClosedCallInList.OpeningTime;
                    break;
                case "5":
                    sortField = BO.FieldsClosedCallInList.EntryTime;
                    break;
                case "6":
                    sortField = BO.FieldsClosedCallInList.ActualEndTime;
                    break;
                case "7":
                    sortField = BO.FieldsClosedCallInList.TheEndType;
                    break;
                default:
                    Console.WriteLine("Invalid input. No sorting applied.");
                    break;
            }
        }
        // החזרת הערך שנבחר או null
        return sortField;
    }

    static BO.FieldsOpenCallInList? sortFilterOpen()
    {
        BO.FieldsOpenCallInList? sortField = null;
        Console.WriteLine("Do you want to sort the list? (1 for Yes, 0 for No): ");
        if (int.TryParse(Console.ReadLine(), out int sortInput) && sortInput == 1)
        {
            // תפריט מיון לפי FieldsOpenCallInList
            Console.WriteLine("\nChoose a field to sort by:");
            Console.WriteLine("1. Id");
            Console.WriteLine("2. Call Type");
            Console.WriteLine("3. Verbal Description");
            Console.WriteLine("4. Address");
            Console.WriteLine("5. Opening Time");
            Console.WriteLine("6. Max Time To End");
            Console.WriteLine("7. Distance");

            // קליטת הבחירה של המשתמש
            switch (Console.ReadLine())
            {
                case "1":
                    sortField = BO.FieldsOpenCallInList.Id;
                    break;
                case "2":
                    sortField = BO.FieldsOpenCallInList.TheCallType;
                    break;
                case "3":
                    sortField = BO.FieldsOpenCallInList.VerbalDescription;
                    break;
                case "4":
                    sortField = BO.FieldsOpenCallInList.Address;
                    break;
                case "5":
                    sortField = BO.FieldsOpenCallInList.OpeningTime;
                    break;
                case "6":
                    sortField = BO.FieldsOpenCallInList.MaxTimeToEnd;
                    break;
                case "7":
                    sortField = BO.FieldsOpenCallInList.Distance;
                    break;
                default:
                    Console.WriteLine("Invalid input. No sorting applied.");
                    break;
            }

            // החזרת הערך שנבחר או null
        }
        return sortField;
    }

}

