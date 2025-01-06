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

namespace PL;

/// <summary>
/// Interaction logic for ManagerChooseWindow.xaml
/// </summary>
public partial class ManagerChooseWindow : Window
{
    public ManagerChooseWindow()
    {
        InitializeComponent();
    }
    private void btnManagerWindow_Click(object sender, RoutedEventArgs e)
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
        new MainWindow().Show();
    }

    private void btnVolunteerWindow_Click(object sender, RoutedEventArgs e)
    {
        new MainVolunteerWindow().Show();
    }


}

