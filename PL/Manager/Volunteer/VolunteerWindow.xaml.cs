using System;
using System.Windows;
using System.Windows.Threading;

namespace PL.Volunteer;

/// <summary>
/// Code-behind logic for the VolunteerWindow.xaml file.
/// Manages the UI interactions and binds data to the ViewModel.
/// </summary>
public partial class VolunteerWindow : Window
{
    /// <summary>
    /// Static instance of the business logic layer (BL).
    /// </summary>
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private volatile DispatcherOperation? _observerOperation = null; //stage 7
    private volatile DispatcherOperation? _observerOperation2 = null; //stage 7

    /// <summary>
    /// Dependency property for the button text ("Add" or "Update").
    /// </summary>
    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow));

    /// <summary>
    /// Gets or sets the text for the action button.
    /// </summary>
    public string ButtonText
    {
        get { return (string)GetValue(ButtonTextProperty); }
        set { SetValue(ButtonTextProperty, value); }
    }

    /// <summary>
    /// Constructor for the VolunteerWindow, with an optional volunteer ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to load. Defaults to 0 for adding a new volunteer.</param>
    public VolunteerWindow(int id = 0)
    {
        // Set the button text based on whether adding or updating a volunteer.
        ButtonText = id == 0 ? "Add" : "Update";
        InitializeComponent();

        // Set the data context for data binding.
        DataContext = this;

        // Initialize the CurrentVolunteer property: fetch an existing volunteer if an ID is provided,
        // or create a new volunteer object for adding.
        try
        {
            CurrentVolunteer = (id != 0) ? s_bl.Volunteer.Read(id)! : new BO.Volunteer();
        }
        catch (Exception ex)
        {
            // Show an error message if initialization fails.
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Dependency property for the current volunteer being managed.
    /// </summary>
    public BO.Volunteer CurrentVolunteer
    {
        get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
        set { SetValue(CurrentVolunteerProperty, value); }
    }

    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

    /// <summary>
    /// Event handler for the Add/Update button click.
    /// </summary>
    private void ButtonAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer == null)
        {
            // Show an error message if volunteer details are missing.
            MessageBox.Show("Volunteer details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            if (ButtonText == "Add")
            {
                // Add a new volunteer using the business logic layer.
                s_bl.Volunteer.Create(CurrentVolunteer);
                MessageBox.Show("Volunteer added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Update the existing volunteer details.
                s_bl.Volunteer.Update(PO.LogInID, CurrentVolunteer);
                MessageBox.Show("Volunteer updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Close the window after a successful operation.
            Close();
        }
        catch (Exception ex)
        {
            // Show an error message if something goes wrong.
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Re-fetch the volunteer's details from the BL to ensure they are up-to-date.
    /// </summary>
    private void queryVolunteer()
    {
        int id = CurrentVolunteer!.Id;
        CurrentVolunteer = null;
        CurrentVolunteer = s_bl.Volunteer.Read(id);
    }

    /// <summary>
    /// Observer method to refresh the volunteer data when notified of changes.
    /// </summary>
    private void volunteerObserver()
{
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                queryVolunteer();
            });
}
    /// <summary>
    /// Event handler for when the window is loaded.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer!.Id != 0)
            // Add an observer for the current volunteer if it exists.
            s_bl.Volunteer.AddObserver(CurrentVolunteer!.Id, volunteerObserver);
        s_bl.Admin.AddClockObserver(clockObserver); // Register for clock updates

    }

    /// <summary>
    /// Event handler for when the window is closed.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
        if (CurrentVolunteer!.Id != 0)
            // Remove the observer for the current volunteer when the window is closed.
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer!.Id, volunteerObserver);
        s_bl.Admin.RemoveClockObserver(clockObserver); // Register for clock updates
    }

    private void clockObserver()
    {
        if (_observerOperation2 is null || _observerOperation2.Status == DispatcherOperationStatus.Completed)
            _observerOperation2 = Dispatcher.BeginInvoke(() =>
            {
                queryVolunteer();
            });
    }
}
