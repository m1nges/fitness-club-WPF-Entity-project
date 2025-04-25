using fitness_club.Model;
using fitness_club.Windows;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace fitness_club.Pages.TrainerPages
{
    public partial class TrainerPastClassesPage : Page
    {
        private int trainerId = AuthorizationWin.currentUser.Trainer.TrainerId;
        private DateTime today = DateTime.UtcNow.Date;

        public TrainerPastClassesPage()
        {
            InitializeComponent();
            LoadClasses();
            FilterComboBox.SelectedIndex = 0;
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadClasses();
        }

        private void LoadClasses()
        {
            using var db = new AppDbContext();
            var classes = db.Class
                .Include(c => c.ClassInfo)
                .Include(c => c.Hall)
                .Include(c => c.WorkSchedule)
                .Where(c => c.WorkSchedule.TrainerId == trainerId &&
                            c.WorkSchedule.WorkDate <= today)
                .ToList();

            string selected = (FilterComboBox.SelectedItem as ComboBoxItem)?.Content as string;

            if (selected == "Только отмеченные")
            {
                classes = classes.Where(c => c.TrainerChecked).ToList();
            }
            else if (selected == "Только неотмеченные")
            {
                classes = classes.Where(c => !c.TrainerChecked && (today - c.WorkSchedule.WorkDate).TotalDays < 3).ToList();
            }

            PastClassesListView.ItemsSource = classes
                .Select(c => new
                {
                    c.ClassId,
                    ClassName = c.ClassInfo.ClassName,
                    Date = c.WorkSchedule.WorkDate,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    HallName = c.Hall.HallName,
                    Price = c.Price ?? 0,
                    TrainerChecked = c.TrainerChecked
                })
                .OrderByDescending(x => x.Date)
                .ToList();
        }

        private void PastClassesListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dynamic selected = PastClassesListView.SelectedItem;
            if (selected != null)
            {
                DateTime classDate = selected.Date;

                if ((today - classDate).TotalDays > 3)
                {
                    MessageBox.Show("Прошло более 3 дней с момента проведения занятия. Отметка недоступна.", "Ограничение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                NavigationService?.Navigate(new ClassVisitorsListPage(selected.ClassId));
            }
        }
    }
}
