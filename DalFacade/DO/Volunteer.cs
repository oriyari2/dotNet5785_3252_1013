using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DO;

/// <summary>
/// The Volunteer entity contains personal information for a "volunteer", including a unique ID.
/// </summary>
/// <param name="Id">Represents an ID card that uniquely identifies the volunteer.</param>
/// <param name="Name">Full name (first and last name)</param>
/// <param name="PhoneNumber">Represents a standard mobile phone.</param>
/// <param name="Email">Represents an email address</param>
/// <param name="Password">Login password</param>
/// <param name="Address">Full address of the volunteer.</param>
/// <param name="Latitude">A number indicating how far south or north a point on the Earth's
/// surface is from the equator.</param>
/// <param name="Longitude">A number indicating how far east or west a point on the Earth's
/// surface is from the equator.</param>
/// <param name="Role">Role in the system. Manager or volunteer</param>
/// <param name="Active">A boolean type that returns whether the volunteer is active
/// or inactive.</param>
/// <param name="MaxDistance">Maximum distance to receive a reading</param>
/// <param name="TheDistanceType">Distance type: air distance, walking distance,
/// driving distance</param>
public record Volunteer
(
    int Id,
    string Name,
    string PhoneNumber,
    string Email,
    string? Password, //place to bonus$$
    string? Address,
    double? Latitude,
    double? Longitude,
    RoleType Role,
    bool Active,
    double? MaxDistance,
    DistanceType TheDistanceType //place to bonus$$
    )
{
    public Volunteer() : this(0, "", "", "", null, null, null, null,
    RoleType.volunteer, false, null, DistanceType.air)
    { } // empty ctor


}

