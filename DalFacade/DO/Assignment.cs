namespace DO;

/// <summary>
/// An entity that links a "call" to a "volunteer" who has chosen
/// to handle it. The call has been assigned to the volunteer.
/// Includes a running and unique ID number of the linking entity.
/// As well as an ID number of the call and the volunteer's ID number.
/// </summary>
/// <param name="Id">Running ID number of the allocation entity</param>
/// <param name="CallId">Running ID number of the calling entity</param>
/// <param name="VolunteerId">Volunteer ID</param>
/// <param name="EntryTime">Time of entry for treatment</param>
/// <param name="ActualEndTime">Actual treatment completion time</param>
/// <param name="TheEndType">Type of treatment termination</param>
public record Assignment
(
    int Id,
    int CallId,
    int VolunteerId,
    DateTime EntryTime,
    DateTime? ActualEndTime,
    EndType? TheEndType
)
{
    public Assignment() : this(0, 0, 0, DateTime.Now,
    null, null)
    { } // empty ctor
}
