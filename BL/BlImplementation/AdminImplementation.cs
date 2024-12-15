namespace BlImplementation;
using BlApi;
using Helpers;

using System;

internal class AdminImplementation : IAdmin
    
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AdvanceClock(BO.TimeUnit unit) => ClockManager.UpdateClock(unit switch
    {
        BO.TimeUnit.Minute => ClockManager.Now.AddMinutes(1),
        BO.TimeUnit.Hour => ClockManager.Now.AddHours(1),
        BO.TimeUnit.Day => ClockManager.Now.AddDays(1),
        BO.TimeUnit.Month => ClockManager.Now.AddMonths(1),
        BO.TimeUnit.Year => ClockManager.Now.AddYears(1),
        _ => DateTime.MinValue
    });

        
    public DateTime GetClock()
    {
        return ClockManager.Now;
    }

    public TimeSpan GetRiskRange()
    {
        return _dal.Config.RiskRange;
    }

    public void Intialize()
    {
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    public void Reset()
    {
        _dal.ResetDB();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    public void SetRiskRange(TimeSpan riskRange)
    {
        _dal.Config.RiskRange = riskRange;
    }
}
