namespace BO;

/// <summary>
/// The Call entity contains details for a "call", including a unique running ID number.
/// </summary>
/// <param name="Id">Represents a number that uniquely identifies the call.</param>
/// <param name="TheCallType">Specifies the type of the call.</param>
/// <param name="VerbalDescription">A brief description of the call.</param>
/// <param name="Address">The address related to the call.</param>
/// <param name="Latitude">A number indicating how far south or north a point on the Earth's
/// surface is from the equator.</param>
/// <param name="Longitude">A number indicating how far east or west a point on the Earth's
/// surface is from the prime meridian.</param>
/// <param name="OpeningTime">The time (date and time) when the call was first opened.</param>
/// <param name="MaxTimeToEnd">The maximum time (date and time) allowed for resolving the call.</param>
/// <param name="status">The current status of the call (e.g., open, in-progress, closed).</param>
/// <param name="listAssignForCall">A list of assignments related to this call.</param>
public class Call
{
    public int Id { get; init; } // Unique identifier for the call

    public CallType TheCallType { get; init; } // Type of the call

    public string? VerbalDescription { get; init; } // Description of the call

    public string? Address { get; set; } // Address related to the call

    public double? Latitude { get; set; } // Latitude of the call's location

    public double? Longitude { get; set; } // Longitude of the call's location

    public DateTime OpeningTime { get; set; } // Time when the call was opened

    public DateTime? MaxTimeToEnd { get; set; } // Maximum time allowed for resolution

    public Status status { get; set; } // Current status of the call

    public List<BO.CallAssignInList>? listAssignForCall { get; set; } // Assignments related to the call
}
