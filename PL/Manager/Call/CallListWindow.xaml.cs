
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace PL.Call;

/// <summary>
/// Interaction logic for CallListWindow.xaml
/// </summary>

public partial class CallListWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public CallListWindow()
    {
        InitializeComponent();
    }
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
    private static IEnumerable<BO.CallInList> helpReadAllCall(BO.CallType callTypeHelp)
    {
        return (callTypeHelp == BO.CallType.None)
            ? s_bl?.Call.ReadAll(null ,null, BO.FieldsCallInList.CallId)!
            : s_bl?.Call.ReadAll(BO.FieldsCallInList.TheCallType, callTypeHelp, BO.FieldsCallInList.CallId)!;
    }

    /// <summary>
    /// Observer to refresh the volunteer list whenever there are updates.
    /// </summary>
    private void CallListObserver() => RefreshCallList();

    /// <summary>
    /// Adds the observer when the window is loaded.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
        => s_bl.Call.AddObserver(CallListObserver);

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
    /// Opens the VolunteerWindow to add a new volunteer when the Add button is clicked.
    /// </summary>
    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        new CallWindow().Show();
    }

    /// <summary>
    /// Property to store the currently selected volunteer in the DataGrid.
    /// </summary>
    public BO.CallInList? SelectedCall { get; set; }

    /// <summary>
    /// Opens the VolunteerWindow for the selected volunteer when the user double-clicks a row in the DataGrid.
    /// </summary>
    private void lsvCallsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedCall != null)
            new CallWindow(SelectedCall.CallId).Show();
    }

    /// <summary>
    /// Deletes the selected volunteer when the Delete button is clicked.
    /// Shows a confirmation dialog before performing the delete operation.
    /// </summary>
    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int callId)
        {
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
                s_bl.Call.Delete(callId);
                MessageBox.Show("Call deleted successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int Id)
        {
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
                s_bl.Call.CancelTreatment(PO.LogInID,Id);
                MessageBox.Show("Call canceled successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

}
