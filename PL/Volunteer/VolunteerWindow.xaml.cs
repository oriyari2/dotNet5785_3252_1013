using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Volunteer;

/// <summary>
/// Interaction logic for VolunteerWindow.xaml
/// </summary>
public partial class VolunteerWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public static readonly DependencyProperty ButtonTextProperty =
        DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow));

    public string ButtonText
    {
        get { return (string)GetValue(ButtonTextProperty); }
        set { SetValue(ButtonTextProperty, value); }
    }

    public VolunteerWindow(int id = 0)
    {
        ButtonText = id == 0 ? "Add" : "Update";
        InitializeComponent();
        DataContext = this;
        currentVolunteer = (id != 0) ? s_bl.Volunteer.Read(id)! : new BO.Volunteer();
    }

    public BO.Volunteer currentVolunteer
    {
        get { return (BO.Volunteer)GetValue(currentVolunteerProperty); }
        set { SetValue(currentVolunteerProperty, value); }
    }

    // Using a DependencyProperty as the backing store for currentVolunteer.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty currentVolunteerProperty =
    DependencyProperty.Register("currentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

    private void ButtonAddUpdate_Click(object sender, RoutedEventArgs e)
    {
        if (currentVolunteer == null)
        {
            MessageBox.Show("Volunteer details are missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (ButtonText == "Add")
        {
            try
            {
                s_bl.Volunteer.Create(currentVolunteer);
                MessageBox.Show("Volunteer added successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        else
        {
            try
            {
                s_bl.Volunteer.Update(currentVolunteer.Id, currentVolunteer);//**
                MessageBox.Show("Volunteer updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void queryVolunteer()
    {
        int id = currentVolunteer!.Id;
        currentVolunteer = null;
        currentVolunteer = s_bl.Volunteer.Read(id);
    }

    private void volunteerObserver()
        => queryVolunteer();

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (currentVolunteer!.Id != 0)
            s_bl.Volunteer.AddObserver(currentVolunteer!.Id, volunteerObserver);
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        if (currentVolunteer!.Id != 0)
            s_bl.Volunteer.RemoveObserver(currentVolunteer!.Id, volunteerObserver);
    }


}




