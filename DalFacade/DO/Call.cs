namespace DO;

/// <summary>
/// The call entity contains details for a "call", including a unique running ID number.
/// </summary>
/// <param name="Id">Represents a number that uniquely identifies the call.</param>
/// <param name="TheCallType">Call type</param>
/// <param name="VerbalDescription">Description of the reading.</param>
/// <param name="Address">Full address of the call</param>
/// <param name="Latitude">A number indicating how far south or north a point on the
/// Earth's surface is from the equator.</param>
/// <param name="Longitude">A number indicating how far east or west a point on the
/// Earth's surface is from the equator.</param>
/// <param name="OpeningTime">Time (date and time) when the call was opened by the
/// administrator</param>
/// <param name="MaxTimeToEnd">Time (date and time) by which the call should close.</param>
public record Call
(
    int Id,
    CallType TheCallType,
    string? VerbalDescription,
    string Address,
    double Latitude,
    double Longitude,
    DateTime OpeningTime,
    DateTime? MaxTimeToEnd

    )
{
    public Call() : this(0, CallType.type1,null,"",0,0,
    DateTime.Now, DateTime.Now){ } // empty ctor 
}
