namespace BlImplementation;
using BlApi;
using Helpers;

using System;

/// <summary>
/// The AdminImplementation class provides administrative functionality, including clock management, database reset, and risk range configuration.
/// </summary>
public class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get; // Initialize the data access layer (DAL) instance using the factory pattern.

    /// <summary>
    /// Advances the clock by a specified time unit.
    /// </summary>
    /// <param name="unit">The time unit to advance (Minute, Hour, Day, Month, or Year).</param>
    public void AdvanceClock(BO.TimeUnit unit) => ClockManager.UpdateClock(unit switch // Switch case to handle different time units
    {
        BO.TimeUnit.Minute => ClockManager.Now.AddMinutes(1), // Add one minute
        BO.TimeUnit.Hour => ClockManager.Now.AddHours(1), // Add one hour
        BO.TimeUnit.Day => ClockManager.Now.AddDays(1), // Add one day
        BO.TimeUnit.Month => ClockManager.Now.AddMonths(1), // Add one month
        BO.TimeUnit.Year => ClockManager.Now.AddYears(1), // Add one year
        _ => DateTime.MinValue // Default case to return a minimum value if the unit is not recognized
    });

    /// <summary>
    /// Gets the current clock time.
    /// </summary>
    /// <returns>The current date and time of the clock.</returns>
    public DateTime GetClock()
    {
        return ClockManager.Now; // Return the current time managed by ClockManager.
    }

    /// <summary>
    /// Retrieves the risk range from the DAL.
    /// </summary>
    /// <returns>The time span representing the risk range.</returns>
    public TimeSpan GetRiskRange()
    {
        return _dal.Config.RiskRange; // Retrieve the risk range configuration from the DAL.
    }

    /// <summary>
    /// Initializes the database and clock.
    /// </summary>
    public void Intialize()
    {
        DalTest.Initialization.Do(); // Perform the DAL initialization
        ClockManager.UpdateClock(ClockManager.Now); // Set the current time in the ClockManager
    }

    /// <summary>
    /// Resets the database and updates the clock to the current time.
    /// </summary>
    public void Reset()
    {
        _dal.ResetDB(); // Reset the database to its initial state
        ClockManager.UpdateClock(ClockManager.Now); // Reset the clock to the current time
        Console.WriteLine("Reset completed successfully."); // Inform the user that the reset was successful
    }

    /// <summary>
    /// Sets a new risk range value in the DAL.
    /// </summary>
    /// <param name="riskRange">The new risk range value to set.</param>
    public void SetRiskRange(TimeSpan riskRange)
    {
        _dal.Config.RiskRange = riskRange; // Set the new risk range in the DAL configuration.
    }
}

