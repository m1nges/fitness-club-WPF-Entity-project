using fitness_club.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace fitness_club.Pages.AdminPages
{
    public partial class EditGroupClassWindow : Window
    {
        private readonly int classId;
        public EditGroupClassWindow(int classId)
        {
            InitializeComponent();
            this.classId = classId;

            LoadTrainers();
            LoadClassData();
        }

        private void LoadTrainers()
        {
            using var db = new AppDbContext();
            TrainerComboBox.ItemsSource = db.Trainers
                .Select(t => new
                {
                    t.TrainerId,
                    FullName = t.LastName + " " + t.FirstName
                }).ToList();
        }

        private void LoadClassData()
        {
            using var db = new AppDbContext();
            var cls = db.Class
                .Include(c => c.WorkSchedule)
                .FirstOrDefault(c => c.ClassId == classId);

            if (cls != null)
            {
                TrainerComboBox.SelectedValue = cls.WorkSchedule.TrainerId;
                DatePicker.SelectedDate = cls.WorkSchedule.WorkDate;
                StartTimeBox.Text = cls.StartTime.ToString(@"hh\:mm");
                EndTimeBox.Text = cls.EndTime.ToString(@"hh\:mm");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (TrainerComboBox.SelectedValue is not int newTrainerId ||
                !DatePicker.SelectedDate.HasValue ||
                !TimeSpan.TryParseExact(StartTimeBox.Text, "hh\\:mm", CultureInfo.InvariantCulture, out var startTime) ||
                !TimeSpan.TryParseExact(EndTimeBox.Text, "hh\\:mm", CultureInfo.InvariantCulture, out var endTime))
            {
                MessageBox.Show("Проверьте введенные данные.");
                return;
            }

            var date = DatePicker.SelectedDate.Value;

            using var db = new AppDbContext();

            var ws = db.WorkSchedules
                .FirstOrDefault(w => w.TrainerId == newTrainerId && w.WorkDate == date);

            if (ws == null)
            {
                MessageBox.Show("У выбранного тренера нет смены на эту дату.");
                return;
            }

            var cls = db.Class.Find(classId);
            if (cls != null)
            {
                cls.WorkScheduleId = ws.WorkScheduleId;
                cls.StartTime = startTime;
                cls.EndTime = endTime;

                db.SaveChanges();
                MessageBox.Show("Занятие успешно обновлено.");
                this.Close();
            }
        }
    }
}
