namespace BO;

/// <summary>
/// Specifies the role types for users in the system.
/// </summary>
public enum RoleType { manager, volunteer }; // Defines the roles: manager or volunteer

/// <summary>
/// Specifies the types of distances used in the system.
/// </summary>
public enum DistanceType { air, walking, driving }; // Defines distance types: air, walking, or driving

/// <summary>
/// Specifies the types of assistance requests (calls) in the system.
/// </summary>
public enum CallType { Transportation, Babysitting, Shopping, food, Cleaning, None }; // Defines call types: transportation, babysitting, etc.

/// <summary>
/// Specifies the statuses of a call in the system.
/// </summary>
public enum Status { treatment, riskTreatment }; // Defines call statuses: under treatment or at risk

/// <summary>
/// Specifies the resolution types for a call.
/// </summary>
public enum EndType { treated, self, manager, expired }; // Defines resolution types: treated, self-resolved, manager-closed, or expired
