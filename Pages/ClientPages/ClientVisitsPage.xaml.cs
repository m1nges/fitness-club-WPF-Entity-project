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
                        Price = cv.Class.Price,
                        Visited = cv.Visited ? "Да" : "Нет"
                    })
                    .OrderByDescending(x => x.Date)
                    .ToList();

                ClientVisitsList.ItemsSource = pastVisits;
            }
        }

        private void RefundVisitContext_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem || menuItem.CommandParameter is not int classId)
            {
                MessageBox.Show("Ошибка при определении занятия для возврата.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using var db = new AppDbContext();
            int clientId = AuthorizationWin.currentUser.Client.ClientId;

            // Подгружаем посещение вместе со всей нужной информацией
            var visit = db.ClassVisits
                .Include(cv => cv.Class)
                    .ThenInclude(c => c.ClassType)
                .Include(cv => cv.Class)
                    .ThenInclude(c => c.ClassInfo)
                .Include(cv => cv.ClientMembership)
                .FirstOrDefault(cv => cv.ClassId == classId && cv.ClientMembership.ClientId == clientId);

            if (visit == null)
            {
                MessageBox.Show("Посещение не найдено. Возможно, вы уже отменили занятие.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка: посещено ли занятие
            if (visit.Visited)
            {
                MessageBox.Show("Нельзя вернуть средства за уже посещённое занятие.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получаем информацию об оплате
            var payment = db.ClassPayments
                .FirstOrDefault(cp => cp.ClassId == classId && cp.ClientId == clientId);

            if (payment == null)
            {
                MessageBox.Show("Оплата за это занятие не найдена. Возможно, оно было бесплатным.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                db.ClassVisits.Remove(visit); // просто удаляем посещение без возврата
                db.SaveChanges();
                LoadClientPastVisits();
                return;
            }

            if (payment.PaymentDate == null)
            {
                MessageBox.Show("Занятие ещё не оплачено. Сначала оплатите, чтобы можно было оформить возврат.", "Информация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var client = db.Clients.FirstOrDefault(c => c.ClientId == clientId);
            if (client == null)
            {
                MessageBox.Show("Клиент не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Возврат средств
            client.Balance += (decimal)payment.Price;

            db.ClientTransactions.Add(new ClientTransaction
            {
                ClientId = client.ClientId,
                OperationDescription = $"Возврат за занятие: {visit.Class.ClassInfo.ClassName}",
                PaymentWay = "На баланс",
                Amount = (decimal)payment.Price,
                TransactionType = "возврат",
                TransactionDate = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc)
            });

            db.ClassPayments.Remove(payment);
            db.ClassVisits.Remove(visit);

            // Удаляем занятие, если индивидуальное
            if (visit.Class.ClassType.ClassTypeId == 2)
            {
                var individualClass = db.Class.FirstOrDefault(c => c.ClassId == classId);
                if (individualClass != null)
                    db.Class.Remove(individualClass);
            }

            db.SaveChanges();

            MessageBox.Show($"Занятие отменено, средства возвращены на ваш баланс. Текущий баланс: {client.Balance}", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadClientPastVisits();
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
