using PL.Volunteer;
using System.Windows;
using System.Windows.Controls;


namespace PL;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public MainWindow()
    {
        InitializeComponent();
    }
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentTime.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

    private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BO.TimeUnit.Minute);
    }
    private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BO.TimeUnit.Hour);
    }
    private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BO.TimeUnit.Day);
    }
    private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BO.TimeUnit.Month);
    }
    private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BO.TimeUnit.Year);
    }

    public TimeSpan CurrentRiskRange
    {
        get { return (TimeSpan)GetValue(CurrentRiskRangeProperty); }
        set { SetValue(CurrentRiskRangeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentRiskRange.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentRiskRangeProperty =
        DependencyProperty.Register("CurrentRiskRange", typeof(TimeSpan), typeof(MainWindow));

    private void btnUpdateRiskRange_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.SetRiskRange(CurrentRiskRange);
    }

    private void clockObserver() //maybe need parameters@
    {
        CurrentTime = s_bl.Admin.GetClock();
    }

    private void configObserver() //maybe need parameters@
    {
        CurrentRiskRange = s_bl.Admin.GetRiskRange();
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        CurrentTime = s_bl.Admin.GetClock();
        CurrentRiskRange = s_bl.Admin.GetRiskRange();
        s_bl.Admin.AddClockObserver(clockObserver);
        s_bl.Admin.AddConfigObserver(configObserver);
    }
    private void MainWindow_Closed(object sender, EventArgs e)
    {
        s_bl.Admin.RemoveClockObserver(clockObserver);
        s_bl.Admin.RemoveConfigObserver(configObserver);
    }
    private void btnVolunteers_Click(object sender, RoutedEventArgs e)
    { new VolunteerListWindow().Show(); }




}