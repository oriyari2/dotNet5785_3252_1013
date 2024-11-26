using DalApi;
namespace Dal;

/// <summary>
/// Represents the XML-based implementation of the data access layer (DAL) for the application.
/// This class provides access to various entities (e.g., Volunteers, Calls, Assignments, Config)
/// and includes methods for resetting the database.
/// </summary>
public class DalXml : IDal
{
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
