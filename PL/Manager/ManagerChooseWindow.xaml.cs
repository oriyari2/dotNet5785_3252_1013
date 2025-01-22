using System.Windows;

namespace PL;

/// <summary>
/// Interaction logic for ManagerChooseWindow.xaml
/// This window allows the user to choose between Manager and Volunteer views.
/// </summary>
public partial class ManagerChooseWindow : Window
{
    /// <summary>
    /// Constructor for ManagerChooseWindow.
    /// Initializes the window and sets the current ID if provided.
    /// </summary>
    /// <param name="id2">The ID of the current user (default is 0).</param>
    public ManagerChooseWindow(int id2 = 0)
    {
        InitializeComponent();
        CurrentId = id2; // Set the current user ID
    }

    /// <summary>
    /// Event handler for the Manager button click.
    /// Opens the MainWindow if not already open.
    /// </summary>
    private void btnManagerWindow_Click(object sender, RoutedEventArgs e)
    {
        foreach (Window window in Application.Current.Windows)
        {
            // Check if there is already an open window of type MainWindow
            if (window is MainWindow)
            {
                window.Activate(); // Bring the existing window to the front
                return; // Exit to avoid opening a new window
            }
        }

        // Open a new MainWindow if none is open
        new MainWindow().Show();
    }

    /// <summary>
    /// Gets or sets the current user ID.
    /// </summary>
    public int CurrentId
    {
        get { return (int)GetValue(CurrentIdProperty); }
        set { SetValue(CurrentIdProperty, value); }
    }

    /// <summary>
    /// Dependency property for the CurrentId.
    /// Enables binding, styling, and other WPF features.
    /// </summary>
    public static readonly DependencyProperty CurrentIdProperty =
        DependencyProperty.Register("CurrentId", typeof(int), typeof(ManagerChooseWindow), new PropertyMetadata(0));

    /// <summary>
    /// Event handler for the Volunteer button click.
    /// Opens the MainVolunteerWindow, passing the current user ID.
    /// </summary>
    private void btnVolunteerWindow_Click(object sender, RoutedEventArgs e)
    {
        new MainVolunteerWindow(CurrentId).Show();
    }
}
