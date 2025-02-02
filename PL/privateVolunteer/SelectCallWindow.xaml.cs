using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PL.privateVolunteer;

/// <summary>
/// Interaction logic for SelectCall.xaml
/// </summary>
public partial class SelectCallWindow : Window
{
    // Static reference to the business logic API (BlApi)
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private volatile DispatcherOperation? _observerOperation = null; //stage 7
    private volatile DispatcherOperation? _observerOperation2 = null; //stage 7
    private volatile DispatcherOperation? _observerOperation3 = null; //stage 7

    /// <summary>
    /// Constructor for SelectCallWindow.
    /// Initializes the window and loads volunteer and call list data.
    /// </summary>
    public SelectCallWindow(int id)
    {
        InitializeComponent();
        DataContext = this;
        try
        {
            // Fetch current volunteer by ID
            CurrentVolunteer = s_bl.Volunteer.Read(id);
            RefreshCallList();
        }
        catch (Exception ex)
        {
            // Show an error message if initialization fails.
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Property for the current volunteer.
    /// </summary>
    public BO.Volunteer CurrentVolunteer
    {
        get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
        set { SetValue(CurrentVolunteerProperty, value); }
    }

    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(SelectCallWindow), new PropertyMetadata(null));

    /// <summary>
    /// Property for the list of open calls.
    /// </summary>
    public IEnumerable<BO.OpenCallInList> CallList
    {
        get { return (IEnumerable<BO.OpenCallInList>)GetValue(CallListProperty); }
        set { SetValue(CallListProperty, value); }
    }

    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register("CallList", typeof(IEnumerable<BO.OpenCallInList>), typeof(SelectCallWindow), new PropertyMetadata(null));

    /// <summary>
    /// The selected call type for filtering calls.
    /// </summary>
    public BO.CallType callType { get; set; } = BO.CallType.None;

    /// <summary>
    /// Refreshes the CallList property with updated data.
    /// </summary>
    private void RefreshCallList()
    {
        CallList = helpReadAllCall(callType);
    }

    /// <summary>
    /// Helper method to fetch the list of calls based on the selected call type filter.
    /// </summary>
    /// <param name="callTypeHelp">The call type filter to apply.</param>
    /// <returns>List of calls matching the filter.</returns>
    private IEnumerable<BO.OpenCallInList> helpReadAllCall(BO.CallType callTypeHelp)
    {
        // Return list of calls filtered by call type
        return (callTypeHelp == BO.CallType.None)
            ? s_bl?.Call.GetOpenCallInList(CurrentVolunteer.Id, null, BO.FieldsOpenCallInList.Distance)!
            : s_bl?.Call.GetOpenCallInList(CurrentVolunteer.Id, callTypeHelp, BO.FieldsOpenCallInList.Distance)!;
    }

    // Helper method for refreshing call list
    private void CallListObserver()
{
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                RefreshCallList();
            });
}
    /// <summary>
    /// Event handler triggered when the call type ComboBox selection changes.
    /// Updates the call list based on the selected call type filter.
    /// </summary>
    private void CallTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Update the selected call type based on the ComboBox selection
        if (sender is ComboBox comboBox && comboBox.SelectedItem is BO.CallType selectedType)
        {
            callType = selectedType; // Update the selected call type
            RefreshCallList(); // Refresh the call list with the new filter
        }
    }

    /// <summary>
    /// Adds the observer when the window is loaded.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(CallListObserver); // Adds observer for call list
        s_bl.Volunteer.AddObserver(VolunteerObserver); // Adds observer for volunteer
        s_bl.Admin.AddClockObserver(clockObserver); // Register for clock updates
        s_bl.Admin.AddConfigObserver(clockObserver);
        RefreshCallList();
    }

    /// <summary>
    /// Removes the observer when the window is closed.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(CallListObserver); // Removes observer for call list
        s_bl.Volunteer.RemoveObserver(VolunteerObserver); // Removes observer for volunteer
        s_bl.Admin.RemoveClockObserver(clockObserver);
        s_bl.Admin.RemoveConfigObserver(clockObserver);
    }

    private void clockObserver()
    {
        if (_observerOperation3 is null || _observerOperation3.Status == DispatcherOperationStatus.Completed)
            _observerOperation3 = Dispatcher.BeginInvoke(() =>
            {
                RefreshVolunteer();
                RefreshCallList();
            });
    }
    /// <summary>
    /// Observer to refresh the volunteer data whenever there are updates.
    /// </summary>
    private void RefreshVolunteer()
    {
        CurrentVolunteer = helpReadVolunteer();
    }

    /// <summary>
    /// Helper method to fetch the current volunteer details.
    /// </summary>
    private BO.Volunteer helpReadVolunteer()
    {
        return s_bl.Volunteer.Read(CurrentVolunteer.Id);
    }

    // Observer method for volunteer data updates
    private void VolunteerObserver() 
{
        if (_observerOperation2 is null || _observerOperation2.Status == DispatcherOperationStatus.Completed)
            _observerOperation2 = Dispatcher.BeginInvoke(() =>
            {
                RefreshVolunteer();
            });
}
    /// <summary>
    /// Property to store the currently selected call in the DataGrid.
    /// </summary>
    public BO.OpenCallInList? SelectedCall { get; set; }

    /// <summary>
    /// Opens the CallWindow for the selected call when the user double-clicks a row in the DataGrid.
    /// </summary>
    private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is DataGrid dataGrid)
        {
            // Update SelectedCall with the selected row
            SelectedCall = (BO.OpenCallInList)dataGrid.SelectedItem;
            // Display a message with the description of the selected call
            if (SelectedCall != null)
                MessageBox.Show(SelectedCall.VerbalDescription, "Call Description", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    /// <summary>
    /// Handles the click event of the select call button.
    /// </summary>
    private void btnSelectCall_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int callId)
        {
            var result = MessageBox.Show(
                "Are you sure you want to select the call?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                // Attempt to select the call for the volunteer
                s_bl.Call.ChooseCallToTreat(CurrentVolunteer.Id, callId);
                RefreshCallList();

                // Notify user of successful call selection
                MessageBox.Show("Call selected successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                // Show an error message if call selection fails.
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
