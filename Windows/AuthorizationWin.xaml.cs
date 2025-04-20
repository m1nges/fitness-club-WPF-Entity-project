using fitness_club.Classes;
using fitness_club.Model;
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

namespace fitness_club.Windows
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWin.xaml
    /// </summary>
    public partial class AuthorizationWin : Window
    {
        UserFromDb userFromDB = new UserFromDb();
        public static Users currentUser { get; set; } = null;
        public AuthorizationWin()
        {
            InitializeComponent();
        }

        private void registrationWinOpenBtn_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWin regWin = new RegistrationWin();
            regWin.ShowDialog();

        }

        private void enterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!(passwordTb.Text != "" && loginTb.Text != ""))
            {
                MessageBox.Show("Введите данные"); return;
            }
            else
            {
                currentUser = userFromDB.GetUser(loginTb.Text, passwordTb.Text);
                if (currentUser != null)
                {
                    MainWindow mainWin = new MainWindow();
                    mainWin.Show();
                    this.Hide();
                }
            }
        }
    }
}
