using System;
using System.Windows;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for the VolunteerWindow.xaml
/// </summary>
public partial class VolunteerWindow : Window
{
    /// <summary>
    /// Static instance of the business logic layer (BL).
    /// </summary>
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

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
        /// <summary>
        /// Set the button text based on whether adding or updating a volunteer.
        /// </summary>
        ButtonText = id == 0 ? "Add" : "Update";
        InitializeComponent();

        /// <summary>
        /// Set the data context for data binding.
        /// </summary>
        DataContext = this;

        /// <summary>
        /// Initialize the CurrentVolunteer property: fetch an existing volunteer if an ID is provided,
        /// or create a new volunteer object for adding.
        /// </summary>
        try
        {
            CurrentVolunteer = (id != 0) ? s_bl.Volunteer.Read(id)! : new BO.Volunteer();
        }
        catch (Exception ex)
        {
            /// <summary>
            /// Show an error message if initialization fails.
            /// </summary>
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
            /// <summary>
            /// Show an error message if volunteer details are missing.
            /// </summary>
            MessageBox.Show("Volunteer details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            if (ButtonText == "Add")
            {
                /// <summary>
                /// Add a new volunteer using the business logic layer.
                /// </summary>
                s_bl.Volunteer.Create(CurrentVolunteer);
                MessageBox.Show("Volunteer added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                /// <summary>
                /// Update the existing volunteer details.
                /// </summary>
                s_bl.Volunteer.Update(CurrentVolunteer.Id, CurrentVolunteer);
                MessageBox.Show("Volunteer updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            /// <summary>
            /// Close the window after a successful operation.
            /// </summary>
            Close();
        }
        catch (Exception ex)
        {
            /// <summary>
            /// Show an error message if something goes wrong.
            /// </summary>
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
        => queryVolunteer();

    /// <summary>
    /// Event handler for when the window is loaded.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer!.Id != 0)
            /// <summary>
            /// Add an observer for the current volunteer if it exists.
            /// </summary>
            s_bl.Volunteer.AddObserver(CurrentVolunteer!.Id, volunteerObserver);
    }

    /// <summary>
    /// Event handler for when the window is closed.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
        if (CurrentVolunteer!.Id != 0)
            /// <summary>
            /// Remove the observer for the current volunteer when the window is closed.
            /// </summary>
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer!.Id, volunteerObserver);
    }
}
