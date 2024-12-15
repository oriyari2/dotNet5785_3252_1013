using DO;
using Helpers;

namespace BO;

/// <summary>
/// The Volunteer entity represents a volunteer in the system, including their personal information, role, and activity statistics.
/// </summary>
/// <param name="Id">A unique identifier for the volunteer.</param>
/// <param name="Name">The name of the volunteer (first and last).</param>
/// <param name="PhoneNumber">The volunteer's phone number.</param>
/// <param name="Email">The volunteer's email address.</param>
/// <param name="Password">The volunteer's login password (optional).</param>
/// <param name="Address">The address of the volunteer (optional).</param>
/// <param name="Latitude">The latitude coordinate of the volunteer's location (optional).</param>
/// <param name="Longitude">The longitude coordinate of the volunteer's location (optional).</param>
/// <param name="Role">The role of the volunteer (e.g., manager or volunteer).</param>
/// <param name="Active">Indicates whether the volunteer is active in the system.</param>
/// <param name="MaxDistance">The maximum distance within which the volunteer can handle calls (optional).</param>
/// <param name="TheDistanceType">The type of distance measurement used for call assignments (e.g., air, walking, driving).</param>
/// <param name="TotalHandled">The total number of calls handled by the volunteer.</param>
/// <param name="TotalCanceled">The total number of calls canceled by the volunteer.</param>
/// <param name="TotalExpired">The total number of calls that expired before being completed by the volunteer.</param>
/// <param name="IsProgress">An optional reference to the call that the volunteer is currently handling.</param>
public class Volunteer
{
    public override string ToString() => this.ToStringProperty();

    public int Id { get; init; } // Unique identifier for the volunteer

    public string Name { get; init; } // Name of the volunteer

    public string PhoneNumber { get; set; } // Phone number of the volunteer

    public string Email { get; set; } // Email address of the volunteer

    public string? Password { get; set; } // Optional login password (place to bonus$$)

    public string? Address { get; set; } // Optional address of the volunteer

    public double? Latitude { get; set; } // Optional latitude of the volunteer's location

    public double? Longitude { get; set; } // Optional longitude of the volunteer's location

    public RoleType Role { get; set; } // Role of the volunteer (manager or volunteer)

    public bool Active { get; set; } // Indicates if the volunteer is active

    public double? MaxDistance { get; set; } // Maximum distance within which the volunteer can handle calls

    public DistanceType TheDistanceType { get; set; } // Distance type (place to bonus$$)

    public int TotalHandled { get; init; } // Total calls handled by the volunteer

    public int TotalCanceled { get; init; } // Total calls canceled by the volunteer

    public int TotalExpired { get; init; } // Total calls that expired before being completed

    public BO.CallInProgress? IsProgress { get; set; } // Current call in progress (if any)
}
