using DalApi;
namespace Dal;
public class ConfigImplementation : IConfig // Class implementing the IConfig interface
{
    public DateTime Clock // Property to get or set the Clock value
    {
        get => Config.Clock; // Retrieves the clock from the Config class
        set => Config.Clock = value; // Sets the clock value in the Config class
    }

    public TimeSpan RiskRange // Property to get or set the RiskRange value
    {
        get => Config.RiskRange; // Retrieves the risk range from the Config class
        set => Config.RiskRange = value; // Sets the risk range value in the Config class
    }

    public void Reset() // Method to reset the configuration settings
    {
        Config.Reset(); // Calls the Reset method of the Config class
    }
}

