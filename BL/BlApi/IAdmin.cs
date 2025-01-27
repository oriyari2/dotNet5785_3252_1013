namespace BlApi;

public interface IAdmin
{
    public DateTime GetClock();
    public void AdvanceClock(BO.TimeUnit timeUnit);
    public TimeSpan GetRiskRange();
    public void SetRiskRange (TimeSpan riskRange);
    public void Reset();
    public void Intialize();
    void StartSimulator(int interval); //stage 7
    void StopSimulator(); //stage 7

    #region Stage 5
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    #endregion Stage 5
}
