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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        ObserverManager.AddListObserver(RefreshVolunteerList);

        public VolunteerListWindow()
        {
            InitializeComponent();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        public BO.CallType callType { get; set; } = BO.CallType.None;

        private void CallTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // עדכון רשימת הקורסים בהתאם לקריטריון הסינון
            VolunteerList = helpReadAllVolunteer(callType);
        }

        private void RefreshVolunteerList()
        {
            VolunteerList = helpReadAllVolunteer(callType);
        }

        private static IEnumerable<BO.VolunteerInList>  helpReadAllVolunteer(BO.CallType callTypeHelp)
        {
            // שימוש במתודת ReadAll מה-BL כדי לשלוף את הרשימה המעודכנת
            return (callTypeHelp == BO.CallType.None)
                ? s_bl?.Volunteer.ReadAll(null, BO.FieldsVolunteerInList.Id, null)!
                : s_bl?.Volunteer.ReadAll(null, BO.FieldsVolunteerInList.Id, callTypeHelp)!;
        }

    }
    
}
