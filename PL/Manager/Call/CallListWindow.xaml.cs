using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PL.Call;

/// <summary>
/// Interaction logic for CallListWindow.xaml
/// This window displays a list of calls and allows the user to manage them.
/// </summary>
public partial class CallListWindow : Window
{
    // Static instance of the business logic layer
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private volatile DispatcherOperation? _observerOperation = null; //stage 7
    private volatile DispatcherOperation? _observerOperation2 = null; //stage 7


    /// <summary>
    /// Constructor for the CallListWindow.
    /// Initializes the window and sets the status filter if provided.
    /// </summary>
    /// <param name="status">Optional status filter for the call list.</param>
    public CallListWindow(BO.Status? status = null)
    {
        Status = status;
        InitializeComponent();
    }

    /// <summary>
    /// Dependency property to store the status filter for the call list.
    /// </summary>
    public BO.Status? Status
    {
        get { return (BO.Status?)GetValue(StatusProperty); }
        set { SetValue(StatusProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Status. This enables animation, styling, binding, etc.
    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register("Status", typeof(BO.Status?), typeof(CallListWindow), new PropertyMetadata(null));

    /// <summary>
    /// Dependency property for the CallList, which is bound to the DataGrid in the XAML.
    /// </summary>
    public IEnumerable<BO.CallInList> CallList
    {
        get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
        set { SetValue(CallListProperty, value); }
    }

    /// <summary>
    /// Registration of the dependency property for CallList.
    /// </summary>
    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow), new PropertyMetadata(null));

    /// <summary>
    /// Property to store the currently selected call type filter.
    /// </summary>
    public BO.CallType callType { get; set; } = BO.CallType.None;

    /// <summary>
    /// Event handler triggered when the call type ComboBox selection changes.
    /// Updates the call list based on the selected call type filter.
    /// </summary>
    private void CallTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CallList = helpReadAllCall(callType);
    }

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
    private IEnumerable<BO.CallInList> helpReadAllCall(BO.CallType callTypeHelp)
    {
        // Fetch the list of calls based on the selected call type
        var callStatusList = (callTypeHelp == BO.CallType.None)
             ? s_bl?.Call.ReadAll(null, null, BO.FieldsCallInList.CallId)!
            : s_bl?.Call.ReadAll(BO.FieldsCallInList.TheCallType, callTypeHelp, BO.FieldsCallInList.CallId)!;

        // Filter by status if a status filter is applied
        if (Status != null)
            return callStatusList.Where(w => w.status == Status);

        return callStatusList;
    }

    /// <summary>
    /// Observer to refresh the call list whenever there are updates.
    /// </summary>
    private void CallListObserver()
    {
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                RefreshCallList();
            });
    }

    /// <summary>
    /// Adds the observer when the window is loaded.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
{
        s_bl.Call.AddObserver(CallListObserver);
        s_bl.Admin.AddClockObserver(clockObserver); // Register for clock updates

    }
    /// <summary>
    /// Removes the observer when the window is closed.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
{
        s_bl.Call.RemoveObserver(CallListObserver);
        s_bl.Admin.RemoveClockObserver(clockObserver); // Removes observer for volunteer

    }
    private void clockObserver()
    {
        if (_observerOperation2 is null || _observerOperation2.Status == DispatcherOperationStatus.Completed)
            _observerOperation2 = Dispatcher.BeginInvoke(() =>
            {
                RefreshCallList();
            });
    }
    /// <summary>
    /// Event handler for selection change in the DataGrid.
    /// Placeholder for additional functionality if needed.
    /// </summary>
    private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
    {
        // Placeholder for additional functionality
    }

    /// <summary>
    /// Opens the CallWindow to add a new call when the Add button is clicked.
    /// </summary>
    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        new CallWindow().Show();
    }

    /// <summary>
    /// Property to store the currently selected call in the DataGrid.
    /// </summary>
    public BO.CallInList? SelectedCall { get; set; }

    /// <summary>
    /// Opens the CallWindow for the selected call when the user double-clicks a row in the DataGrid.
    /// </summary>
    private void lsvCallsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedCall != null)
            new CallWindow(SelectedCall.CallId).Show();
    }

    /// <summary>
    /// Deletes the selected call when the Delete button is clicked.
    /// Shows a confirmation dialog before performing the delete operation.
    /// </summary>
    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int callId)
        {
            // Show confirmation dialog
            var result = MessageBox.Show(
                "Are you sure you want to delete the call?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                // Attempt to delete the call
                s_bl.Call.Delete(callId);
                MessageBox.Show("Call deleted successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                // Show error message if deletion fails
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /// <summary>
    /// Cancels the assignment of the selected call when the Cancel button is clicked.
    /// Shows a confirmation dialog before performing the cancellation.
    /// </summary>
    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int Id)
        {
            // Show confirmation dialog
            var result = MessageBox.Show(
                "Are you sure you want to cancel the assignment of this call?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                // Attempt to cancel the assignment
                s_bl.Call.CancelTreatment(PO.LogInID, Id);
                MessageBox.Show("Call canceled successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                // Show error message if cancellation fails
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
