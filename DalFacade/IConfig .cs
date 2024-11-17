namespace DalApi;
public interface IConfig // Interface for configuration settings
{
    DateTime Clock { get; set; } // Property for the current time or clock
    TimeSpan RiskRange { get; set; } // Property for the risk range duration
    void Reset(); // Method to reset the configuration
}
