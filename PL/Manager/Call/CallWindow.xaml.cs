using PL.Volunteer;
using System.Windows;
using System.Windows.Threading;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallWindow : Window
    {
        /// <summary>
        /// Static instance of the business logic layer (BL).
        /// </summary>
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        private volatile DispatcherOperation? _observerOperation2 = null; //stage 7

        /// <summary>
        /// Constructor for the CallWindow, with an optional call ID.
        /// </summary>
        /// <param name="callId">The ID of the call to load. Defaults to 0 for adding a new call.</param>
        public CallWindow(int callId = 0)
        {
            // Set the button text based on whether adding or updating a call.
            ButtonText = callId == 0 ? "Add" : "Update";
            InitializeComponent();

            // Set the data context for data binding.
            DataContext = this;

            try
            {
                // Initialize the CurrentCall property: fetch an existing call if an ID is provided,
                // or create a new call object for adding.
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
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextProperty =
           DependencyProperty.Register("ButtonText", typeof(string), typeof(CallWindow));

        /// <summary>
        /// Dependency property for the current call being managed.
        /// </summary>
        public BO.Call CurrentCall
        {
            get { return (BO.Call)GetValue(CurrentCallProperty); }
            set
            {
                SetValue(CurrentCallProperty, value);
                // Force UI update
                if (value != null)
                {
                    var latitude = value.Latitude;
                    var longitude = value.Longitude;
                    SetValue(CurrentCallProperty, value);
                }
            }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

        /// <summary>
        /// Dependency property for the current time.
        /// </summary>
        public DateTime CurrentTime //for the opening time or curent time
        {
            get { return (DateTime)GetValue(CurrentOpeningTimeProperty); }
            set { SetValue(CurrentOpeningTimeProperty, value); }
        }

        public static readonly DependencyProperty CurrentOpeningTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(CallWindow), new PropertyMetadata(null));



        /// <summary>
        /// Observer method to update the current time from the backend clock.
        /// </summary>
        private void clockObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    CurrentTime = ButtonText == "Update" ? CurrentCall.OpeningTime : s_bl.Admin.GetClock(); // Get the current time from the backend
                    queryCall();
                });
        }

        /// <summary>
        /// Event handler for the Add/Update button click.
        /// </summary>
        private void ButtonAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentCall == null)
            {
                // Show an error message if call details are missing.
                MessageBox.Show("Call details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                if (ButtonText == "Add")
                {
                    // Add a new call using the business logic layer.
                    CurrentCall.OpeningTime = s_bl.Admin.GetClock();
                    s_bl.Call.Create(CurrentCall);
                    MessageBox.Show("Call added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Update the existing call details.
                    s_bl.Call.Update(CurrentCall);
                    MessageBox.Show("Call updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Close();
            }
            catch (Exception ex)
            {
                // Show an error message if something goes wrong.
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Re-fetch the call's details from the BL to ensure they are up-to-date.
        /// </summary>
        private void queryCall()
        {
            int id = CurrentCall!.Id;
            if(id!=0) 
                CurrentCall = s_bl.Call.Read(id);

        }

        /// <summary>
        /// Observer method to refresh the call data when notified of changes.
        /// </summary>
        private void callObserver()
{
            if (_observerOperation2 is null || _observerOperation2.Status == DispatcherOperationStatus.Completed)
                _observerOperation2 = Dispatcher.BeginInvoke(() =>
                {
                    queryCall();
                });

}
        /// <summary>
        /// Event handler for when the window is loaded.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentCall!.Id != 0)
                // Add an observer for the current call if it exists.
                s_bl.Call.AddObserver(CurrentCall!.Id, callObserver);

           CurrentTime = ButtonText == "Update" ? CurrentCall.OpeningTime : s_bl.Admin.GetClock(); // Get the current time from the backend
            s_bl.Admin.AddClockObserver(clockObserver); // Register for clock updates
        }

        /// <summary>
        /// Event handler for when the window is closed.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            if (CurrentCall!.Id != 0)
                // Remove the observer for the current call when the window is closed.
                s_bl.Call.RemoveObserver(CurrentCall!.Id, callObserver);

            s_bl.Admin.RemoveClockObserver(clockObserver);
        }
    }
}
