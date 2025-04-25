using fitness_club.Classes;
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
    /// Логика взаимодействия для ClientSchedulePage.xaml  
    /// </summary>  
    public partial class ClientSchedulePage : Page
    {
        Users currentUser = null;
        public ClientSchedulePage()
        {
            InitializeComponent();
            currentUser = AuthorizationWin.currentUser;
            LoadIndividualClasses();
            LoadGroupClasses();
            LoadTrainerList();
            LoadAvailableGroupClasses();
        }

        private void LoadTrainerList()
        {
            using (var db = new AppDbContext())
            {
                TrainerComboBox.ItemsSource = db.Trainers
                    .Select(t => new
                    {
                        t.TrainerId,
                        FullName = t.LastName + " " + t.FirstName
                    })
                    .ToList();
            }
        }

        public void LoadIndividualClasses()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var clientId = db.Clients
                        .Where(c => c.UserId == currentUser.UserId)
                        .Select(c => c.ClientId)
                        .FirstOrDefault();

                    var individualClasses = db.ClassVisits
                        .Include(cv => cv.Class)
                            .ThenInclude(c => c.ClassType)
                        .Include(cv => cv.Class)
                            .ThenInclude(c => c.WorkSchedule)
                                .ThenInclude(ws => ws.Trainer)
                        .Include(cv => cv.Class)
                            .ThenInclude(c => c.ClassInfo)
                        .Where(cv =>
                            cv.ClientMembership.ClientId == clientId &&
                            cv.Class.ClassType.ClassTypeName == "Индивидуальные" &&
                            cv.Class.WorkSchedule.WorkDate >= DateTime.Today)
                        .Select(cv => new
                        {
                            cv.Class.ClassId,
                            ClassName = cv.Class.ClassInfo.ClassName,
                            Date = cv.Class.WorkSchedule.WorkDate,
                            Time = $"{cv.Class.StartTime} - {cv.Class.EndTime}",
                            TrainerName = cv.Class.WorkSchedule.Trainer.LastName + " " + cv.Class.WorkSchedule.Trainer.FirstName
                        })
                        .ToList();

                    AuthorizedIndividualList.ItemsSource = individualClasses;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки индивидуальных занятий: {ex.Message}");
            }
        }


        public void LoadGroupClasses()
        {
            try
            {
                using (var db = new AppDbContext())
                {
                    var clientId = db.Clients
                        .Where(c => c.UserId == currentUser.UserId)
                        .Select(c => c.ClientId)
                        .FirstOrDefault();

                    var groupClasses = db.ClassVisits
                        .Include(cv => cv.Class)
                            .ThenInclude(c => c.ClassType)
                        .Include(cv => cv.Class)
                            .ThenInclude(c => c.WorkSchedule)
                                .ThenInclude(ws => ws.Trainer)
                        .Include(cv => cv.Class)
                            .ThenInclude(c => c.ClassInfo)
                        .Where(cv =>
                            cv.ClientMembership.ClientId == clientId &&
                            cv.Class.ClassType.ClassTypeName != "Индивидуальные" &&
                            cv.Class.WorkSchedule.WorkDate >= DateTime.Today)
                        .Select(cv => new
                        {
                            cv.Class.ClassId,
                            ClassName = cv.Class.ClassInfo.ClassName,
                            Date = cv.Class.WorkSchedule.WorkDate,
                            Time = $"{cv.Class.StartTime} - {cv.Class.EndTime}",
                            TrainerName = cv.Class.WorkSchedule.Trainer.LastName + " " + cv.Class.WorkSchedule.Trainer.FirstName
                        })
                        .ToList();

                    AuthorizedGroupList.ItemsSource = groupClasses;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки групповых занятий: {ex.Message}");
            }
        }


        private void DurationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAvailableTimeSlots();
        }

        private void TrainerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAvailableTimeSlots();
        }

        private void UpdateAvailableTimeSlots()
        {
            TimeSlotsListBox.ItemsSource = null;
            PriceTextBlock.Text = "";

            if (TrainerComboBox.SelectedValue == null || DurationComboBox.SelectedItem == null)
                return;

            int trainerId = (int)TrainerComboBox.SelectedValue;
            double duration = GetDurationFromComboBox();
            var durationTime = TimeSpan.FromHours(duration);

            try
            {
                using (var db = new AppDbContext())
                {
                    // Получаем данные клиента
                    var clientId = db.Clients
                        .Where(c => c.UserId == currentUser.UserId)
                        .Select(c => c.ClientId)
                        .FirstOrDefault();

                    // Получаем тренера и его цену
                    var trainer = db.Trainers.FirstOrDefault(t => t.TrainerId == trainerId);
                    if (trainer?.IndividualPrice == null)
                    {
                        PriceTextBlock.Text = "Не указана цена тренировки";
                        PriceTextBlock.Foreground = Brushes.Red;
                        return;
                    }

                    // Получаем все рабочие расписания тренера (ближайшие 14 дней)
                    var today = DateTime.Today;
                    var schedules = db.WorkSchedules
                        .Where(ws => ws.TrainerId == trainerId &&
                                    ws.WorkDate >= today &&
                                    ws.WorkDate <= today.AddDays(14))
                        .ToList();

                    // Получаем все занятия тренера
                    var trainerClasses = db.Class
                        .Include(c => c.WorkSchedule)
                        .Where(c => c.WorkSchedule.TrainerId == trainerId)
                        .Select(c => new
                        {
                            c.WorkSchedule.WorkDate,
                            Start = c.StartTime,
                            End = c.EndTime
                        })
                        .ToList();

                    // Получаем все занятия клиента
                    var clientClasses = db.ClassVisits
                        .Include(cv => cv.Class)
                        .ThenInclude(c => c.WorkSchedule)
                        .Where(cv => cv.ClientMembership.ClientId == clientId)
                        .Select(cv => new
                        {
                            cv.Class.WorkSchedule.WorkDate,
                            Start = cv.Class.StartTime,
                            End = cv.Class.EndTime
                        })
                        .ToList();

                    var availableSlots = new List<string>();

                    foreach (var schedule in schedules)
                    {
                        var currentTime = schedule.StartTime;

                        while (currentTime + durationTime <= schedule.EndTime)
                        {
                            var slotEnd = currentTime + durationTime;

                            bool isTrainerBusy = trainerClasses.Any(tc =>
                                tc.WorkDate == schedule.WorkDate &&
                                currentTime < tc.End && slotEnd > tc.Start);

                            if (!isTrainerBusy)
                            {
                                availableSlots.Add($"{schedule.WorkDate:yyyy-MM-dd} {currentTime:hh\\:mm}");
                            }

                            currentTime = currentTime.Add(TimeSpan.FromMinutes(30));
                        }
                    }

                    TimeSlotsListBox.ItemsSource = availableSlots;

                    if (availableSlots.Any())
                    {
                        PriceTextBlock.Text = $"Стоимость: {trainer.IndividualPrice * duration} руб.";
                        PriceTextBlock.Foreground = Brushes.Green;
                    }
                    else
                    {
                        PriceTextBlock.Text = "Нет доступных слотов";
                        PriceTextBlock.Foreground = Brushes.Red;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке слотов: {ex.Message}");
            }
        }

        private double GetDurationFromComboBox()
        {
            if (DurationComboBox.SelectedItem is ComboBoxItem item)
            {
                return item.Content.ToString() switch
                {
                    "1.5 часа" => 1.5,
                    "2 часа" => 2.0,
                    _ => 1.0
                };
            }
            return 1.0;
        }

        private bool CanTrainerTakeClass(int trainerId, DateTime date, TimeSpan start, TimeSpan end, bool isIndividual)
        {
            using (var db = new AppDbContext())
            {
                var schedule = db.WorkSchedules.FirstOrDefault(ws =>
                    ws.TrainerId == trainerId &&
                    ws.WorkDate == date &&
                    ws.StartTime <= start &&
                    ws.EndTime >= end);

                if (schedule == null)
                {
                    MessageBox.Show("Тренер не работает в выбранное время.");
                    return false;
                }

                var overlappingClass = db.Class
                    .Include(c => c.ClassType)
                    .Include(c => c.WorkSchedule)
                    .Where(c =>
                        c.WorkSchedule.TrainerId == trainerId &&
                        c.WorkSchedule.WorkDate == date &&
                        (
                            (start >= c.StartTime && start < c.EndTime) ||
                            (end > c.StartTime && end <= c.EndTime) ||
                            (start <= c.StartTime && end >= c.EndTime)
                        ))
                    .ToList();

                foreach (var c in overlappingClass)
                {
                    bool existingIsIndividual = c.ClassType.ClassTypeName == "Индивидуальные";
                    if (existingIsIndividual || isIndividual)
                    {
                        MessageBox.Show("У тренера уже есть занятие в это время.");
                        return false;
                    }
                }

                return true;
            }
        }

        private void AuthorizedIndividual_Click(object sender, RoutedEventArgs e)
        {
            if (TrainerComboBox.SelectedValue == null || TimeSlotsListBox.SelectedItem == null || DurationComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите тренера, длительность и время.");
                return;
            }

            int trainerId = (int)TrainerComboBox.SelectedValue;
            double duration = GetDurationFromComboBox();

            string slot = TimeSlotsListBox.SelectedItem.ToString();
            var split = slot.Split(' ');
            DateTime date = DateTime.Parse(split[0]);
            TimeSpan startTime = TimeSpan.Parse(split[1]);
            TimeSpan endTime = startTime.Add(TimeSpan.FromHours(duration));

            using (var db = new AppDbContext())
            {
                int clientId = db.Clients.First(c => c.UserId == AuthorizationWin.currentUser.UserId).ClientId;

                var membership = db.ClientMemberships
                    .FirstOrDefault(cm => cm.ClientId == clientId && cm.EndDate.Date >= DateTime.UtcNow.Date && cm.StartDate <= DateTime.UtcNow.Date); 

                if (membership == null)
                {
                    MessageBox.Show("Нет активного абонемента.");
                    return;
                }

                if (IsClientBusyAt(clientId, date, startTime, endTime))
                {
                    MessageBox.Show("У вас уже есть занятие в это время.");
                    return;
                }

                if (!CanTrainerTakeClass(trainerId, date, startTime, endTime, true))
                {
                    return;
                }

                var schedule = db.WorkSchedules.First(ws =>
                    ws.TrainerId == trainerId &&
                    ws.WorkDate == date &&
                    ws.StartTime <= startTime &&
                    ws.EndTime >= endTime);

                var trainer = db.Trainers.First(t => t.TrainerId == trainerId);
                double totalPrice = trainer.IndividualPrice.Value * duration;


                var newClass = new Class
                {
                    ClassInfoId = 1,
                    ClassTypeId = 2,
                    HallId = 5,
                    WorkScheduleId = schedule.WorkScheduleId,
                    PeopleQuantity = 1,
                    StartTime = startTime,
                    EndTime = endTime,
                    Price = totalPrice
                };
                db.Class.Add(newClass);
                db.SaveChanges();

                db.ClassVisits.Add(new ClassVisits
                {
                    ClassId = newClass.ClassId,
                    ClientMembershipId = membership.ClientMembershipId
                });
                db.SaveChanges();

                var classPayment = new ClassPayments
                {
                    ClassId = newClass.ClassId,
                    PaymentDate = null,
                    Price = totalPrice,
                    ClientId = clientId
                };
                db.ClassPayments.Add(classPayment);
                db.SaveChanges();

                MessageBox.Show("Вы успешно записались!");
                LoadIndividualClasses();
                UpdateAvailableTimeSlots();
            }
        }

        private bool IsClientBusyAt(int clientId, DateTime date, TimeSpan start, TimeSpan end)
        {
            using (var db = new AppDbContext())
            {
                var busyVisits = db.ClassVisits
                    .Include(cv => cv.Class)
                    .ThenInclude(c => c.WorkSchedule)
                    .Where(cv =>
                        cv.ClientMembership.ClientId == clientId &&
                        cv.Class.WorkSchedule.WorkDate == date)
                    .ToList();

                foreach (var visit in busyVisits)
                {
                    var existingStart = visit.Class.StartTime;
                    var existingEnd = visit.Class.EndTime;

                    // Проверка на пересечение
                    if (start < existingEnd && end > existingStart)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private void AuthorizedGroup_Click(object sender, RoutedEventArgs e)
        {
            var selectedClass = AvailableGroupList.SelectedItem;

            if (selectedClass != null)
                SignUpOnGroupClass(selectedClass);
            else
                MessageBox.Show("Вы не выбрали занятие для записи");
        }

        private void SignUpOnGroupClass(dynamic selectedClass)
        {
            if (AvailableGroupList.SelectedItem == null)
            {
                MessageBox.Show("Выберите занятие из списка.");
                return;
            }

            int classId = selectedClass.ClassId;

            using (var db = new AppDbContext())
            {
                int clientId = db.Clients
                    .Where(c => c.UserId == AuthorizationWin.currentUser.UserId)
                    .Select(c => c.ClientId)
                    .FirstOrDefault();

                var membership = db.ClientMemberships
                    .FirstOrDefault(cm => cm.ClientId == clientId && cm.EndDate.Date >= DateTime.UtcNow.Date && cm.StartDate.Date <= DateTime.UtcNow.Date);

                if (membership == null)
                {
                    MessageBox.Show("У вас нет активного абонемента.");
                    return;
                }

                var classObj = db.Class
                    .Include(c => c.WorkSchedule)
                    .Include(c => c.ClassVisits)
                    .FirstOrDefault(c => c.ClassId == classId);

                if (classObj == null)
                {
                    MessageBox.Show("Занятие не найдено.");
                    return;
                }

                // Проверка: клиент уже записан?
                bool alreadySigned = db.ClassVisits.Any(cv =>
                    cv.ClientMembership.ClientId == clientId &&
                    cv.ClassId == classId);

                if (alreadySigned)
                {
                    MessageBox.Show("Вы уже записаны на это занятие.");
                    return;
                }

                var clientVisits = db.ClassVisits
                    .Include(cv => cv.Class)
                        .ThenInclude(c => c.WorkSchedule)
                    .Where(cv => cv.ClientMembership.ClientId == clientId &&
                                 cv.Class.WorkSchedule.WorkDate == classObj.WorkSchedule.WorkDate)
                    .ToList();

                var newStart = classObj.StartTime;
                var newEnd = classObj.EndTime;

                bool hasOverlap = clientVisits.Any(v =>
                {
                    var existing = v.Class;
                    return newStart < existing.EndTime && newEnd > existing.StartTime;
                });

                if (hasOverlap)
                {
                    MessageBox.Show("У вас уже есть занятие в это время.");
                    return;
                }

                // Проверка на свободные места
                if (classObj.ClassVisits.Count >= classObj.PeopleQuantity)
                {
                    MessageBox.Show("На это занятие нет свободных мест.");
                    return;
                }

                // Запись
                db.ClassVisits.Add(new ClassVisits
                {
                    ClassId = classObj.ClassId,
                    ClientMembershipId = membership.ClientMembershipId
                });
                db.SaveChanges();

                DateTime classDate = db.Class
                    .Where(c => c.ClassId == classId)
                    .Select(c => c.WorkSchedule.WorkDate)
                    .FirstOrDefault();

                //Добавление записи в таблицу ClassPayments
                var classPayment = new ClassPayments
                {
                    ClassId = classObj.ClassId,
                    PaymentDate = classObj.Price == 0 ? DateTime.SpecifyKind(classDate, DateTimeKind.Utc) : null,
                    Price = classObj.Price ?? 0,
                    ClientId = clientId
                };
                db.ClassPayments.Add(classPayment);
                db.SaveChanges();

                MessageBox.Show("Вы успешно записались на групповое занятие!");
                LoadGroupClasses();
                LoadAvailableGroupClasses();
            }
        }

        private void LoadAvailableGroupClasses()
        {
            using (var db = new AppDbContext())
            {
                int clientId = db.Clients
                    .Where(c => c.UserId == AuthorizationWin.currentUser.UserId)
                    .Select(c => c.ClientId)
                    .FirstOrDefault();

                var today = DateTime.Today;

                var availableGroupClasses = db.Class
                    .Include(c => c.ClassType)
                    .Include(c => c.WorkSchedule).ThenInclude(ws => ws.Trainer)
                    .Include(c => c.Hall)
                    .Include(c => c.ClassInfo)
                    .Include(c => c.ClassVisits).ThenInclude(cv => cv.ClientMembership)
                    .Where(c =>
                        c.ClassType.ClassTypeName != "Индивидуальные" &&
                        c.WorkSchedule.WorkDate >= today &&
                        !c.ClassVisits.Any(cv => cv.ClientMembership != null && cv.ClientMembership.ClientId == clientId))
                    .Select(c => new
                    {
                        c.ClassId,
                        ClassName = c.ClassInfo.ClassName,
                        Date = c.WorkSchedule.WorkDate,
                        Time = $"{c.StartTime:hh\\:mm} - {c.EndTime:hh\\:mm}",
                        HallName = c.Hall.HallName,
                        TrainerName = $"{c.WorkSchedule.Trainer.LastName} {c.WorkSchedule.Trainer.FirstName}",
                        Price = c.Price,
                        FreeSpots = c.PeopleQuantity - c.ClassVisits.Count
                    })
                    .ToList();

                AvailableGroupList.ItemsSource = availableGroupClasses;
            }
        }

        private void OpenClassDetails(dynamic selectedItem)
        {
            try
            {
                if (selectedItem != null)
                {
                    int classId = selectedItem.ClassId;
                    ClassDetailsWindow cdw = new ClassDetailsWindow(classId, "schedulePage");
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

        private dynamic GetSelectedItemFromContext(object sender)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return null;

            var contextMenu = menuItem.Parent as ContextMenu;
            if (contextMenu == null) return null;

            var border = contextMenu.PlacementTarget as Border;
            if (border == null) return null;

            // Ищем родительский ListViewItem
            var listViewItem = FindVisualParent<ListViewItem>(border);
            if (listViewItem == null) return null;

            return listViewItem.DataContext;
        }

        private T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            if (parentObject is T parent)
                return parent;
            else
                return FindVisualParent<T>(parentObject);
        }

        private void AuthorizedIndividualList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dynamic selectedItem = AuthorizedIndividualList.SelectedItem;
            if (selectedItem != null)
                OpenClassDetails(selectedItem);
            else
                MessageBox.Show("Вы не выбрали занятие, о котором хотите загрузить информацию");
        }

        private void AuthorizedGroupList_MouseDoubleClick(object sender, MouseButtonEventArgs e)

        {
            dynamic selectedItem = AuthorizedGroupList.SelectedItem;
            if (selectedItem != null)
                OpenClassDetails(selectedItem);
            else
                MessageBox.Show("Вы не выбрали занятие, о котором хотите загрузить информацию");
        }
        private void AvailableGroupList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            dynamic selectedItem = AvailableGroupList.SelectedItem;
            if (selectedItem != null)
                OpenClassDetails(selectedItem);
            else
                MessageBox.Show("Вы не выбрали занятие, о котором хотите загрузить информацию");
        }

        private void SignUpGroupClassContext_Click(object sender, RoutedEventArgs e) 
        {
            var selectedClass = GetSelectedItemFromContext(sender);
            if (selectedClass != null)
                SignUpOnGroupClass(selectedClass);
            else
                MessageBox.Show("Вы не выбрали занятие для записи");
        }

        private void AvailableGroupDetailsContext_Click(object sender, RoutedEventArgs e) 
        {
            var selectedItem = GetSelectedItemFromContext(sender);
            if (selectedItem != null)
                OpenClassDetails(selectedItem);
            else
                MessageBox.Show("Вы не выбрали занятие, о котором хотите загрузить информацию");
        }

        private void IndividualClassDetailsContext_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = GetSelectedItemFromContext(sender);
            if (selectedItem != null)
                OpenClassDetails(selectedItem);
            else
                MessageBox.Show("Вы не выбрали занятие, о котором хотите загрузить информацию");
        }

        private void GroupClassDetailsContext_Click(object sender, RoutedEventArgs e) 
        {
            var selectedItem = GetSelectedItemFromContext(sender);
            if (selectedItem != null)
                OpenClassDetails(selectedItem);
            else
                MessageBox.Show("Вы не выбрали занятие, о котором хотите загрузить информацию");
        }

        private void CancelClientClass(int classId, bool isIndividual)
        {
            try
            {
                using var db = new AppDbContext();

                var clientId = AuthorizationWin.currentUser.Client.ClientId;
                var todayUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);

                var classVisit = db.ClassVisits
                    .Include(cv => cv.ClientMembership)
                    .FirstOrDefault(cv => cv.ClassId == classId && cv.ClientMembership.ClientId == clientId);

                if (classVisit == null)
                {
                    MessageBox.Show("Запись не найдена.");
                    return;
                }

                if (classVisit.Visited)
                {
                    MessageBox.Show("Нельзя отменить занятие, которое уже посещено.");
                    return;
                }

                var payment = db.ClassPayments
                    .FirstOrDefault(cp => cp.ClassId == classId && cp.ClientId == clientId);

                if (payment != null && payment.PaymentDate != null)
                {
                    var client = db.Clients.FirstOrDefault(c => c.ClientId == clientId);
                    if (client != null)
                    {
                        client.Balance += (decimal)payment.Price;

                        db.ClientTransactions.Add(new ClientTransaction
                        {
                            ClientId = client.ClientId,
                            OperationDescription = isIndividual
                                ? "Возврат за отмену индивидуального занятия"
                                : "Возврат за отмену группового занятия",
                            PaymentWay = "На баланс",
                            Amount = (decimal)payment.Price,
                            TransactionType = "возврат",
                            TransactionDate = todayUtc
                        });
                    }
                }

                if (payment != null)
                    db.ClassPayments.Remove(payment);

                db.ClassVisits.Remove(classVisit);

                if (isIndividual)
                {
                    var classToDelete = db.Class.FirstOrDefault(c => c.ClassId == classId);
                    if (classToDelete != null)
                        db.Class.Remove(classToDelete);
                }

                db.SaveChanges();

                if (isIndividual)
                {
                    LoadIndividualClasses();
                    UpdateAvailableTimeSlots();
                }
                else
                {
                    LoadGroupClasses();
                    LoadAvailableGroupClasses();
                }

                MessageBox.Show("Запись на занятие отменена.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отмене: {ex.Message}");
            }
        }

        private void CancelIndividualClassContext_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.CommandParameter is int classId)
            {
                CancelClientClass(classId, isIndividual: true);
            }
        }

        private void CancelGroupClassContext_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.CommandParameter is int classId)
            {
                CancelClientClass(classId, isIndividual: false);
            }
        }

    }
}