﻿namespace BlImplementation;
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
    /// 
    public void AdvanceClock(BO.TimeUnit unit)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.UpdateClock(unit switch // Switch case to handle different time units
        {
            BO.TimeUnit.Minute => AdminManager.Now.AddMinutes(1), // Add one minute
            BO.TimeUnit.Hour => AdminManager.Now.AddHours(1), // Add one hour
            BO.TimeUnit.Day => AdminManager.Now.AddDays(1), // Add one day
            BO.TimeUnit.Month => AdminManager.Now.AddMonths(1), // Add one month
            BO.TimeUnit.Year => AdminManager.Now.AddYears(1), // Add one year
            _ => DateTime.MinValue // Default case to return a minimum value if the unit is not recognized
        });
    }

    /// <summary>
    /// Gets the current clock time.
    /// </summary>
    /// <returns>The current date and time of the clock.</returns>
    public DateTime GetClock()
    {
        return AdminManager.Now; // Return the current time managed by ClockManager.
    }

    /// <summary>
    /// Retrieves the risk range from the DAL.
    /// </summary>
    /// <returns>The time span representing the risk range.</returns>
    public TimeSpan GetRiskRange()
    {
        return AdminManager.RiskRange; // Retrieve the risk range configuration from the DAL.
    }  

    /// <summary>
    /// Initializes the database and clock.
    /// </summary>
    public void Intialize()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.InitializeDB(); //stage 7
    }

    /// <summary>
    /// Resets the database and updates the clock to the current time.
    /// </summary>
    public void Reset()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ResetDB(); //stage 7
    }

    /// <summary>
    /// Sets a new risk range value in the DAL.
    /// </summary>
    /// <param name="riskRange">The new risk range value to set.</param>
    public void SetRiskRange(TimeSpan riskRange)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.RiskRange = riskRange; // Set the new risk range in the DAL configuration.
    }

    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
   AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5

    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }

    public void StopSimulator()
    => AdminManager.Stop(); //stage 7

}

