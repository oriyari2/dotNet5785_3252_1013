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
        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
        private static ICall? s_dalCall = new CallImplementation(); //stage 1
        private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
        private static IConfig? s_dalConfig = new ConfigImplementation(); //stage 
        private enum chooseMain { exit, volunteer, call, assignment, initialization, print, config, reset };
        private enum subMenu { exit, create, read, readAll, update, delete, deleteAll }

        /*
         The function will return what action the user wants to perform on the entity.
        */
        private static subMenu inputChoose()
        {
            subMenu choose;
            Console.WriteLine("Please enter one of the following options:exit, create, read," +
                " readAll, update, delete, deleteAll");
            string? input = Console.ReadLine();
            while (!Enum.TryParse(input, out choose))//$$
                input = Console.ReadLine();
            return choose;
        }

        /*
         creates new volunteer
        */
        private static Volunteer volunteerCreate()
        {
            Console.WriteLine("Enter ID (integer):");
            int id;
            string? input = Console.ReadLine();
            while (!int.TryParse(input, out id))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer ID:");
                input = Console.ReadLine();
            }

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
            input = Console.ReadLine();
            while (!Enum.TryParse(input, out role))
            {
                Console.WriteLine("Invalid role type. Please enter a valid role type:");
                input = Console.ReadLine();
            }

            Console.WriteLine("Is the Volunteer Active? (true/false):");
            bool active;
            input = Console.ReadLine();
            while (!bool.TryParse(input, out active))
            {
                Console.WriteLine("Invalid input. Please enter true or false:");
                input = Console.ReadLine();
            }

            Console.WriteLine("Enter Max Distance (optional, decimal):");
            double? maxDistance = double.TryParse(Console.ReadLine(), out double tempMaxDistance) ? tempMaxDistance : null;

            Console.WriteLine("Enter Distance Type (air, walking, driving):");
            DistanceType theDistanceType;
            input = Console.ReadLine();
            while (!Enum.TryParse(input, out theDistanceType))
            {
                Console.WriteLine("Invalid distance type. Please enter a valid distance type:");
                input = Console.ReadLine();
            }

            Volunteer vol = new Volunteer(id, name, phoneNumber, email, password, address, latitude, longitude, role, active, maxDistance, theDistanceType);
            return vol;

        }

        /*
        A method that Chat GPT wrote. We sent him to implement the read method with the implementation
        of read in the IVolunteer class.
        */
        private static void volunteerRead()
        {
            Console.WriteLine("Enter volunteer ID:");
            int id;
            string? input = Console.ReadLine();
            while (!int.TryParse(input, out id))
            {
                Console.WriteLine("Invalid input. Please enter a valid ID:");
                input = Console.ReadLine();
            }

            Volunteer? volunteer = s_dalVolunteer?.Read(id);

            if (volunteer != null)
                Console.WriteLine(volunteer);
            else
                Console.WriteLine("Volunteer not found.");
        }

        private static void volunteerReadAll()
        {
            var listVolunteer = s_dalVolunteer.ReadAll();
            foreach (Volunteer vol in listVolunteer)
                Console.WriteLine(vol);
        }

        private static void volunteerDelete()
        {
            Console.WriteLine("Enter volunteer ID:");
            int id;
            string? input = Console.ReadLine();
            while (!int.TryParse(input, out id))
            {
                Console.WriteLine("Invalid input. Please enter a valid ID:");
                input = Console.ReadLine();
            }
            s_dalVolunteer?.Delete(id);
        }


        private static Call callCreate()
        {
            Console.WriteLine("Enter CallType (Transportation, Babysitting, Shopping, food , Cleaning):");
            CallType callType;
            string? input = Console.ReadLine();
            while (!CallType.TryParse(input, out callType))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer ID:");
                input = Console.ReadLine();
            }

            Console.WriteLine("Enter description of the call.:");
            string VerbalDescription = Console.ReadLine() ?? "";

            Console.WriteLine("Enter Address:");
            string Address = Console.ReadLine() ?? "";

            Console.WriteLine("Enter Latitude (optional, decimal):");
            double latitude;
            input = Console.ReadLine();
            while (!double.TryParse(input, out latitude))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer ID:");
                input = Console.ReadLine();
            }

            Console.WriteLine("Enter Latitude (optional, decimal):");
            double longitude;
            input = Console.ReadLine();
            while (!double.TryParse(input, out longitude))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer ID:");
                input = Console.ReadLine();
            }

            Call call = new Call(0, callType, VerbalDescription, Address, latitude, longitude, DateTime.Now, null);
            return call;

        }

        private static void callRead()
        {
            Console.WriteLine("Enter call ID:");
            int id;
            string? input = Console.ReadLine();
            while (!int.TryParse(input, out id))
            {
                Console.WriteLine("Invalid input. Please enter a valid ID:");
                input = Console.ReadLine();
            }

            Call? call = s_dalCall?.Read(id);

            if (call != null)
                Console.WriteLine(call);
            else
                Console.WriteLine("Volunteer not found.");
        }

        private static void callReadAll()
        {
            var listCall = s_dalCall.ReadAll();
            
            foreach (Call call in listCall)
                Console.WriteLine(call);
        }

        private static void callDelete()
        {
            Console.WriteLine("Enter call ID:");
            int id;
            string? input = Console.ReadLine();
            while (!int.TryParse(input, out id))
            {
                Console.WriteLine("Invalid input. Please enter a valid ID:");
                input = Console.ReadLine();
            }
            s_dalCall?.Delete(id);
        }

     
        private static Assignment assignmentCreate()
        {
            Console.WriteLine("Enter Call ID (integer):");
            int CallId;
            string? input = Console.ReadLine();
            while (!int.TryParse(input, out CallId))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer ID:");
                input = Console.ReadLine();
            }

            Console.WriteLine("Enter Call ID (integer):");
            int VolunteerId;
            input = Console.ReadLine();
            while (!int.TryParse(input, out VolunteerId))
            {
                Console.WriteLine("Invalid input. Please enter a valid integer ID:");
                input = Console.ReadLine();
            }

            
            Assignment assignment = new Assignment(0, CallId, VolunteerId, DateTime.Now, null, null);
            return assignment;

        }

        private static void assignmentRead()
        {
            Console.WriteLine("Enter assignment ID:");
            int id;
            string? input = Console.ReadLine();
            while (!int.TryParse(input, out id))
            {
                Console.WriteLine("Invalid input. Please enter a valid ID:");
                input = Console.ReadLine();
            }

            Assignment? assignment = s_dalAssignment?.Read(id);

            if (assignment != null)
                Console.WriteLine(assignment);
            else
                Console.WriteLine("Assignment not found.");
        }

        private static void assignmentReadAll()
        {
            var listAssignment = s_dalAssignment.ReadAll();

            foreach (Assignment assignment in listAssignment)
                Console.WriteLine(assignment);
        }

        private static void assignmentDelete()
        {
            Console.WriteLine("Enter  assignment ID:");
            int id;
            string? input = Console.ReadLine();
            while (!int.TryParse(input, out id))
            {
                Console.WriteLine("Invalid input. Please enter a valid ID:");
                input = Console.ReadLine();
            }
            s_dalAssignment?.Delete(id);
        }
        static void Main(string[] args)
        {
            chooseMain choose;
            do
            {
                Console.WriteLine("Please enter one of the following options:exit, volunteer, call," +
                    " assignment, initialization, print, config, reset");
                string? input = Console.ReadLine();
                while (!Enum.TryParse(input, out choose))
                    input = Console.ReadLine();

                try
                {
                    switch (choose)
                    {
                        case chooseMain.exit:
                            break;
                        case chooseMain.volunteer:
                            {
                                subMenu chooseNew = inputChoose();
                                while(chooseNew!= subMenu.exit)
                                { 
                                switch (chooseNew)
                                {
                                    case subMenu.exit:
                                        break;
                                    case subMenu.create:
                                        {
                                            Volunteer vol = volunteerCreate();
                                            s_dalVolunteer?.Create(vol);
                                            break;
                                        }
                                    case subMenu.read:
                                        {
                                            volunteerRead();
                                            break;
                                        }
                                    case subMenu.readAll:
                                        {
                                            volunteerReadAll();
                                            break;
                                        }
                                    case subMenu.update:
                                        {
                                            Volunteer vol = volunteerCreate();
                                            //print vol
                                            s_dalVolunteer?.Update(vol);
                                            break;
                                        }
                                    case subMenu.delete:
                                        {
                                            volunteerDelete();
                                            break;
                                        }
                                    case subMenu.deleteAll:
                                        {
                                            s_dalVolunteer.DeleteAll();
                                            break;
                                        }
                                    default:
                                        break;
                                }
                                chooseNew = inputChoose();

                                }
                                break;
                            }
                        case chooseMain.call:
                            {
                                subMenu chooseNew = inputChoose();
                                while (chooseNew != subMenu.exit)
                                {
                                switch (chooseNew)
                                {
                                    case subMenu.exit:
                                        break;
                                    case subMenu.create:
                                        {
                                            Call newCall = callCreate();
                                            s_dalCall?.Create(newCall);
                                            break;
                                        }
                                    case subMenu.read:
                                        {
                                            callRead();
                                            break;
                                        }
                                    case subMenu.readAll:
                                        {
                                            callReadAll();
                                            break;
                                        }
                                    case subMenu.update:
                                        {
                                            Call call = callCreate();
                                            s_dalCall?.Update(call);
                                            break;
                                        }
                                    case subMenu.delete:
                                        {
                                            callDelete();
                                            break;
                                        }
                                    case subMenu.deleteAll:
                                        {
                                            s_dalCall.DeleteAll();
                                            break;
                                        }
                                    default:
                                        break;

                                }
                                    chooseNew = inputChoose();

                                }
                                break;
                            }
                        case chooseMain.assignment:
                            {
                                subMenu chooseNew = inputChoose();
                                while (chooseNew != subMenu.exit)
                                {
                                switch (chooseNew)
                                {
                                    case subMenu.exit:
                                        break;
                                    case subMenu.create:
                                        {
                                            Assignment assignment = assignmentCreate();
                                            s_dalAssignment?.Create(assignment);
                                            break;
                                        }
                                    case subMenu.read:
                                        {
                                            assignmentRead();
                                            break;
                                        }
                                    case subMenu.readAll:
                                        {
                                            assignmentReadAll();
                                            break;
                                        }
                                    case subMenu.update:
                                        {
                                            Assignment assignment = assignmentCreate();
                                            s_dalAssignment?.Update(assignment);
                                            break;
                                        }
                                    case subMenu.delete:
                                        {
                                            assignmentDelete();
                                            break;
                                        }
                                    case subMenu.deleteAll:
                                        {
                                            s_dalAssignment.DeleteAll();
                                            break;
                                        }
                                    default:
                                        break;
                                }
                                    chooseNew = inputChoose();

                                }
                                break;
                            }

                        case chooseMain.initialization:
                            { 
                            Initialization.Do(s_dalCall, s_dalAssignment, s_dalVolunteer, s_dalConfig);
                            break;
                            }
                        case chooseMain.print:
                            volunteerReadAll();
                            callReadAll();
                            assignmentReadAll();
                            break;
                        case chooseMain.config:
                            break;
                        case chooseMain.reset:
                            s_dalConfig?.Reset();
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }


            } while (choose != chooseMain.exit);

        }
    }
}
