namespace DalApi;
public interface IConfig
{
    DateTime Clock { get; set; }
    TimeSpan RiskRange { get; set; } //maybe not need##
    void Reset();

}

