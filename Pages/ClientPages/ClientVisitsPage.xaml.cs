using fitness_club.Model;
using fitness_club.Windows;
using Microsoft.EntityFrameworkCore;
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

namespace fitness_club.Pages.ClientPages
{
    /// <summary>
    /// Логика взаимодействия для ClientVisitsPage.xaml
    /// </summary>
    public partial class ClientVisitsPage : Page
    {
        public ClientVisitsPage()
        {
            InitializeComponent();
            LoadClientPastVisits();
        }

        private void LoadClientPastVisits()
        {
            using (var db = new AppDbContext())
            {
                int clientId = db.Clients
                    .Where(c => c.UserId == AuthorizationWin.currentUser.UserId)
                    .Select(c => c.ClientId)
                    .FirstOrDefault();

                var today = DateTime.Today;

                var pastVisits = db.ClassVisits
                    .Include(cv => cv.Class)
                        .ThenInclude(c => c.WorkSchedule)
                            .ThenInclude(ws => ws.Trainer)
                    .Include(cv => cv.Class)
                        .ThenInclude(c => c.Hall)
                    .Include(cv => cv.Class)
                        .ThenInclude(c => c.ClassInfo)
                    .Where(cv =>
                        cv.ClientMembership.ClientId == clientId &&
                        cv.Class.WorkSchedule.WorkDate < today)
                    .Select(cv => new
                    {
                        cv.Class.ClassId,
                        ClassName = cv.Class.ClassInfo.ClassName,
                        Date = cv.Class.WorkSchedule.WorkDate,
                        Time = $"{cv.Class.StartTime:hh\\:mm} - {cv.Class.EndTime:hh\\:mm}",
                        HallName = cv.Class.Hall.HallName,
                        TrainerName = $"{cv.Class.WorkSchedule.Trainer.LastName} {cv.Class.WorkSchedule.Trainer.FirstName}",
                        Price = cv.Class.Price
                    })
                    .OrderByDescending(x => x.Date)
                    .ToList();

                ClientVisitsList.ItemsSource = pastVisits;
            }
        }

        private void OpenClassDetails(dynamic selectedItem)
        {
            
            try
            {
                if (selectedItem != null)
                {
                    int classId = selectedItem.ClassId;
                    ClassDetailsWindow cdw = new ClassDetailsWindow(classId, "visitsPage");
                    cdw.Show();
                }
                else
                {
                    MessageBox.Show("Вы не выбрали занятие");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить информацию о занятии: {ex.Message}");
            }
        }

        private void ClientVisitsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dynamic selectedItem = ClientVisitsList.SelectedItem;
            if (selectedItem != null)
                OpenClassDetails(selectedItem);
            else
                MessageBox.Show("Вы не выбрали занятие, о котором хотите загрузить информацию");
        }
    }
}
