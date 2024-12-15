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
                            Console.Write("Enter time unit to advance: ");
                            if (Enum.TryParse(Console.ReadLine(), out BO.TimeUnit timeUnit))
                                s_bl.Admin.AdvanceClock(timeUnit);
                            else
                                Console.WriteLine("Invalid Time Unit");
                            break;

                        case AdminMenuOptions.GetRiskRange:
                            Console.WriteLine(s_bl.Admin.GetRiskRange());
                            break;

                        case AdminMenuOptions.SetRiskRange:
                            Console.Write("Enter risk range (hh:mm:ss): ");
                            if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan range))
                                s_bl.Admin.SetRiskRange(range);
                            else
                                Console.Write("Invalid risk range ");
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
                            foreach (var amount in s_bl.Call.CallsAmount())
                                Console.WriteLine(amount);
                            break;

                        case CallMenuOptions.Read:
                            Console.Write("Enter call ID: ");
                            if (int.TryParse(Console.ReadLine(), out int id))
                                Console.WriteLine(s_bl.Call.Read(id));
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
}

