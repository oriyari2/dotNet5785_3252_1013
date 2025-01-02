using System;
using System.Windows;

namespace PL.Volunteer;

public partial class VolunteerWindow : Window
{
    // Static instance of the business logic layer (BL)
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    // Dependency property for the button text ("Add" or "Update")
    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow));

    public string ButtonText
    {
        get { return (string)GetValue(ButtonTextProperty); }
        set { SetValue(ButtonTextProperty, value); }
    }

    // Constructor for the window, with optional volunteer ID
    public VolunteerWindow(int id = 0)
    {
        // Set the button text based on whether we are adding or updating a volunteer
        ButtonText = id == 0 ? "Add" : "Update";
        InitializeComponent();

        // Set the data context for data binding
        DataContext = this;

        // Initialize the CurrentVolunteer property: fetch an existing volunteer if an ID is provided,
        // or create a new volunteer object for adding a new one
        try
        {
            CurrentVolunteer = (id != 0) ? s_bl.Volunteer.Read(id)! : new BO.Volunteer();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }

    // Dependency property for the current volunteer being managed in this window
    public BO.Volunteer CurrentVolunteer
    {
        get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
        set { SetValue(CurrentVolunteerProperty, value); }
    }

    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

    // Event handler for the Add/Update button click
    private void ButtonAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer == null)
        {
            // Show an error message if volunteer details are missing
            MessageBox.Show("Volunteer details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            if (ButtonText == "Add")
            {
                // Add a new volunteer using the business logic layer
                s_bl.Volunteer.Create(CurrentVolunteer);
                MessageBox.Show("Volunteer added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                // Update the existing volunteer details
                s_bl.Volunteer.Update(CurrentVolunteer.Id, CurrentVolunteer);
                MessageBox.Show("Volunteer updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Close the window after a successful operation
            Close();
        }
        catch (Exception ex)
        {
            // Show an error message if something goes wrong
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Re-fetch the volunteer's details from the BL to ensure they are up-to-date
    private void queryVolunteer()
    {
        int id = CurrentVolunteer!.Id;
        CurrentVolunteer = null;
        CurrentVolunteer = s_bl.Volunteer.Read(id);
    }

    // Observer method to refresh the volunteer data when notified of changes
    private void volunteerObserver()
        => queryVolunteer();

    // Event handler for when the window is loaded
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer!.Id != 0)
            // Add an observer for the current volunteer if it exists
            s_bl.Volunteer.AddObserver(CurrentVolunteer!.Id, volunteerObserver);
    }

    // Event handler for when the window is closed
    private void Window_Closed(object sender, EventArgs e)
    {
        if (CurrentVolunteer!.Id != 0)
        // Remove the observer for the current volunteer when the window is closed
        {
            s_bl.Volunteer.RemoveObserver(CurrentVolunteer!.Id, volunteerObserver);
        }
    }
}
