namespace BO;

/// <summary>
/// The CallInList entity represents a summarized view of a call, including its status, type, timing, and assignment details.
/// </summary>
/// <param name="Id">A unique identifier for the record in the list.</param>
/// <param name="CallId">The unique identifier of the call.</param>
/// <param name="TheCallType">The type of the call (e.g., emergency, service request).</param>
/// <param name="OpeningTime">The time when the call was opened.</param>
/// <param name="TimeToEnd">The remaining time until the call should end, if applicable.</param>
/// <param name="LastVolunteer">The name of the last volunteer assigned to the call.</param>
/// <param name="CompletionTreatment">The time taken to complete the call's treatment, if applicable.</param>
/// <param name="status">The current status of the call (e.g., open, closed, in-progress).</param>
/// <param name="TotalAssignments">The total number of assignments associated with the call.</param>
public class CallInList
{
    public int Id { get; set; } // Unique identifier for the record in the list

    public int CallId { get; set; } // Unique identifier of the call

    public CallType TheCallType { get; set; } // Type of the call

    public DateTime OpeningTime { get; set; } // Time when the call was opened

    public TimeSpan? TimeToEnd { get; set; } // Remaining time until the call ends, if applicable

    public string? LastVolunteer { get; set; } // Name of the last volunteer assigned to the call

    public TimeSpan? CompletionTreatment { get; set; } // Time taken to complete the call's treatment

    public Status status { get; set; } // Current status of the call

    public int TotalAssignments { get; set; } // Total number of assignments related to the call
}
