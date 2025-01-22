using Newtonsoft.Json.Linq;
using PL.Volunteer;
using System.Windows;
using System.Windows.Controls;

namespace PL;

/// <summary>
/// Interaction logic for LogInWindow.xaml
/// </summary>
public partial class LogInWindow : Window
{
    // Singleton instance of the BL API
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    /// <summary>
    /// Constructor for the LogInWindow.
    /// Initializes the components and sets the data context.
    /// </summary>
    public LogInWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    /// <summary>
    /// Property for the CurrentId value.
    /// </summary>
    public int CurrentId
    {
        get { return (int)GetValue(CurrentIdProperty); }
        set { SetValue(CurrentIdProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentId.  
    // This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentIdProperty =
        DependencyProperty.Register("CurrentId", typeof(int), typeof(LogInWindow), new PropertyMetadata(0));

    /// <summary>
    /// Property for the CurrentPassword value.
    /// </summary>
    public string CurrentPassword
    {
        get { return (string)GetValue(CurrentPasswordProperty); }
        set { SetValue(CurrentPasswordProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentPassword.  
    // This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentPasswordProperty =
        DependencyProperty.Register("CurrentPassword", typeof(string), typeof(LogInWindow), new PropertyMetadata(""));

    /// <summary>
    /// Handles the Log In button click event.
    /// Verifies the credentials and opens the corresponding window.
    /// </summary>
    private void btnLogIn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BO.RoleType role = s_bl.Volunteer.LogIn(CurrentId, CurrentPassword);
            if (role == BO.RoleType.manager)
            {
                // Setting LogInID and opening the ManagerChooseWindow
                PO.LogInID = CurrentId;
                new ManagerChooseWindow(CurrentId).Show();
            }
            else
            {
                // Opening the MainVolunteerWindow for regular users
                new MainVolunteerWindow(CurrentId).Show();
            }
        }
        catch (Exception ex)
        {
            // Show an error message if login fails
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Handles the TextChanged event for the password TextBox.
    /// Updates the CurrentPassword property with the entered password.
    /// </summary>
    private void TextBox_TextChangedpass(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            // Update CurrentPassword when the text changes
            CurrentPassword = textBox.Text;
        }
    }

    /// <summary>
    /// Handles the TextChanged event for the ID TextBox.
    /// Updates the CurrentId property with the entered ID.
    /// </summary>
    private void TextBox_TextChangedId(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            // Try parsing the entered text as an integer and update CurrentId
            int.TryParse(textBox.Text, out int id);
            CurrentId = id;
        }
    }
}
