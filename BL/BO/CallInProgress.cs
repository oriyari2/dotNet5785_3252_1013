namespace BO;

/// <summary>
/// The CallInProgress entity represents a call currently being handled, 
/// including details such as the call's type, description, location, timing, and distance.
/// </summary>
/// <param name="Id">A unique identifier for the record in the system.</param>
/// <param name="CallId">The unique identifier of the call.</param>
/// <param name="TheCallType">The type of the call (e.g., emergency, service request).</param>
/// <param name="VerbalDescription">A brief description of the call.</param>
/// <param name="Address">The address associated with the call.</param>
/// <param name="OpeningTime">The time when the call was first opened.</param>
/// <param name="MaxTimeToEnd">The maximum allowed time to resolve the call.</param>
/// <param name="EntryTime">The time when the volunteer or handler began addressing the call.</param>
/// <param name="Distance">The distance to the call's location from the handler or volunteer.</param>
/// <param name="status">The current status of the call (e.g., in-progress, escalated).</param>
public class CallInProgress
{
    public int Id { get; set; } // Unique identifier for the record

    public int CallId { get; set; } // Unique identifier of the call

    public CallType TheCallType { get; set; } // Type of the call

    public string? VerbalDescription { get; set; } // Description of the call

    public string Address { get; set; } // Address associated with the call

    public DateTime OpeningTime { get; set; } // Time when the call was opened

    public DateTime? MaxTimeToEnd { get; set; } // Maximum allowed time to resolve the call

    public DateTime EntryTime { get; set; } // Time when the handling of the call began

    public double Distance { get; set; } // Distance to the call's location

    public Status status { get; set; } // Current status of the call
}
