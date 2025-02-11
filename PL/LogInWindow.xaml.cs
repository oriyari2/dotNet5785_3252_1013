using Newtonsoft.Json.Linq;
using PL.Volunteer;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL;

/// <summary>
/// Interaction logic for LogInWindow.xaml
/// </summary>
public partial class LogInWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public LogInWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    public int CurrentId
    {
        get { return (int)GetValue(CurrentIdProperty); }
        set { SetValue(CurrentIdProperty, value); }
    }

    public static readonly DependencyProperty CurrentIdProperty =
        DependencyProperty.Register("CurrentId", typeof(int), typeof(LogInWindow), new PropertyMetadata(0));

    public string CurrentPassword
    {
        get { return (string)GetValue(CurrentPasswordProperty); }
        set { SetValue(CurrentPasswordProperty, value); }
    }

    public static readonly DependencyProperty CurrentPasswordProperty =
        DependencyProperty.Register("CurrentPassword", typeof(string), typeof(LogInWindow), new PropertyMetadata(""));

    private void btnLogIn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            BO.RoleType role = s_bl.Volunteer.LogIn(CurrentId, CurrentPassword);
            if (role == BO.RoleType.manager)
            {
                PO.LogInID = CurrentId;
                new ManagerChooseWindow(CurrentId).Show();
            }
            else
            {
                new MainVolunteerWindow(CurrentId).Show();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            CurrentPassword = passwordBox.Password;
        }
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            btnLogIn_Click(btnLogIn, null);
        }
    }

    private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            btnLogIn_Click(btnLogIn, null);
        }
    }
}
