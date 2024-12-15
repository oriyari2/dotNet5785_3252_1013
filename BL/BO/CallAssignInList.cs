using Helpers;

namespace BO;

/// <summary>
/// The CallAssignInList entity represents an assignment in a call, including details about the assigned volunteer and timing information.
/// </summary>
/// <param name="VolunteerId">The unique ID of the volunteer assigned to the call.</param>
/// <param name="Name">The name of the assigned volunteer.</param>
/// <param name="EntryTime">The time when the volunteer was assigned to the call.</param>
/// <param name="ActualEndTime">The actual time when the assignment ended.</param>
/// <param name="TheEndType">The type of end for the assignment (e.g., completed, canceled).</param>
public class CallAssignInList
{
    public override string ToString() => this.ToStringProperty();
    public int? VolunteerId { get; init; } // ID of the assigned volunteer

    public string? Name { get; init; } // Name of the assigned volunteer

    public DateTime EntryTime { get; init; } // Time the volunteer was assigned to the call

    public DateTime? ActualEndTime { get; init; } // Actual time the assignment ended

    public EndType? TheEndType { get; init; } // Type of end for the assignment
}
