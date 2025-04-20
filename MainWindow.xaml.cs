using fitness_club.Windows;
using fitness_club.Classes;
using fitness_club.Pages;
using fitness_club.Pages.ClientPages;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace fitness_club
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FrameClass.MainAppFrame = MainFrame;
            LoadStartPage();
        }

        private void LoadStartPage()
        {
            switch (AuthorizationWin.currentUser.RoleId)
            {
                case 1:
                    MainFrame.Navigate(new TrainerPage());
                    break;
                case 2: 
                    MainFrame.Navigate(new ClientPage());
                    break;
                case 3: 
                    MainFrame.Navigate(new AdminPage());
                    break;
                default:
                    MessageBox.Show("Неизвестная роль пользователя.");
                    Close();
                    break;
            }
        }
    }
}