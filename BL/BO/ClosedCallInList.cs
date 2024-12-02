namespace BO;

/// <summary>
/// The ClosedCallInList entity represents a call that has been closed, 
/// including details such as its type, address, timing, and closure type.
/// </summary>
/// <param name="Id">A unique identifier for the closed call record.</param>
/// <param name="TheCallType">The type of the call (e.g., emergency, service request).</param>
/// <param name="Address">The address associated with the call.</param>
/// <param name="OpeningTime">The time when the call was first opened.</param>
/// <param name="EntryTime">The time when handling of the call began.</param>
/// <param name="ActualEndTime">The actual time when the call was resolved and closed.</param>
/// <param name="TheEndType">The type of closure for the call (e.g., resolved, canceled).</param>
public class ClosedCallInList
{
    public int Id { get; set; } // Unique identifier for the closed call

    public CallType TheCallType { get; set; } // Type of the call

    public string Address { get; set; } // Address associated with the call

    public DateTime OpeningTime { get; set; } // Time when the call was opened

    public DateTime EntryTime { get; set; } // Time when the handling of the call began

    public DateTime? ActualEndTime { get; set; } // Time when the call was resolved and closed

    public EndType? TheEndType { get; set; } // Type of closure for the call
}
