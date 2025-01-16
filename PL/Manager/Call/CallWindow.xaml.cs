using PL.Volunteer;
using System.Windows;
namespace PL.Call;

/// <summary>
/// Interaction logic for CallWindow.xaml
/// </summary>
public partial class CallWindow : Window
{
    
    /// <summary>
    /// Static instance of the business logic layer (BL).
    /// </summary>
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public CallWindow(int callId = 0)
    {
       // Set the button text based on whether adding or updating a volunteer.
        
        ButtonText = callId == 0 ? "Add" : "Update";
        InitializeComponent();
        // Set the data context for data binding.
        DataContext = this;

        
        /// Initialize the CurrentVolunteer property: fetch an existing volunteer if an ID is provided,
        /// or create a new volunteer object for adding.
        
        try
        {
            CurrentCall = (callId != 0) ? s_bl.Call.Read(callId)! : new BO.Call();
        }
        catch (Exception ex)
        {
            
            // Show an error message if initialization fails.
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Dependency property for the button text ("Add" or "Update").
    /// </summary>
   

    /// <summary>
    /// Gets or sets the text for the action button.
    /// </summary>
    public string ButtonText
    {
        get { return (string)GetValue(ButtonTextProperty); }
        set { SetValue(ButtonTextProperty, value); }
    }

    public static readonly DependencyProperty ButtonTextProperty =
       DependencyProperty.Register("ButtonText", typeof(string), typeof(CallWindow));

    /// <summary>
    /// Constructor for the VolunteerWindow, with an optional volunteer ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to load. Defaults to 0 for adding a new volunteer.</param>


    /// <summary>
    /// Dependency property for the current volunteer being managed.
    /// </summary>
    public BO.Call CurrentCall
    {
        get { return (BO.Call)GetValue(CurrentCallProperty); }
        set { SetValue(CurrentCallProperty, value); }
    }

    public static readonly DependencyProperty CurrentCallProperty =
        DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

    /// <summary>
    /// Event handler for the Add/Update button click.
    /// </summary>
    private void ButtonAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentCall == null)
        {
            /// <summary>
            /// Show an error message if volunteer details are missing.
            /// </summary>
            MessageBox.Show("Call details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            if (ButtonText == "Add")
            {
                /// <summary>
                /// Add a new volunteer using the business logic layer.
                /// </summary>
                CurrentCall.OpeningTime = s_bl.Admin.GetClock();
                s_bl.Call.Create(CurrentCall);
                MessageBox.Show("Call added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                /// <summary>
                /// Update the existing volunteer details.
                /// </summary>
                s_bl.Call.Update(CurrentCall);
                MessageBox.Show("Call updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }

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
    private void queryCall()
    {
        int id = CurrentCall!.Id;
       // CurrentCall = null;
        CurrentCall = s_bl.Call.Read(id);
    }

    /// <summary>
    /// Observer method to refresh the call data when notified of changes.
    /// </summary>
    private void callObserver()
        => queryCall();

    /// <summary>
    /// Event handler for when the window is loaded.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (CurrentCall!.Id != 0)
            /// <summary>
            /// Add an observer for the current volunteer if it exists.
            /// </summary>
            s_bl.Call.AddObserver(CurrentCall!.Id, callObserver);
    }

    /// <summary>
    /// Event handler for when the window is closed.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
        if (CurrentCall!.Id != 0)
            /// <summary>
            /// Remove the observer for the current volunteer when the window is closed.
            /// </summary>
            s_bl.Call.RemoveObserver(CurrentCall!.Id, callObserver);
    }

}
