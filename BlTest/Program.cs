using BO;

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
                            while (!TimeSpan.TryParse(Console.ReadLine(), out  range))
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
                            var amounts = s_bl.Call.CallsAmount().ToArray(); // שמירת התוצאה במערך
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
                            int id;
                            while (!int.TryParse(Console.ReadLine(), out  id))
                                Console.Write("Invalid Id "); 
                            try
                            { 
                            Console.WriteLine(s_bl.Call.Read(id));
                            }
                            catch(BO.BlDoesNotExistException ex) 
                            {
                                PrintException(ex);
                            }

                            break;

                        case CallMenuOptions.Update:
                            Console.Write("Enter call ID to update: ");
                            //if (int.TryParse(Console.ReadLine(), out int updateId))
                            //{
                            //    var callToUpdate = s_bl.Call.Read(updateId);
                            //    Console.WriteLine("Enter new details for the call:");
                            //    // ניתן להניח פה קבלת נתונים חדשים מהמשתמש, לדוגמה:
                            //    Console.Write("New Description: ");
                            //    string newDescription = Console.ReadLine();
                            //    callToUpdate.Description = newDescription;
                            //    s_bl.Call.Update(callToUpdate);
                            //    Console.WriteLine("Call updated successfully.");
                            //}
                            //else
                                Console.WriteLine("Invalid Id");
                            break;

                        case CallMenuOptions.Delete:
                            Console.Write("Enter call ID to delete: ");
                            if (int.TryParse(Console.ReadLine(), out int deleteId))
                            {
                                s_bl.Call.Delete(deleteId);
                                Console.WriteLine("Call deleted successfully.");
                            }
                            else
                                Console.WriteLine("Invalid Id");
                            break;

                        case CallMenuOptions.Create:
                            CallMenuCreate();
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
                Console.WriteLine("Invalid input.");

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

            if (Enum.TryParse(Console.ReadLine(), out VolunteerMenuOptions choice))
            {
                try
                {
                    switch (choice)
                    {
                        case VolunteerMenuOptions.Exit:
                            exit = true;
                            break;

                        case VolunteerMenuOptions.Read:
                            Console.Write("Enter Volunteer ID: ");
                            if (int.TryParse(Console.ReadLine(), out int id))
                                Console.WriteLine(s_bl.Volunteer.Read(id));
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

        Console.Write("Address:()\n");
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
}

