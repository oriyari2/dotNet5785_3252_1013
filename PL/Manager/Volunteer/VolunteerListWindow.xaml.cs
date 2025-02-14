﻿using PL.privateVolunteer;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerListWindow.xaml
/// </summary>
public partial class VolunteerListWindow : Window
{
    /// <summary>
    /// Static instance of the business logic layer (BL).
    /// </summary>
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private volatile DispatcherOperation? _observerOperation = null; //stage 7
    private volatile DispatcherOperation? _observerOperation2 = null; //stage 7


    /// <summary>
    /// Initializes a new instance of the <see cref="VolunteerListWindow"/> class.
    /// </summary>
    public VolunteerListWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Dependency property for the VolunteerList, which is bound to the DataGrid in the XAML.
    /// </summary>
    public IEnumerable<BO.VolunteerInList> VolunteerList
    {
        get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
        set { SetValue(VolunteerListProperty, value); }
    }

    /// <summary>
    /// Registration of the dependency property for VolunteerList.
    /// </summary>
    public static readonly DependencyProperty VolunteerListProperty =
        DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

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
        VolunteerList = helpReadAllVolunteer(callType);
    }

    /// <summary>
    /// Refreshes the VolunteerList property with updated data.
    /// </summary>
    private void RefreshVolunteerList()
    {
        VolunteerList = helpReadAllVolunteer(callType);
    }

    /// <summary>
    /// Helper method to fetch the list of volunteers based on the selected call type filter.
    /// </summary>
    /// <param name="callTypeHelp">The call type filter to apply.</param>
    /// <returns>List of volunteers matching the filter.</returns>
    private static IEnumerable<BO.VolunteerInList> helpReadAllVolunteer(BO.CallType callTypeHelp)
    {
        return (callTypeHelp == BO.CallType.None)
            ? s_bl?.Volunteer.ReadAll(null, BO.FieldsVolunteerInList.Id, null)!
            : s_bl?.Volunteer.ReadAll(null, BO.FieldsVolunteerInList.Id, callTypeHelp)!;
    }

    /// <summary>
    /// Observer to refresh the volunteer list whenever there are updates.
    /// </summary>
    private void volunteerListObserver()
{
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                RefreshVolunteerList();
            });
}
    /// <summary>
    /// Adds the observer when the window is loaded.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
{
        s_bl.Volunteer.AddObserver(volunteerListObserver);
        s_bl.Admin.AddClockObserver(clockObserver); // Register for clock updates
        s_bl.Admin.AddConfigObserver(clockObserver);

    }
    /// <summary>
    /// Removes the observer when the window is closed.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
{
        s_bl.Volunteer.RemoveObserver(volunteerListObserver);
        s_bl.Admin.RemoveClockObserver(clockObserver); // Register for clock updates
        s_bl.Admin.RemoveConfigObserver(clockObserver);
    }

    private void clockObserver()
    {
        if (_observerOperation2 is null || _observerOperation2.Status == DispatcherOperationStatus.Completed)
            _observerOperation2 = Dispatcher.BeginInvoke(() =>
            {
                RefreshVolunteerList();
            });
    }
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
        new VolunteerWindow().Show();
    }

    /// <summary>
    /// Property to store the currently selected volunteer in the DataGrid.
    /// </summary>
    public BO.VolunteerInList? SelectedVolunteer { get; set; }

    /// <summary>
    /// Opens the VolunteerWindow for the selected volunteer when the user double-clicks a row in the DataGrid.
    /// </summary>
    private void lsvVolunteersList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedVolunteer != null)
            new VolunteerWindow(SelectedVolunteer.Id).Show();
    }

    /// <summary>
    /// Deletes the selected volunteer when the Delete button is clicked.
    /// Shows a confirmation dialog before performing the delete operation.
    /// </summary>
    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int volunteerId)
        {
            var result = MessageBox.Show(
                "Are you sure you want to delete the volunteer?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                // Close all related windows before deleting the volunteer
                var windowsToClose = Application.Current.Windows
                 .OfType<Window>()
                 .Where(w =>
                     w is MainVolunteerWindow mvw && mvw.CurrentVolunteer.Id == volunteerId ||
                     w is SelectCallWindow scw && scw.CurrentVolunteer.Id == volunteerId ||
                     w is CallHistoryWindow chw && chw.CurrentVolunteer.Id == volunteerId ||
                     w is VolunteerWindow vw && vw.CurrentVolunteer.Id == volunteerId).ToList();

                foreach (var window in windowsToClose)
                {
                    window.Close();
                }

                // Delete the volunteer from the database
                s_bl.Volunteer.Delete(volunteerId);

                MessageBox.Show("Volunteer deleted successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
