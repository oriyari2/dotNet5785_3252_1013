namespace BlApi;

public interface IAdmin
{
    public DateTime GetClock();
    public void AdvanceClock(BO.TimeUnit timeUnit);
    public TimeSpan GetRiskRange();
    public void SetRiskRange (TimeSpan riskRange);
    public void Reset();
    public void Intialize();


}
