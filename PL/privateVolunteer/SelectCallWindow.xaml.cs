using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL.privateVolunteer;

/// <summary>
/// Interaction logic for SelectCall.xaml
/// </summary>
public partial class SelectCallWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public SelectCallWindow(int id)
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

    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(SelectCallWindow), new PropertyMetadata(null));

    public IEnumerable<BO.OpenCallInList> CallList
    {
        get { return (IEnumerable<BO.OpenCallInList>)GetValue(CallListProperty); }
        set { SetValue(CallListProperty, value); }
    }

    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register("CallList", typeof(IEnumerable<BO.OpenCallInList>), typeof(SelectCallWindow), new PropertyMetadata(null));

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
        return (callTypeHelp == BO.CallType.None)
            ? s_bl?.Call.GetOpenCallInList(CurrentVolunteer.Id, null, BO.FieldsOpenCallInList.Distance)!
            : s_bl?.Call.GetOpenCallInList(CurrentVolunteer.Id, callTypeHelp, BO.FieldsOpenCallInList.Distance)!;
    }
    private void CallListObserver() => RefreshCallList();
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
        s_bl.Call.AddObserver(CallListObserver);
        // s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, VolunteerObserver);
        s_bl.Volunteer.AddObserver(VolunteerObserver);
        RefreshCallList();
    }

    /// <summary>
    /// Removes the observer when the window is closed.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(CallListObserver);
        // s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, VolunteerObserver);
         s_bl.Volunteer.RemoveObserver(VolunteerObserver);
    }

    /// <summary>
    /// Observer to refresh the call list whenever there are updates.
    /// </summary>

    private void RefreshVolunteer()
    {
        CurrentVolunteer = helpReadVolunteer();
    }

    /// <summary>
    /// Helper method to fetch the list of calls based on the selected call type filter.
    /// </summary>
    /// <param name="callTypeHelp">The call type filter to apply.</param>
    /// <returns>List of calls matching the filter.</returns>
    private BO.Volunteer helpReadVolunteer()
    {
        return s_bl.Volunteer.Read(CurrentVolunteer.Id);
    }
    private void VolunteerObserver() => RefreshVolunteer();
    /// <summary>
    /// Property to store the currently selected call in the DataGrid.
    /// </summary>
    public BO.OpenCallInList? SelectedCall { get; set; }

    /// <summary>
    /// Opens the CallWindow for the selected call when the user double-clicks a row in the DataGrid.
    /// </summary>
    private void lsvCallsList_MouseClick(object sender, MouseButtonEventArgs e)
    {
        //if (SelectedCall != null)
        //    new CallWindow(SelectedCall.Id).Show();
    }

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
                s_bl.Call.ChooseCallToTreat(CurrentVolunteer.Id,callId);
                RefreshCallList();

                MessageBox.Show("Call selected successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
