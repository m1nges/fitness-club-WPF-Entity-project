using fitness_club.Classes;
using fitness_club.Model;
using fitness_club.Windows;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace fitness_club.Pages.TrainerPages
{
    public partial class TrainerClientsPage : Page
    {
        private int trainerId = AuthorizationWin.currentUser.Trainer.TrainerId;

        public TrainerClientsPage()
        {
            InitializeComponent();
            LoadClients();
        }

        private void LoadClients()
        {
            using var db = new AppDbContext();

            var clients = db.Class
                .Include(c => c.ClassVisits)
                    .ThenInclude(cv => cv.ClientMembership)
                        .ThenInclude(cm => cm.Client)
                .Include(c => c.WorkSchedule)
                .Where(c => c.WorkSchedule.TrainerId == trainerId && c.ClassTypeId == 2)
                .SelectMany(c => c.ClassVisits)
                .Select(cv => new
                {
                    ClientMembershipId = cv.ClientMembershipId,
                    ClientId = cv.ClientMembership.Client.ClientId,
                    FullName = cv.ClientMembership.Client.LastName + " " + cv.ClientMembership.Client.FirstName
                })
                .Distinct()
                .ToList();

            ClientsListView.ItemsSource = clients;
        }

        private void ClientsListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dynamic selected = ClientsListView.SelectedItem;
            if (selected != null)
            {
                int clientId = selected.ClientId;
                var window = new WritePlanWindow(trainerId, clientId);
                window.ShowDialog();
            }
        }
    }
}
