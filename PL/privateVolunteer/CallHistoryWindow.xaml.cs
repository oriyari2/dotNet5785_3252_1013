﻿using PL.Call;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PL.privateVolunteer;

/// <summary>
/// Represents the CallHistoryWindow, which displays a list of closed calls for a specific volunteer.
/// </summary>
public partial class CallHistoryWindow : Window
{
    // Static instance of the BL interface.
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    private volatile DispatcherOperation? _observerOperation = null; //stage 7
    private volatile DispatcherOperation? _observerOperation2 = null; //stage 7

    /// <summary>
    /// Initializes a new instance of the CallHistoryWindow class.
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    public CallHistoryWindow(int id)
    {
        InitializeComponent();
        DataContext = this;
        try
        {
            CurrentVolunteer = s_bl.Volunteer.Read(id);
            RefreshCallList();
        }
        catch (Exception ex)
        {
            // Show an error message if initialization fails.
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Dependency property to store the current volunteer's details.
    /// </summary>
    public BO.Volunteer CurrentVolunteer
    {
        get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
        set { SetValue(CurrentVolunteerProperty, value); }
    }

    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(CallHistoryWindow), new PropertyMetadata(null));

    /// <summary>
    /// Dependency property to store the list of closed calls.
    /// </summary>
    public IEnumerable<BO.ClosedCallInList> CallList
    {
        get { return (IEnumerable<BO.ClosedCallInList>)GetValue(CallListProperty); }
        set { SetValue(CallListProperty, value); }
    }

    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register("CallList", typeof(IEnumerable<BO.ClosedCallInList>), typeof(CallHistoryWindow), new PropertyMetadata(null));

    // Property to store the selected call type filter.
    public BO.CallType callType { get; set; } = BO.CallType.None;

    /// <summary>
    /// Refreshes the CallList property with updated data.
    /// </summary>
    private void RefreshCallList()
    {
        CallList = helpReadAllCall(callType);
    }

    /// <summary>
    /// Helper method to fetch the list of calls based on the selected call type filter.
    /// </summary>
    /// <param name="callTypeHelp">The call type filter to apply.</param>
    /// <returns>List of calls matching the filter.</returns>
    private IEnumerable<BO.ClosedCallInList> helpReadAllCall(BO.CallType callTypeHelp)
    {
        return (callTypeHelp == BO.CallType.None)
            ? s_bl?.Call.GetClosedCallInList(CurrentVolunteer.Id, null, BO.FieldsClosedCallInList.ActualEndTime)!
            : s_bl?.Call.GetClosedCallInList(CurrentVolunteer.Id, callTypeHelp, BO.FieldsClosedCallInList.ActualEndTime)!;
    }

    /// <summary>
    /// Event handler triggered when the call type ComboBox selection changes.
    /// Updates the call list based on the selected call type filter.
    /// </summary>
    private void CallTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Update the selected call type based on the ComboBox selection
        if (sender is ComboBox comboBox && comboBox.SelectedItem is BO.CallType selectedType)
        {
            callType = selectedType; // Update the selected call type
            RefreshCallList(); // Refresh the call list with the new filter
        }
    }

    /// <summary>
    /// Adds the observer when the window is loaded.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        s_bl.Call.AddObserver(CallListObserver);
        s_bl.Admin.AddClockObserver(clockObserver); // Register for clock updates
        s_bl.Admin.AddConfigObserver(clockObserver);
        RefreshCallList();
    }

    /// <summary>
    /// Removes the observer when the window is closed.
    /// </summary>
    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Call.RemoveObserver(CallListObserver);
        s_bl.Admin.RemoveClockObserver(clockObserver);
        s_bl.Admin.RemoveConfigObserver(clockObserver);

    }

    /// <summary>
    /// Observer to refresh the call list whenever there are updates.
    /// </summary>
    private void CallListObserver()
    {
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                RefreshCallList();

            });
    }

    /// <summary>
    /// Property to store the currently selected call in the DataGrid.
    /// </summary>
    public BO.ClosedCallInList? SelectedCall { get; set; }

    private void clockObserver()
    {
        if (_observerOperation2 is null || _observerOperation2.Status == DispatcherOperationStatus.Completed)
            _observerOperation2 = Dispatcher.BeginInvoke(() =>
            {
                RefreshCallList();
            });
    }

    
}
