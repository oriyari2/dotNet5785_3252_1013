namespace Dal;
using DalApi;

/// <summary>
/// Implementation of the data access layer (DAL) using in-memory lists.
/// Provides access to the Volunteer, Call, Assignment, and Config entities
/// as well as the ability to reset the database.
/// </summary>
sealed public class DalList : IDal
{
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
