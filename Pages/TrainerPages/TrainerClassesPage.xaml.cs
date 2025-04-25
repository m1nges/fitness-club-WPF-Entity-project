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
using System.Windows.Controls.Primitives;
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
    /// Логика взаимодействия для TrainerClassesPage.xaml
    /// </summary>
    public partial class TrainerClassesPage : Page
    {
        private readonly int trainerId;
        DateTime todayUtc = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        public TrainerClassesPage()
        {
            InitializeComponent();
            trainerId = AuthorizationWin.currentUser.Trainer.TrainerId;
            LoadClasses();
        }

        private void LoadClasses()
        {
            var todayUtc = DateTime.UtcNow.Date;

            using var db = new AppDbContext();

            var classes = db.Class
                .Include(c => c.ClassInfo)
                .Include(c => c.Hall)
                .Include(c => c.ClassType)
                .Include(c => c.WorkSchedule)
                    .ThenInclude(ws => ws.Trainer)
                .Include(c => c.ClassVisits)
                    .ThenInclude(cv => cv.ClientMembership.Client)
                .Where(c => c.WorkSchedule.TrainerId == trainerId &&
                            c.WorkSchedule.WorkDate > todayUtc)
                .ToList();

            var individual = classes
                .Where(c => c.ClassTypeId == 2)
                .Select(c => new
                {
                    c.ClassId,
                    ClassName = c.ClassInfo.ClassName,
                    Date = c.WorkSchedule.WorkDate,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    HallName = c.Hall.HallName,
                    Price = c.Price ?? 0,
                    ClientName = c.ClassVisits.FirstOrDefault()?.ClientMembership?.Client != null
                        ? $"{c.ClassVisits.First().ClientMembership.Client.LastName} {c.ClassVisits.First().ClientMembership.Client.FirstName}"
                        : "Нет клиента"
                })
                .ToList();

            var group = classes
                .Where(c => c.ClassTypeId != 2)
                .Select(c => new
                {
                    c.ClassId,
                    ClassName = c.ClassInfo.ClassName,
                    Date = c.WorkSchedule.WorkDate,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    HallName = c.Hall.HallName,
                    Price = c.Price ?? 0,
                    VisitorsCount = c.ClassVisits.Count
                })
                .ToList();

            IndividualSessionsList.ItemsSource = individual;
            GroupSessionsList.ItemsSource = group;
        }

        //private void IndividualSessionsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    dynamic selectedClass = IndividualSessionsList.SelectedItem;
        //    if (selectedClass != null)
        //    {
        //        ;
        //        NavigationService?.Navigate(new ClassVisitorsListPage(selectedClass.ClassId));
        //    }
        //    else
        //    {
        //        MessageBox.Show("Выберите занятие", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        //private void GroupSessionsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    dynamic selectedClass = GroupSessionsList.SelectedItem;
        //    if (selectedClass != null)
        //    {
        //        NavigationService?.Navigate(new ClassVisitorsListPage(selectedClass.ClassId));
        //    }
        //    else
        //    {
        //        MessageBox.Show("Выберите занятие", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}



    }
}
