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
    public ManagerChooseWindow( int id2=0)
    {
        InitializeComponent();
        CurrentId = id2;
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
    public int CurrentId
    {
        get { return (int)GetValue(CurrentIdProperty); }
        set { SetValue(CurrentIdProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentId.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentIdProperty =
        DependencyProperty.Register("CurrentId", typeof(int), typeof(ManagerChooseWindow), new PropertyMetadata(0));
    private void btnVolunteerWindow_Click(object sender, RoutedEventArgs e)
    {
        new MainVolunteerWindow(CurrentId).Show();
    }


}

