using PL.Call;
using PL.privateVolunteer;
using System.Windows;
using System.Windows.Controls;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainVolunteerWindow.xaml
    /// </summary>
    public partial class MainVolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Singleton instance of the BL API

        /// <summary>
        /// Constructor that initializes the MainVolunteerWindow with optional volunteer ID.
        /// </summary>
        public MainVolunteerWindow(int id = 0)
        {
            InitializeComponent();
            DataContext = this;
            try
            {
                CurrentVolunteer = s_bl.Volunteer.Read(id); // Retrieve volunteer details
                if (CurrentVolunteer.IsProgress != null)
                {
                    CurrentCall = s_bl.Call.Read(CurrentVolunteer.IsProgress.CallId); // Retrieve current call if progress exists
                }
                else
                    CurrentCall = null;
            }
            catch (Exception ex)
            {
                // Show an error message if initialization fails.
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Gets or sets the current volunteer.
        /// </summary>
        public BO.Volunteer CurrentVolunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentVolunteer. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(MainVolunteerWindow), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the current call associated with the volunteer.
        /// </summary>
        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentCall. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(MainVolunteerWindow), new PropertyMetadata(null));

        /// <summary>
        /// Handles the click event of the "Update" button to update the volunteer's information.
        /// </summary>
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer == null)
            {
                // Show an error message if volunteer details are missing.
                MessageBox.Show("Volunteer details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                s_bl.Volunteer.Update(CurrentVolunteer.Id, CurrentVolunteer); // Update volunteer information
                MessageBox.Show("Volunteer updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Show an error message if something goes wrong during update.
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the click event of the "Cancel Treatment" button to cancel the volunteer's treatment for the current call.
        /// </summary>
        private void BtnCancelTreatment_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to cancel your treatment for this call?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                s_bl.Call.CancelTreatment(CurrentVolunteer.Id, CurrentVolunteer.IsProgress.Id); // Cancel the treatment for the current call
                MessageBox.Show("Call canceled successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Show an error message if something goes wrong during cancellation.
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the click event of the "End Treatment" button to end the volunteer's treatment for the current call.
        /// </summary>
        private void BtnEndTreatmrnt_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to end your treatment for this call?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                s_bl.Call.EndTreatment(CurrentVolunteer.Id, CurrentVolunteer.IsProgress.Id); // End treatment for the current call
                MessageBox.Show("Call ended successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Show an error message if something goes wrong during ending treatment.
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the click event of the "Calls History" button to open the call history window for the volunteer.
        /// </summary>
        private void BtnCallsHistory_Click(object sender, RoutedEventArgs e)
        {
            new CallHistoryWindow(CurrentVolunteer.Id).Show(); // Open the call history window for the current volunteer
        }

        /// <summary>
        /// Handles the click event of the "Select Call" button to open the call selection window for the volunteer.
        /// </summary>
        private void BtnSelectCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if there's already an open window for the current volunteer
                var existingWindow = Application.Current.Windows
                    .OfType<SelectCallWindow>()
                    .FirstOrDefault(w => w.CurrentVolunteer.Id == CurrentVolunteer.Id);

                if (existingWindow != null)
                {
                    // Focus on the existing window
                    existingWindow.Focus();
                }
                else
                {
                    // Create and show a new window
                    var newWindow = new SelectCallWindow(CurrentVolunteer.Id);
                    newWindow.Show();
                }
            }
            catch (Exception ex)
            {
                // Show an error message if something goes wrong during call selection.
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Event handler for when the window is loaded to set initial values and add observers.
        /// </summary>
        private void MainVolunteerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Set initial values when the window is loaded
            RefreshCallInProgress();
            s_bl.Call.AddObserver(callInProgressObserver); // Add observer to monitor call progress
            s_bl.Volunteer.AddObserver(VolunteerObserver); // Add observer to monitor volunteer progress
        }

        /// <summary>
        /// Cleans up observers when the window is closed to prevent memory leaks.
        /// </summary>
        private void MainVolunteerWindow_Closed(object sender, EventArgs e)
        {
            // Cleanup observers when the window is closed
            s_bl.Call.RemoveObserver(callInProgressObserver);
            s_bl.Volunteer.RemoveObserver(VolunteerObserver);
        }

        /// <summary>
        /// Refreshes the current call information for the volunteer.
        /// </summary>
        private void RefreshCallInProgress()
        {
            var newCall = helpReadCallInProgress();
            if (newCall?.status == BO.Status.expired)
            {
                CurrentCall = null;
                RefreshVolunteer();
            }
            else
            {
                CurrentCall = newCall;
            }
        }

        /// <summary>
        /// Helper function to read the current call information from the backend.
        /// </summary>
        private BO.Call? helpReadCallInProgress()
        {
            try
            {
                var volunteer = s_bl.Volunteer.Read(CurrentVolunteer.Id);
                if (volunteer.IsProgress != null)
                {
                    var call = s_bl.Call.Read(volunteer.IsProgress.CallId);
                    // אם הקריאה פגת תוקף, נחזיר null
                    if (call.status == BO.Status.expired)
                        return null;
                    return call;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Observes changes in the call progress and refreshes the call data.
        /// </summary>
        private void callInProgressObserver() => RefreshCallInProgress();

        /// <summary>
        /// Refreshes the volunteer's information from the backend.
        /// </summary>
        private void RefreshVolunteer()
        {
            CurrentVolunteer = helpReadVolunteer(); // Update the volunteer data from the backend
        }

        /// <summary>
        /// Helper function to read the current volunteer information from the backend.
        /// </summary>
        private BO.Volunteer helpReadVolunteer()
        {
            return s_bl.Volunteer.Read(CurrentVolunteer.Id); // Return the volunteer details
        }

        /// <summary>
        /// Observes changes in the volunteer's progress and refreshes the volunteer data.
        /// </summary>
        private void VolunteerObserver() => RefreshVolunteer();
    }
}
