using Helpers;

namespace BO;

/// <summary>
/// The VolunteerInList entity represents a volunteer listed in the system, 
/// including their basic details and statistics regarding their handled calls.
/// </summary>
/// <param name="Id">A unique identifier for the volunteer.</param>
/// <param name="Name">The name of the volunteer (first and last).</param>
/// <param name="Active">Indicates whether the volunteer is currently active.</param>
/// <param name="TotalHandled">The total number of calls handled by the volunteer.</param>
/// <param name="TotalCanceled">The total number of calls canceled by the volunteer.</param>
/// <param name="TotalExpired">The total number of calls that expired without completion by the volunteer.</param>
/// <param name="CurrentCall">An optional reference to the current call the volunteer is handling (if any).</param>
/// <param name="TheCallType">The type of the call the volunteer is handling (if any).</param>
public class VolunteerInList
{
    public override string ToString() => this.ToStringProperty();

    public int Id { get; init; } // Unique identifier for the volunteer

    public string Name { get; init; } // Name of the volunteer

    public bool Active { get; init; } // Indicates if the volunteer is active

    public int TotalHandled { get; init; } // Total number of calls handled by the volunteer

    public int TotalCanceled { get; init; } // Total number of calls canceled by the volunteer

    public int TotalExpired { get; init; } // Total number of calls that expired without completion

    public int? CurrentCall { get; init; } // Current call the volunteer is handling (if any)

    public CallType TheCallType { get; init; } // Type of the call the volunteer is handling
}
