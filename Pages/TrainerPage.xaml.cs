using fitness_club.Classes;
using fitness_club.Pages.TrainerPages;
using fitness_club.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace fitness_club.Pages
{
    /// <summary>
    /// Логика взаимодействия для TrainerPage.xaml
    /// </summary>
    public partial class TrainerPage : Page
    {
        public TrainerPage()
        {
            InitializeComponent();
            FrameClass.TrainerInnerFrame = TrainerFrame;
            TrainerFrame.Navigate(new TrainerProfilePage());
            userName.Text = AuthorizationWin.currentUser.Trainer.LastName + " " +
                AuthorizationWin.currentUser.Trainer.FirstName + " " +
                AuthorizationWin.currentUser.Trainer.Patronymic;
        }

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            TrainerFrame.Navigate(new TrainerProfilePage());
        }

        private void ReviewsBtn_Click(object sender, RoutedEventArgs e)
        {
            TrainerFrame.Navigate(new TrainerReviewsPage());
        }

        private void SchedulesBtn_Click(object sender, RoutedEventArgs e)
        {
            TrainerFrame.Navigate(new TrainerClassesPage());

        }

        private void VisitsBtn_Click(object sender, RoutedEventArgs e)
        {
            TrainerFrame.Navigate(new TrainerPastClassesPage());
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

        private void MyClients_Click(object sender, RoutedEventArgs e)
        {
            TrainerFrame.Navigate(new TrainerClientsPage());
        }
    }
}
