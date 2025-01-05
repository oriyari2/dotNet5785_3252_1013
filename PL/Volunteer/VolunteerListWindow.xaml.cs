using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerListWindow.xaml
/// </summary>
public partial class VolunteerListWindow : Window
{
    // Static instance of the business logic layer (BL)
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public VolunteerListWindow()
    {
        InitializeComponent();
    }

    // Dependency property for the VolunteerList, which is bound to the DataGrid in the XAML
    public IEnumerable<BO.VolunteerInList> VolunteerList
    {
        get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
        set { SetValue(VolunteerListProperty, value); }
    }

    // Registration of the dependency property for VolunteerList
    public static readonly DependencyProperty VolunteerListProperty =
        DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

    // Property to store the currently selected call type filter
    public BO.CallType callType { get; set; } = BO.CallType.None;

    // Event handler triggered when the call type ComboBox selection changes
    private void CallTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Updates the volunteer list based on the selected call type filter
        VolunteerList = helpReadAllVolunteer(callType);
    }

    // Refreshes the VolunteerList property with updated data
    private void RefreshVolunteerList()
    {
        VolunteerList = helpReadAllVolunteer(callType);
    }

    // Helper method to fetch the list of volunteers based on the selected call type filter
    private static IEnumerable<BO.VolunteerInList> helpReadAllVolunteer(BO.CallType callTypeHelp)
    {
        // Uses the ReadAll method from the BL to fetch the updated list
        return (callTypeHelp == BO.CallType.None)
            ? s_bl?.Volunteer.ReadAll(null, BO.FieldsVolunteerInList.Id, null)!
            : s_bl?.Volunteer.ReadAll(null, BO.FieldsVolunteerInList.Id, callTypeHelp)!;
    }

    // Observer to refresh the volunteer list whenever there are updates
    private void volunteerListObserver() => RefreshVolunteerList();

    // Adds the observer when the window is loaded
    private void Window_Loaded(object sender, RoutedEventArgs e)
        => s_bl.Volunteer.AddObserver(volunteerListObserver);

    // Removes the observer when the window is closed
    private void Window_Closed(object sender, EventArgs e)
        => s_bl.Volunteer.RemoveObserver(volunteerListObserver);

    // Event handler for selection change in the DataGrid
    private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
    {
        // Placeholder for additional functionality if needed
    }

    // Opens the VolunteerWindow to add a new volunteer when the Add button is clicked
    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerWindow().Show();
    }

    // Property to store the currently selected volunteer in the DataGrid
    public BO.VolunteerInList? SelectedVolunteer { get; set; }

    // Opens the VolunteerWindow for the selected volunteer when the user double-clicks a row in the DataGrid
    private void lsvVolunteersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedVolunteer != null)
            new VolunteerWindow(SelectedVolunteer.Id).Show();
    }

    // Deletes the selected volunteer when the Delete button is clicked
    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int volunteerId)
        {
            // Confirmation dialog to ensure the user wants to delete the volunteer
            var result = MessageBox.Show(
                "Are you sure you want to delete the volunteer?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                // User did not confirm the action
                return;
            }

            // Calls the Delete method from the BL to remove the volunteer
            s_bl.Volunteer.Delete(volunteerId);
        }
    }


}
