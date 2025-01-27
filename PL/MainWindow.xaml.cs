using PL.Call;
using PL.Volunteer;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL;
/// <summary>
/// Main window of the application, responsible for UI interactions and managing navigation between other windows.
/// </summary>
public partial class MainWindow : Window
{
    // Singleton instance of the BL API
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public MainWindow()
    {
        InitializeComponent(); // Initialize the UI components

    }

    /// <summary>
    /// Triggered when the text in the TextBox is changed.
    /// Currently, it is not implemented.
    /// </summary>
    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Placeholder for text change handling, currently unused
    }

    // DependencyProperty for CurrentTime, enables animation, styling, binding, etc.
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentTime
    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow), new PropertyMetadata(null));

    /// <summary>
    /// Advances the clock by one minute when the button is clicked.
    /// </summary>
    private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.Minute); // Advance the clock by one minute
            RefreshCallAmounts();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Advances the clock by one hour when the button is clicked.
    /// </summary>
    private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
    {
        try{s_bl.Admin.AdvanceClock(BO.TimeUnit.Hour); // Advance the clock by one hour
            RefreshCallAmounts();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Advances the clock by one day when the button is clicked.
    /// </summary>
    private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
    {
       try{ s_bl.Admin.AdvanceClock(BO.TimeUnit.Day); // Advance the clock by one day
            RefreshCallAmounts();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Advances the clock by one month when the button is clicked.
    /// </summary>
    private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
    {
        try{s_bl.Admin.AdvanceClock(BO.TimeUnit.Month); // Advance the clock by one month
            RefreshCallAmounts();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Advances the clock by one year when the button is clicked.
    /// </summary>
    private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
    {
       try{ s_bl.Admin.AdvanceClock(BO.TimeUnit.Year); // Advance the clock by one year
            RefreshCallAmounts();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // DependencyProperty for CurrentRiskRange, enables animation, styling, binding, etc.
    public TimeSpan CurrentRiskRange
    {
        get { return (TimeSpan)GetValue(CurrentRiskRangeProperty); }
        set { SetValue(CurrentRiskRangeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentRiskRange
    public static readonly DependencyProperty CurrentRiskRangeProperty =
        DependencyProperty.Register("CurrentRiskRange", typeof(TimeSpan), typeof(MainWindow), new PropertyMetadata(null));

    /// <summary>
    /// Updates the risk range based on user input when the button is clicked.
    /// </summary>
    private void btnUpdateRiskRange_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.SetRiskRange(CurrentRiskRange); // Update the risk range based on user input
        RefreshCallAmounts();
    }

    /// <summary>
    /// Updates the current time by getting it from the backend system.
    /// </summary>
    private void clockObserver()
    {
        CurrentTime = s_bl.Admin.GetClock(); // Get the current time from the backend
        RefreshCallAmounts();
    }

    /// <summary>
    /// Updates the current risk range by getting it from the backend system.
    /// </summary>
    private void configObserver()
    {
        CurrentRiskRange = s_bl.Admin.GetRiskRange(); // Get the current risk range from the backend
    }

    /// <summary>
    /// Called when the window is loaded. Initializes necessary values and sets up observers.
    /// </summary>
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Set initial values when the window is loaded
        CurrentTime = s_bl.Admin.GetClock(); // Set the current time from the backend
        CurrentRiskRange = s_bl.Admin.GetRiskRange(); // Set the current risk range from the backend
        RefreshCallAmounts(); // Ensure that this runs when the window is loaded
        s_bl.Admin.AddClockObserver(clockObserver); // Register for clock updates
        s_bl.Admin.AddConfigObserver(configObserver); // Register for configuration updates
        s_bl.Call.AddObserver(callAmountsObserver);
    }

    /// <summary>
    /// Cleans up observers when the window is closed to prevent memory leaks.
    /// </summary>
    private void MainWindow_Closed(object sender, EventArgs e)
    {
        // Cleanup observers when the window is closed
        s_bl.Admin.RemoveClockObserver(clockObserver);
        s_bl.Admin.RemoveConfigObserver(configObserver);
        s_bl.Call.RemoveObserver(callAmountsObserver);
    }

    /// <summary>
    /// Opens the VolunteerListWindow if it is not already open, otherwise brings the existing window to the front.
    /// </summary>
    private void btnVolunteers_Click(object sender, RoutedEventArgs e)
    {
        foreach (Window window in Application.Current.Windows)
        {
            // Check if there is already an open window of type VolunteerListWindow
            if (window is VolunteerListWindow)
            {
                window.Activate(); // If such a window is open, bring it to the front
                return; // If the window is already open, don't open a new one
            }
        }

        // If no window is open, open a new one
        new VolunteerListWindow().Show(); // Open the VolunteerListWindow when the button is clicked
    }

    /// <summary>
    /// Opens the CallListWindow if it is not already open, otherwise brings the existing window to the front.
    /// </summary>
    private void btnCalls_Click(object sender, RoutedEventArgs e)
    {
        foreach (Window window in Application.Current.Windows)
        {
            // Check if there is already an open window of type VolunteerListWindow
            if (window is CallListWindow)
            {
                window.Activate(); // If such a window is open, bring it to the front
                return; // If the window is already open, don't open a new one
            }
        }

        // If no window is open, open a new one
        new CallListWindow(null).Show(); // Open the VolunteerListWindow when the button is clicked
    }

    /// <summary>
    /// Resets the database after asking for user confirmation.
    /// </summary>
    private void btnReset_Click(object sender, RoutedEventArgs e)
    {
        // Ask for user confirmation before resetting the database
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
        // Change the mouse cursor to a waiting state
        Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
        try
        {
            // Close all windows except for the main window
            foreach (Window openWindow in Application.Current.Windows)
            {
                if (openWindow != this) // Keep the main window open
                {
                    openWindow.Close();
                }
            }
            // Call the Reset method in the BL to reset the database
            s_bl.Admin.Reset();
            RefreshCallAmounts();

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
            // Restore the mouse cursor to normal after the operation
            Mouse.OverrideCursor = null;
        }
    }

    /// <summary>
    /// Initializes the database after asking for user confirmation.
    /// </summary>
    private void btnIntialize_Click(object sender, RoutedEventArgs e)
    {
        // Ask for user confirmation before initializing the database
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
        // Change the mouse cursor to a waiting state
        Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
        try
        {
            // Close all windows except for the main window
            foreach (Window openWindow in Application.Current.Windows)
            {
                if (openWindow != this) // Keep the main window open
                {
                    openWindow.Close();
                }
            }
            // Call the Initialize method in the BL to initialize the database
            s_bl.Admin.Intialize();
            RefreshCallAmounts();

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
            // Restore the mouse cursor to normal after the operation
            Mouse.OverrideCursor = null;
        }
    }

    // DependencyProperty for CallAmounts, enables animation, styling, binding, etc.
    public int[] CallAmounts
    {
        get { return (int[])GetValue(CallAmountsProperty); }
        set { SetValue(CallAmountsProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CallAmounts
    public static readonly DependencyProperty CallAmountsProperty =
        DependencyProperty.Register("CallAmounts", typeof(int[]), typeof(MainWindow), new PropertyMetadata(null));

    /// <summary>
    /// Refreshes the call amounts by reading the latest data from the backend.
    /// </summary>
    private void RefreshCallAmounts()
    {
        CallAmounts = helpReadCallAmounts(); // Update the amounts from the data source
    }

    /// <summary>
    /// Helper function to read the current call amounts from the backend.
    /// </summary>
    private static int[] helpReadCallAmounts()
    {
        return s_bl.Call.CallsAmount().ToArray();
    }

    /// <summary>
    /// Observes changes in the call amounts and refreshes the displayed data.
    /// </summary>
    private void callAmountsObserver() => RefreshCallAmounts();

    /// <summary>
    /// Handles status clicks and opens a filtered call list window.
    /// </summary>
    private void OnStatusClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string status)
        {
            // Map the clicked status to the BO.Status enum or similar
            BO.Status? filterStatus = status switch
            {
                "Open" => BO.Status.open,
                "Treatment" => BO.Status.treatment,
                "Closed" => BO.Status.close,
                "Expired" => BO.Status.expired,
                "RiskTreatment" => BO.Status.riskTreatment,
                "RiskOpen" => BO.Status.riskOpen,
                _ => null
            };

            if (filterStatus.HasValue)
            {
                // Open a filtered call list window
                var window = new CallListWindow(filterStatus);
                window.Show();
            }
        }
    }
}