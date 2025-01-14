using System.Windows;
namespace PL;
/// <summary>
/// Interaction logic for MainVolunteerWindow.xaml
/// </summary>
public partial class MainVolunteerWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get(); // Singleton instance of the BL API

    public MainVolunteerWindow(int id3=0)
    {
        InitializeComponent();
        DataContext = this;
        try
        {
            CurrentVolunteer = s_bl.Volunteer.Read(id3);
            if(CurrentVolunteer.IsProgress!=null)
            {
                CurrentCall = s_bl.Call.Read(CurrentVolunteer.IsProgress.CallId);
            }
            else
                CurrentCall = null;

        }
        catch (Exception ex)
        {
            /// <summary>
            /// Show an error message if initialization fails.
            /// </summary>
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public BO.Volunteer CurrentVolunteer
    {
        get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
        set { SetValue(CurrentVolunteerProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentId.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentVolunteerProperty =
        DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(MainVolunteerWindow), new PropertyMetadata(null));



    public BO.Call? CurrentCall
    {
        get { return (BO.Call?)GetValue(CurrentCallProperty); }
        set { SetValue(CurrentCallProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentCall.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentCallProperty =
        DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(MainVolunteerWindow), new PropertyMetadata(null));



    private void BtnUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentVolunteer == null)
        {
            /// <summary>
            /// Show an error message if volunteer details are missing.
            /// </summary>
            MessageBox.Show("Volunteer details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
                s_bl.Volunteer.Update(CurrentVolunteer.Id, CurrentVolunteer);
                MessageBox.Show("Volunteer updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            /// <summary>
            /// Show an error message if something goes wrong.
            /// </summary>
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <sum
    

}
