using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace PL.privateVolunteer;

/// <summary>
/// Interaction logic for CallHistory.xaml
/// </summary>
public partial class CallHistoryWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public CallHistoryWindow(int id)
    {
        InitializeComponent();
        DataContext = this;
        try
        {
            CurrentVolunteer = s_bl.Volunteer.Read(id);
            RefreshCallList();
        }
        catch (Exception ex)
        {

            // Show an error message if initialization fails.
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
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(CallHistoryWindow), new PropertyMetadata(null));


    public IEnumerable<BO.ClosedCallInList> CallList
    {
        get { return (IEnumerable<BO.ClosedCallInList>)GetValue(CallListProperty); }
        set { SetValue(CallListProperty, value); }
    }
    /// <summary>
    /// Registration of the dependency property for CallList.
    /// </summary>
    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register("CallList", typeof(IEnumerable<BO.ClosedCallInList>), typeof(CallHistoryWindow), new PropertyMetadata(null));

    public BO.CallType callType { get; set; } = BO.CallType.None;

    /// <summary>
    /// Event handler triggered when the call type ComboBox selection changes.
    /// Updates the volunteer list based on the selected call type filter.
    /// </summary>
    private void CallTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CallList = helpReadAllCall(callType);
    }

    /// <summary>
    /// Refreshes the VolunteerList property with updated data.
    /// </summary>
    private void RefreshCallList()
    {
        CallList = helpReadAllCall(callType);

    }

    /// <summary>
    /// Helper method to fetch the list of volunteers based on the selected call type filter.
    /// </summary>
    /// <param name="callTypeHelp">The call type filter to apply.</param>
    /// <returns>List of volunteers matching the filter.</returns>
    private  IEnumerable<BO.ClosedCallInList> helpReadAllCall(BO.CallType callTypeHelp)
    {
        return (callTypeHelp == BO.CallType.None)
            ? s_bl?.Call.GetClosedCallInList(CurrentVolunteer.Id, null,BO.FieldsClosedCallInList.ActualEndTime)!
            : s_bl?.Call.GetClosedCallInList(CurrentVolunteer.Id, callTypeHelp, BO.FieldsClosedCallInList.ActualEndTime)!;
    }

    /// <summary>
    /// Observer to refresh the volunteer list whenever there are updates.
    /// </summary>
    private void CallListObserver() => RefreshCallList();

    /// <summary>
    /// Adds the observer when the window is loaded.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    { 
        s_bl.Call.AddObserver(CallListObserver);
        RefreshCallList();
    }

    /// <summary>
    /// Removes the observer when the window is closed.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
        => s_bl.Call.RemoveObserver(CallListObserver);

    /// <summary>
    /// Event handler for selection change in the DataGrid.
    /// Placeholder for additional functionality if needed.
    /// </summary>
    private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
    {
        // Placeholder
    }

   
   

    /// <summary>
    /// Property to store the currently selected volunteer in the DataGrid.
    /// </summary>
    public BO.ClosedCallInList? SelectedCall { get; set; }

    /// <summary>
    /// Opens the VolunteerWindow for the selected volunteer when the user double-clicks a row in the DataGrid.
    /// </summary>
    private void lsvCallsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        //if (SelectedCall != null)
        //    new CallWindow(SelectedCall.CallId).Show();
    }


}
