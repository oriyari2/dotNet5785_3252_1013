namespace DalApi;

/// <summary>
/// Represents the in-memory data access implementation of the DAL.
/// </summary>
public interface IDal
{
    IVolunteer Volunteer { get; }
    ICall Call { get; }
    IAssignment Assignment { get; }
    IConfig Config { get; }
    void ResetDB();
}
