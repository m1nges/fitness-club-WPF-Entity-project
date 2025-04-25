using fitness_club.Classes;
using fitness_club.Model;
using fitness_club.Pages.ClientPages;
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
    /// Логика взаимодействия для ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        public ClientPage()
        {
            InitializeComponent();
            FrameClass.ClientInnerFrame = ClientFrame;
            ClientFrame.Navigate(new ClientSchedulePage());
            userName.Text = AuthorizationWin.currentUser.Client.LastName + " " +
                AuthorizationWin.currentUser.Client.FirstName + " " +
                AuthorizationWin.currentUser.Client.Patronymic;
        }

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            ClientFrame.Navigate(new ClientProfilePage());
        }

        private void MembershipBtn_Click(object sender, RoutedEventArgs e)
        {
            ClientFrame.Navigate(new ClientMembershipPage());
        }

        private void VisitsBtn_Click(object sender, RoutedEventArgs e)
        {
            ClientFrame.Navigate(new ClientVisitsPage());
        }

        private void ServicesBtn_Click(object sender, RoutedEventArgs e)
        {
            ClientFrame.Navigate(new ClientServicesPage());
        }

        private void SchedulesBtn_Click(object sender, RoutedEventArgs e)
        {
            ClientFrame.Navigate(new ClientSchedulePage());
        }

        private void logoutBtn_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationWin.currentUser = null;
            AuthorizationWin authWin = new AuthorizationWin();
            authWin.Show();
            var window = Window.GetWindow(this);
            window?.Close();
        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void RecommendationBtn_Click(object sender, RoutedEventArgs e)
        {
            ClientFrame.Navigate(new ClientPageRecommendations());
        }

        private void EquipmentGuidePageBtn_Click(object sender, RoutedEventArgs e)
        {
            ClientFrame.Navigate(new ClientEquipmentGuidePage());
        }

        private void TrainersBtn_Click(object sender, RoutedEventArgs e)
        {
            ClientFrame.Navigate(new ClientTrainerListPage());
        }

        private void PaymentBtn_Click(object sender, RoutedEventArgs e)
        {
            ClientFrame.Navigate(new ClientPaymentPage());
        }

        private void TransactionsBtn_Click(object sender, RoutedEventArgs e)
        {
            ClientFrame.Navigate(new ClientTransactionsPage());
        }

        private void TrainingPlanBtn_Click(object sender, RoutedEventArgs e)
        {
            ClientFrame.Navigate(new ClientTrainingPlanFromTrainer());
        }
    }
}
