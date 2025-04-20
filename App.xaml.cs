using fitness_club.Windows;
using System.Configuration;
using System.Data;
using System.Windows;

namespace fitness_club
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Создаем и показываем окно авторизации
            AuthorizationWin authWindow = new AuthorizationWin();
            Application.Current.MainWindow = authWindow;
            authWindow.Show();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Создаем главное окно
            var authWindow = new AuthorizationWin();
            authWindow.Closed += (s, args) => this.Shutdown(); // Закрывать приложение при закрытии этого окна
        }

    }

}
