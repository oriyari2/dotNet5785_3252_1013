using PL.Volunteer;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Singleton instance of the BL API
    public MainWindow()
    {
        InitializeComponent(); // Initialize the UI components
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // This function is triggered when the text in the TextBox is changed
        // Currently, it is not implemented.
    }

    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentTime. This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow), new PropertyMetadata(null));

    private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BO.TimeUnit.Minute); // Advance the clock by one minute
    }
    private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BO.TimeUnit.Hour); // Advance the clock by one hour
    }
    private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BO.TimeUnit.Day); // Advance the clock by one day
    }
    private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BO.TimeUnit.Month); // Advance the clock by one month
    }
    private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.AdvanceClock(BO.TimeUnit.Year); // Advance the clock by one year
    }

    public TimeSpan CurrentRiskRange
    {
        get { return (TimeSpan)GetValue(CurrentRiskRangeProperty); }
        set { SetValue(CurrentRiskRangeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentRiskRange. This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentRiskRangeProperty =
        DependencyProperty.Register("CurrentRiskRange", typeof(TimeSpan), typeof(MainWindow), new PropertyMetadata(null));

    private void btnUpdateRiskRange_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.SetRiskRange(CurrentRiskRange); // Update the risk range based on user input
    }

    private void clockObserver() 
    {
        CurrentTime = s_bl.Admin.GetClock(); // Get the current time from the backend
    }

    private void configObserver() 
    {
        CurrentRiskRange = s_bl.Admin.GetRiskRange(); // Get the current risk range from the backend
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Set initial values when the window is loaded
        CurrentTime = s_bl.Admin.GetClock(); // Set the current time from the backend
        CurrentRiskRange = s_bl.Admin.GetRiskRange(); // Set the current risk range from the backend
        s_bl.Admin.AddClockObserver(clockObserver); // Register for clock updates
        s_bl.Admin.AddConfigObserver(configObserver); // Register for configuration updates
    }

    private void MainWindow_Closed(object sender, EventArgs e)
    {
        // Cleanup observers when the window is closed
        s_bl.Admin.RemoveClockObserver(clockObserver);
        s_bl.Admin.RemoveConfigObserver(configObserver);
    }

    private void btnVolunteers_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerListWindow().Show(); // Open the VolunteerListWindow when the button is clicked
    }

    private void btnReset_Click(object sender, RoutedEventArgs e)
    {
        // 1. Ask for user confirmation before resetting the database
        var result = MessageBox.Show(
            "Are you sure you want to reset the database?",
            "Confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
        if (result != MessageBoxResult.Yes)
        {
            // User did not confirm the action, return early
            return;
        }
        // 2. Change the mouse cursor to a waiting state
        Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
        try
        {
            // 3. Close all windows except for the main window
            foreach (Window openWindow in Application.Current.Windows)
            {
                if (openWindow != this) // Keep the main window open
                {
                    openWindow.Close();
                }
            }
            // 4. Call the Reset method in the BL to reset the database
            s_bl.Admin.Reset();

            // Success message after resetting the database
            MessageBox.Show("Database has been successfully reset.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            // Handle any errors that occur (e.g., database access issues)
            MessageBox.Show($"An error occurred while resetting the database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            // 5. Restore the mouse cursor to normal after the operation
            Mouse.OverrideCursor = null;
        }
    }

    private void btnIntialize_Click(object sender, RoutedEventArgs e)
    {
        // 1. Ask for user confirmation before initializing the database
        var result = MessageBox.Show(
            "Are you sure you want to initialize the database?",
            "Confirmation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            // User did not confirm the action, return early
            return;
        }
        // 2. Change the mouse cursor to a waiting state
        Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
        try
        {
            // 3. Close all windows except for the main window
            foreach (Window openWindow in Application.Current.Windows)
            {
                if (openWindow != this) // Keep the main window open
                {
                    openWindow.Close();
                }
            }
            // 4. Call the Initialize method in the BL to initialize the database
            s_bl.Admin.Intialize();

            // Success message after initializing the database
            MessageBox.Show("Database has been successfully initialized.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            // Handle any errors that occur during initialization
            MessageBox.Show($"An error occurred while initializing the database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            // 5. Restore the mouse cursor to normal after the operation
            Mouse.OverrideCursor = null;
        }
    }
}
