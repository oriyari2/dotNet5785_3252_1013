namespace Dal;
using DalApi;
using System.Runtime.ConstrainedExecution;

/// <summary>
/// Implementation of the data access layer (DAL) using in-memory lists.
/// Provides access to the Volunteer, Call, Assignment, and Config entities
/// as well as the ability to reset the database.
/// </summary>

//in this class we used the full lazy singelton method with thread safe, here is an explanation:
//Singleton ensures that there is only one instance of a class during the life of the program,
//and that access to this instance is done globally.
//Lazy Initialization means that the instance of the class will only be created the first time it is needed.
//This saves resources if the instance is not needed during runtime.

//How is this done here?
//The instance variable of the Nested inner class is readonly and is initialized lazily because it is only
//set when it is first called via the Instance property.
//In reality, the Nested class is only loaded into memory when it is first accessed(this happens because
//inner classes are only loaded when they are needed).

//Because the initialization of an instance is performed within a static class (Nested), we can rely on
//the mechanism of the.NET CLR (Common Language Runtime) which guarantees:
//Single Initialization): The static variable is initialized exactly once in the entire program,automatically.
//Thread Safety: The CLR ensures that the initialization of static variables is thread-safe by design,
//meaning that if two threads try to access the same static variable at the same time, only one of them will
//perform the initialization.
sealed internal class DalList : IDal
{
    //Private constructor to prevent creation of new instances from outside.
    private DalList() { }
    public static DalList Instance => Nested.instance;
    private static class Nested
    {
        internal static readonly DalList instance = new DalList();
    }

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public ICall Call { get; } = new CallImplementation();

    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public IConfig Config { get; } = new ConfigImplementation();

    /// <summary>
    /// Resets the database by clearing all data in Volunteer, Call, and Assignment entities 
    /// and resetting the configuration to its default state.
    /// </summary>
    public void ResetDB()
    {
        Volunteer.DeleteAll(); // Clear volunteer data.
        Call.DeleteAll(); // Clear call data.
        Assignment.DeleteAll(); // Clear assignment data.
        Config.Reset(); // Reset configuration.

    }
}
