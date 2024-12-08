namespace BlImplementation;
using BlApi;
using Helpers;

using System;

internal class AdminImplementation : IAdmin
    
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AdvanceClock(BO.TimeUnit timeUnit)
    {
        throw new NotImplementedException();
    }

    public DateTime GetClock()
    {
        throw new NotImplementedException();
    }

    public TimeSpan GetRiskRange()
    {
        throw new NotImplementedException();
    }

    public void Intialize()
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public void SetRiskRange(TimeSpan riskRange)
    {
        throw new NotImplementedException();
    }
}
