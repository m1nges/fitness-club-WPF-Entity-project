using fitness_club.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using fitness_club.Pages.AdminPages;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using fitness_club.Windows;

namespace fitness_club.Pages
{
    /// <summary>
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            FrameClass.AdminInnerFrame = AdminFrame;
        }

        private void HallEquipment_Click(object sender, RoutedEventArgs e)
        {
            
            AdminFrame.Navigate(new AdminEquipmentManagementPage());
        }

        private void Workschedule_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.Navigate(new AdminWorkSchedulePage());
        }

        private void CreateGroupClassesPage_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.Navigate(new AdminManageGroupClassesPage());
        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void logoutBtn_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationWin.currentUser = null;
            AuthorizationWin authWin = new AuthorizationWin();
            authWin.Show();
            var window = Window.GetWindow(this);
            window?.Close();
        }

        private void ReviewModeration_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.Navigate(new AdminModerationPage());
        }

        private void ManageUsers_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.Navigate(new AdminUserEditPage());
        }

        private void ManageServices_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.Navigate(new AdminServiceManagementPage());
        }

        private void ManageMemberships_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.Navigate(new AdminMembershipManagementPage());

        }
    }
}
