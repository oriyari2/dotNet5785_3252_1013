
using PL.Volunteer;
using System.Windows;
using System.Windows.Controls;

namespace PL;

/// <summary>
/// Interaction logic for LogInWindow.xaml
/// </summary>
public partial class LogInWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Singleton instance of the BL API
    public LogInWindow()
    {
        InitializeComponent();
        DataContext = this;
    }
    public int CurrentId
    {
        get { return (int)GetValue(CurrentIdProperty); }
        set { SetValue(CurrentIdProperty, value); 
            PO.LogInID = value;}
    }

    // Using a DependencyProperty as the backing store for CurrentId.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentIdProperty =
        DependencyProperty.Register("CurrentId", typeof(int), typeof(LogInWindow), new PropertyMetadata(0));



    public string CurrentPassword
    {
        get { return (string)GetValue(CurrentPasswordProperty); }
        set { SetValue(CurrentPasswordProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentPassword.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentPasswordProperty =
        DependencyProperty.Register("CurrentPassword", typeof(string), typeof(LogInWindow), new PropertyMetadata(""));

    private void btnLogIn_Click(object sender, RoutedEventArgs e)
    {
        try 
        { 
            BO.RoleType role = s_bl.Volunteer.LogIn(PO.LogInID, CurrentPassword);
            if(role==BO.RoleType.manager)
            {
                foreach (Window window in Application.Current.Windows)
                {
                    // Check if there is already an open window of type VolunteerListWindow
                    if (window is MainWindow)
                    {
                        window.Activate(); // If such a window is open, bring it to the front
                        return; // If the window is already open, don't open a new one
                    }
                }
                new MainWindow().Show(); // Open the VolunteerListWindow when the button is clicked
            }

        }
        catch (Exception ex)
        {
            /// <summary>
            /// Show an error message if initialization fails.
            /// </summary>
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void TextBox_TextChangedpass(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            CurrentPassword = textBox.Text;
        }
    }
    private void TextBox_TextChangedId(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            int.TryParse(textBox.Text, out int id);
            CurrentId = id;
        }
    }

    
}

