using fitness_club.Classes;
using fitness_club.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace fitness_club.Pages.AdminPages
{
    public partial class AdminWorkSchedulePage : Page
    {
        public AdminWorkSchedulePage()
        {
            InitializeComponent();
            WorkDatePicker.SelectedDate = DateTime.Today;
            LoadTrainers();
            LoadSchedules();
        }

        private void LoadTrainers()
        {
            using var db = new AppDbContext();
            var trainers = db.Trainers
                .Include(t => t.User)
                .Select(t => new
                {
                    t.TrainerId,
                    FullName = t.LastName + " " + t.FirstName
                }).ToList();

            TrainerComboBox.ItemsSource = trainers;
        }

        private void LoadSchedules()
        {
            using var db = new AppDbContext();
            var today = DateTime.Today;

            var data = db.WorkSchedules
                .Include(ws => ws.Trainer)
                .ThenInclude(t => t.User)
                .Where(ws => ws.WorkDate >= today)
                .OrderBy(ws => ws.WorkDate)
                .ThenBy(ws => ws.StartTime)
                .Select(ws => new
                {
                    ws.WorkScheduleId,
                    TrainerName = ws.Trainer.LastName + " " + ws.Trainer.FirstName,
                    Date = ws.WorkDate.ToShortDateString(),
                    StartTime = ws.StartTime.ToString(@"hh\:mm"),
                    EndTime = ws.EndTime.ToString(@"hh\:mm")
                })
                .ToList();

            ScheduleListView.ItemsSource = data;
        }


        private void AddSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (TrainerComboBox.SelectedValue is not int trainerId ||
                !TimeSpan.TryParseExact(StartTimeBox.Text, "hh\\:mm", CultureInfo.InvariantCulture, out var startTime) ||
                !TimeSpan.TryParseExact(EndTimeBox.Text, "hh\\:mm", CultureInfo.InvariantCulture, out var endTime) ||
                !(WorkDatePicker.SelectedDate is DateTime selectedDate))
            {
                MessageBox.Show("Проверьте введённые данные.");
                return;
            }

            if (startTime >= endTime)
            {
                MessageBox.Show("Время начала должно быть раньше времени конца.");
                return;
            }

            using var db = new AppDbContext();

            bool exists = db.WorkSchedules.Any(ws =>
                ws.TrainerId == trainerId &&
                ws.WorkDate == selectedDate);

            if (exists)
            {
                MessageBox.Show("У этого тренера уже есть смена на выбранную дату.");
                return;
            }

            db.WorkSchedules.Add(new WorkSchedule
            {
                TrainerId = trainerId,
                WorkDate = selectedDate,
                StartTime = startTime,
                EndTime = endTime
            });

            db.SaveChanges();
            LoadSchedules();
            MessageBox.Show("Смена добавлена.");
        }

        private void DeleteSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int scheduleId)
            {
                using var db = new AppDbContext();

                // Проверка: есть ли занятия на эту смену
                bool hasClasses = db.Class.Any(c => c.WorkScheduleId == scheduleId);
                if (hasClasses)
                {
                    MessageBox.Show(
                        "Невозможно удалить смену, так как на неё назначены занятия.\n" +
                        "Сначала отмените или перенесите эти занятия на другую смену.",
                        "Удаление невозможно", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var ws = db.WorkSchedules.Find(scheduleId);
                if (ws != null)
                {
                    db.WorkSchedules.Remove(ws);
                    db.SaveChanges();
                    LoadSchedules();
                    MessageBox.Show("Смена успешно удалена.");
                }
            }
        }

    }
}
