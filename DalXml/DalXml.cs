using DalApi;
using System.Diagnostics;
namespace Dal;

/// <summary>
/// Represents the XML-based implementation of the data access layer (DAL) for the application.
/// This class provides access to various entities (e.g., Volunteers, Calls, Assignments, Config)
/// and includes methods for resetting the database.
/// </summary>

//in this class we used the full lazy singelton method with thread safe, exactly like we used it in dallist
//and the explanation is there
sealed internal class DalXml : IDal
{
    //Private constructor to prevent creation of new instances from outside.
    private DalXml() { }
    public static DalXml Instance => Nested.instance;
    private static class Nested
    {
        internal static readonly DalXml instance = new DalXml();
    }
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public ICall Call { get; } = new CallImplementation();

    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public IConfig Config { get; } = new ConfigImplementation();

    /// <summary>
    /// Resets the entire database by deleting all entities (Volunteers, Calls, Assignments)
    /// and resetting the configuration values to their defaults.
    /// This operation is typically used for testing or reinitializing the application state.
    /// </summary>
    public void ResetDB()
    {
        Call.DeleteAll();
        Assignment.DeleteAll();
        Volunteer.DeleteAll();
        Config.Reset();
    }
}
