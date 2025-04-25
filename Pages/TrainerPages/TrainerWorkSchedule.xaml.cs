using fitness_club.Model;
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

namespace fitness_club.Pages.TrainerPages
{
    /// <summary>
    /// Логика взаимодействия для TrainerWorkSchedule.xaml
    /// </summary>
    public partial class TrainerWorkSchedule : Page
    {
        public TrainerWorkSchedule()
        {
            InitializeComponent();
            LoadTrainerSchedule();
        }

        private void LoadTrainerSchedule()
        {
            using var db = new AppDbContext();

            var trainerId = AuthorizationWin.currentUser.Trainer.TrainerId;
            var today = DateTime.Today;
            var inTwoWeeks = today.AddDays(14);

            var schedule = db.WorkSchedules
                .Where(ws => ws.TrainerId == trainerId &&
                             ws.WorkDate >= today &&
                             ws.WorkDate <= inTwoWeeks)
                .OrderBy(ws => ws.WorkDate)
                .Select(ws => new
                {
                    ws.WorkDate,
                    StartTime = ws.StartTime.ToString(@"hh\:mm"),
                    EndTime = ws.EndTime.ToString(@"hh\:mm")
                })
                .ToList();

            TrainerScheduleGrid.ItemsSource = schedule;
        }
    }
}
