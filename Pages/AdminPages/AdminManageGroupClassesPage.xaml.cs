using fitness_club.Model;
using fitness_club.Classes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace fitness_club.Pages.AdminPages
{
    public partial class AdminManageGroupClassesPage : Page
    {
        public AdminManageGroupClassesPage()
        {
            InitializeComponent();
            LoadData();
            LoadClasses();
            //LoadWorkSchedules();
            LoadFreeSlots(); 
        }

        private void LoadFreeSlots()
        {
            using var db = new AppDbContext();
            var today = DateTime.Today;

            var workSchedules = db.WorkSchedules
                .Include(ws => ws.Trainer)
                .Where(ws => ws.WorkDate >= today)
                .OrderBy(ws => ws.WorkDate)
                .ThenBy(ws => ws.StartTime)
                .ToList();

            var groupClasses = db.Class
                .Include(c => c.WorkSchedule)
                .Where(c => c.ClassTypeId == 1 && c.WorkSchedule.WorkDate >= today)
                .ToList();

            var freeSlots = new List<object>();

            foreach (var ws in workSchedules)
            {
                var trainerId = ws.TrainerId;
                var date = ws.WorkDate;
                var start = ws.StartTime;
                var end = ws.EndTime;

                var classes = groupClasses
                    .Where(c => c.WorkSchedule.TrainerId == trainerId && c.WorkSchedule.WorkDate == date)
                    .OrderBy(c => c.StartTime)
                    .ToList();

                var current = start;

                foreach (var cls in classes)
                {
                    if (current < cls.StartTime)
                    {
                        freeSlots.Add(new
                        {
                            Trainer = ws.Trainer.LastName + " " + ws.Trainer.FirstName,
                            Date = date.ToShortDateString(),
                            Slot = $"{current:hh\\:mm} - {cls.StartTime:hh\\:mm}"
                        });
                    }
                    current = cls.EndTime > current ? cls.EndTime : current;
                }

                if (current < end)
                {
                    freeSlots.Add(new
                    {
                        Trainer = ws.Trainer.LastName + " " + ws.Trainer.FirstName,
                        Date = date.ToShortDateString(),
                        Slot = $"{current:hh\\:mm} - {end:hh\\:mm}"
                    });
                }
            }

            FreeSlotsListView.ItemsSource = freeSlots;
        }


        private void LoadData()
        {
            using var db = new AppDbContext();

            TrainerComboBox.ItemsSource = db.Trainers
                .Select(t => new
                {
                    t.TrainerId,
                    FullName = t.LastName + " " + t.FirstName
                }).ToList();

            HallComboBox.ItemsSource = db.Halls
                .Select(h => new
                {
                    h.HallId,
                    h.HallName
                }).ToList();

            ClassInfoComboBox.ItemsSource = db.ClassInfo
                .Where(c=>c.ClassName!= "Индивидуальное занятие")
                .Select(c => new
                {
                    c.ClassInfoId,
                    c.ClassName
                }).ToList();

            DatePicker.SelectedDate = DateTime.Today;
        }

        private void CreateClass_Click(object sender, RoutedEventArgs e)
        {
            if (TrainerComboBox.SelectedValue is not int trainerId ||
                HallComboBox.SelectedValue is not int hallId ||
                ClassInfoComboBox.SelectedValue is not int classInfoId ||
                !DatePicker.SelectedDate.HasValue ||
                !TimeSpan.TryParseExact(StartTimeBox.Text, "hh\\:mm", CultureInfo.InvariantCulture, out var startTime) ||
                !TimeSpan.TryParseExact(EndTimeBox.Text, "hh\\:mm", CultureInfo.InvariantCulture, out var endTime) ||
                !int.TryParse(PeopleQtyBox.Text, out int peopleQty) || !double.TryParse(priceBox.Text, out double price))
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            if (startTime >= endTime)
            {
                MessageBox.Show("Время начала должно быть раньше времени окончания.");
                return;
            }

            var date = DatePicker.SelectedDate.Value;

            using var db = new AppDbContext();

            var workSchedule = db.WorkSchedules
                .FirstOrDefault(ws => ws.TrainerId == trainerId && ws.WorkDate == date);

            if (workSchedule == null)
            {
                MessageBox.Show("У выбранного тренера нет смены на эту дату.");
                return;
            }

            var newClass = new Class
            {
                ClassInfoId = classInfoId,
                ClassTypeId = 1, // групповые занятия
                HallId = hallId,
                WorkScheduleId = workSchedule.WorkScheduleId,
                StartTime = startTime,
                EndTime = endTime,
                PeopleQuantity = peopleQty,
                Price = price
            };
            bool overlaps = db.Class.Any(c =>
                c.WorkScheduleId == workSchedule.WorkScheduleId &&
                startTime < c.EndTime &&
                endTime > c.StartTime);

            if (overlaps)
            {
                MessageBox.Show("Нельзя создать занятие: время пересекается с другим занятием у этого тренера.");
                return;
            }

            db.Class.Add(newClass);
            db.SaveChanges();

            MessageBox.Show("Занятие успешно создано.");
            LoadFreeSlots();
            LoadClasses();
        }

        private void LoadClasses()
        {
            using var db = new AppDbContext();

            var today = DateTime.Today;

            var groupClasses = db.Class
                .Include(c => c.ClassInfo)
                .Include(c => c.Hall)
                .Include(c => c.WorkSchedule)
                    .ThenInclude(ws => ws.Trainer)
                .Where(c => c.ClassTypeId == 1 && c.WorkSchedule.WorkDate >= today)
                .OrderBy(c => c.WorkSchedule.WorkDate)
                .ThenBy(c => c.StartTime)
                .Select(c => new
                {
                    c.ClassId,
                    Date = c.WorkSchedule.WorkDate.ToShortDateString(),
                    Time = $"{c.StartTime:hh\\:mm} - {c.EndTime:hh\\:mm}",
                    ClassName = c.ClassInfo.ClassName,
                    TrainerName = c.WorkSchedule.Trainer.LastName + " " + c.WorkSchedule.Trainer.FirstName,
                    HallName = c.Hall.HallName,
                    PeopleQty = c.PeopleQuantity,
                    c.Price
                })
                .ToList();

            GroupClassesListView.ItemsSource = groupClasses;
        }


        private void DeleteClass_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int classId)
            {
                using var db = new AppDbContext();
                var selected = db.Class.Find(classId);
                if (selected != null)
                {
                    db.Class.Remove(selected);
                    db.SaveChanges();
                    MessageBox.Show("Занятие удалено.");
                    LoadClasses();
                }
            }
        }

        private void EditClass_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int classId)
            {
                var win = new EditGroupClassWindow(classId);
                win.ShowDialog();
                LoadClasses();
            }
        }


        private void FreeSlotsListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dynamic selected = FreeSlotsListView.SelectedItem;
            if (selected!=null)
            {
                // Название тренера из слота
                string trainerName = selected.Trainer as string;
                string dateString = selected.Date as string;
                string timeSlot = selected.Slot as string;

                // Разделим время на начало и конец
                var timeParts = timeSlot.Split(" - ");
                if (timeParts.Length == 2)
                {
                    StartTimeBox.Text = timeParts[0];
                    EndTimeBox.Text = timeParts[1];
                }

                // Установим дату
                if (DateTime.TryParseExact(dateString, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    DatePicker.SelectedDate = parsedDate;
                }

                // Найдём тренера в ComboBox по совпадению ФИО
                foreach (var item in TrainerComboBox.Items)
                {
                    dynamic trainer = item;
                    if ((trainer.FullName as string).Trim() == trainerName.Trim())
                    {
                        TrainerComboBox.SelectedItem = trainer;
                        break;
                    }
                }

                MessageBox.Show("Слот выбран. Вы можете сразу нажать 'Создать занятие'.", "Подсказка", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
