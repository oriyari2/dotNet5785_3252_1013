namespace DalApi;

/// <summary>
/// Interface for managing configuration settings in the data access layer (DAL).
/// </summary>
public interface IConfig // Interface for configuration settings
{
    DateTime Clock { get; set; } // Property for the current time or clock
    TimeSpan RiskRange { get; set; } // Property for the risk range duration
    void Reset(); // Method to reset the configuration
}
