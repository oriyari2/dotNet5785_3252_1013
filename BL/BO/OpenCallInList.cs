namespace BO;

/// <summary>
/// The OpenCallInList entity represents a call that is currently open, 
/// including details such as its type, description, location, timing, and distance.
/// </summary>
/// <param name="Id">A unique identifier for the open call record.</param>
/// <param name="TheCallType">The type of the call (e.g., transportation, babysitting).</param>
/// <param name="VerbalDescription">A brief description of the call.</param>
/// <param name="Address">The address associated with the call.</param>
/// <param name="OpeningTime">The time when the call was first opened.</param>
/// <param name="MaxTimeToEnd">The maximum time allowed to resolve the call.</param>
/// <param name="Distance">The distance to the call's location from the handler or volunteer.</param>
public class OpenCallInList
{
    public int Id { get; set; } // Unique identifier for the open call

    public CallType TheCallType { get; set; } // Type of the call

    public string? VerbalDescription { get; set; } // Description of the call

    public string Address { get; set; } // Address associated with the call

    public DateTime OpeningTime { get; set; } // Time when the call was opened

    public DateTime? MaxTimeToEnd { get; set; } // Maximum time allowed for the call to be resolved

    public double Distance { get; set; } // Distance to the call's location
}
