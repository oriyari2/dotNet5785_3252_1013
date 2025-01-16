using PL.Call;
using PL.privateVolunteer;
using System.Windows;
using System.Windows.Controls;
namespace PL;
/// <summary>
/// Interaction logic for MainVolunteerWindow.xaml
/// </summary>
public partial class MainVolunteerWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Singleton instance of the BL API

    public MainVolunteerWindow(int id3=0)
    {
        InitializeComponent();
        DataContext = this;
        try
        {
            CurrentVolunteer = s_bl.Volunteer.Read(id3);
            if(CurrentVolunteer.IsProgress!=null)
            {
                CurrentCall = s_bl.Call.Read(CurrentVolunteer.IsProgress.CallId);
            }
            else
                CurrentCall = null;

        }
        catch (Exception ex)
        {
            /// <summary>
            /// Show an error message if initialization fails.
            /// </summary>
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public BO.Volunteer CurrentVolunteer
    {
        get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
        set { SetValue(CurrentVolunteerProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentId.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(MainVolunteerWindow), new PropertyMetadata(null));



    public BO.Call? CurrentCall
    {
        get { return (BO.Call?)GetValue(CurrentCallProperty); }
        set { SetValue(CurrentCallProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentCall.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentCallProperty =
        DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(MainVolunteerWindow), new PropertyMetadata(null));



    private void BtnUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer == null)
        {
            /// <summary>
            /// Show an error message if volunteer details are missing.
            /// </summary>
            MessageBox.Show("Volunteer details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
                s_bl.Volunteer.Update(CurrentVolunteer.Id, CurrentVolunteer);
                MessageBox.Show("Volunteer updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            /// <summary>
            /// Show an error message if something goes wrong.
            /// </summary>
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnCancelTreatment_Click(object sender, RoutedEventArgs e)
    {

            var result = MessageBox.Show(
                "Are you sure you want to cancel your treatment for this call?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                s_bl.Call.CancelTreatment(CurrentVolunteer.Id,CurrentVolunteer.IsProgress.Id);
                MessageBox.Show("Call canceled successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
       
    }
    private void BtnEndTreatmrnt_Click(object sender, RoutedEventArgs e)
    {

        var result = MessageBox.Show(
            "Are you sure you want to end your treatment for this call?",
            "Confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            s_bl.Call.EndTreatment(CurrentVolunteer.Id, CurrentVolunteer.IsProgress.Id);
            MessageBox.Show("Call ended successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
    private void BtnCallsHistory_Click(object sender, RoutedEventArgs e)
    {
        new CallHistoryWindow(CurrentVolunteer.Id).Show();
    }
    

    private void MainVolunteerWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Set initial values when the window is loaded

        RefreshCallInProgress();


        s_bl.Call.AddObserver(callInProgressObserver);
    }

    /// <summary>
    /// Cleans up observers when the window is closed to prevent memory leaks.
    /// </summary>
    private void MainVolunteerWindow_Closed(object sender, EventArgs e)
    {
        // Cleanup observers when the window is closed

        s_bl.Call.RemoveObserver(callInProgressObserver);
    }

    private void RefreshCallInProgress()
    {
        CurrentCall = helpReadCallInProgress(); // Update the amounts from the data source
    }

    /// <summary>
    /// Helper function to read the current call amounts from the backend.
    /// </summary>
    private  BO.Call? helpReadCallInProgress()
    {

        if ((s_bl.Volunteer.Read(CurrentVolunteer.Id).IsProgress) != null)
            return s_bl.Call.Read(CurrentVolunteer.IsProgress.CallId);
        return null;
    }

    /// <summary>
    /// Observes changes in the call amounts and refreshes the displayed data.
    /// </summary>
    private void callInProgressObserver() => RefreshCallInProgress();


}
