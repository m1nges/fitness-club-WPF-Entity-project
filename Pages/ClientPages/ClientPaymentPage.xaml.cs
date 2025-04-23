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
    public partial class ClientPaymentPage : Page
    {
        DateTime todayUtc = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        int membershipId = 0;
        public ClientPaymentPage()
        {
            InitializeComponent();
            GridLoaded();
        }

        public void GridLoaded()
        {
            GetMemberShipId();
            LoadServicesDebts();
            LoadMembershipDebts();
            LoadLockerDebt();
            LoadClassDebts();
        }

        public void GetMemberShipId()
        {
            using (var db = new AppDbContext())
            {
                membershipId = db.ClientMemberships
                    .Where(cm => cm.ClientId == AuthorizationWin.currentUser.Client.ClientId && cm.EndDate >= todayUtc && cm.StartDate <= todayUtc)
                    .Select(cm => cm.ClientMembershipId)
                    .FirstOrDefault();
            }
        }

        public void LoadServicesDebts()
        {
            using (var db = new AppDbContext())
            {
                var servicesDebts = db.ServicesPayments
                    .Include(sp => sp.Service)
                        .ThenInclude(s => s.ServiceType)
                    .Where(sp => sp.ClientId == AuthorizationWin.currentUser.Client.ClientId && sp.PaymentDate == null)
                    .Select(sp => new ServiceDebt
                    {
                        ServiceId = sp.ServiceId,
                        ServiceName = sp.Service.ServiceName,
                        ServiceType = sp.Service.ServiceType.TypeName,
                        Description = sp.Service.Description,
                        Price = sp.Price,
                        ProvisionDate = sp.Service.MembershipServices.Where(ms=>ms.ServiceId == sp.ServiceId && ms.ClientMembershipId == membershipId)
                            .Select(ms => ms.ProvisionDate)
                            .FirstOrDefault()
                    })
                    .ToList();
                if(servicesDebts.Count != 0)
                {
                    unPayedServicesList.ItemsSource = servicesDebts;

                    double totalDebt = servicesDebts.Sum(d => d.Price);
                    payForAllServicesDebts.Content = $"Оплатить все ({totalDebt} руб.)";
                }
                else
                {
                    unPayedServicesList.Visibility = Visibility.Collapsed;
                    payForAllServicesDebts.Visibility = Visibility.Collapsed;
                    servicesToBeProceedTb.Text = "Нет услуг для оплаты";
                }
            }
        }

        public void LoadMembershipDebts()
        {
            using (var db = new AppDbContext())
            {
                var clientId = AuthorizationWin.currentUser.Client.ClientId;

                // Загружаем неоплаченные платежи
                var debts = db.MembershipPayments
                    .Include(mp => mp.Membership)
                        .ThenInclude(m => m.MembershipType)
                    .Where(mp => mp.ClientId == clientId && mp.PaymentDate == null)
                    .ToList();

                // Получаем client_membership для всех этих membershipId
                var membershipIds = debts.Select(mp => mp.MembershipId).ToList();

                var membershipsInfo = db.ClientMemberships
                    .Where(cm => cm.ClientId == clientId && membershipIds.Contains(cm.MembershipId))
                    .ToList();

                // Объединяем вручную
                var result = debts.Select(mp =>
                {
                    var cm = membershipsInfo.FirstOrDefault(x => x.MembershipId == mp.MembershipId);
                    return new MembershipDebt
                    {
                        MembershipId = mp.MembershipId,
                        MembershipName = mp.Membership.MembershipName,
                        MembershipTypeName = mp.Membership.MembershipType.MembershipTypeName,
                        StartDate = cm?.StartDate ?? DateTime.MinValue,
                        EndDate = cm?.EndDate ?? DateTime.MinValue,
                        Price = mp.Price
                    };
                }).ToList();

                // Вывод
                if (result.Any())
                {
                    unPayedMembershipList.ItemsSource = result;
                    double total = result.Sum(x => x.Price);
                    payForAllMembershipDebts.Content = $"Оплатить все ({total} руб.)";
                }
                else
                {
                    unPayedMembershipList.Visibility = Visibility.Collapsed;
                    payForAllMembershipDebts.Visibility = Visibility.Collapsed;
                    membershipsToBeProceedTb.Text = "Нет абонементов для оплаты";
                }
            }
        }

        public void UpdatePaidServices(List<int> serviceIds)
        {
            using (var db = new AppDbContext())
            {
                // Находим все неоплаченные платежи для выбранных услуг
                var paymentsToUpdate = db.ServicesPayments
                    .Where(sp => sp.ClientId == AuthorizationWin.currentUser.Client.ClientId &&
                                sp.PaymentDate == null &&
                                serviceIds.Contains(sp.ServiceId))
                    .ToList();

                // Помечаем как оплаченные
                foreach (var payment in paymentsToUpdate)
                {
                    payment.PaymentDate = todayUtc;
                }
                db.SaveChanges();

                // Обновляем список
                LoadServicesDebts();
            }
        }

        public void UpdatePaidMemberships(List<int> membershipIds)
        {
            using (var db = new AppDbContext())
            {
                // Получаем сегодняшнюю дату в UTC
                DateTime todayUtc = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);

                // Находим все неоплаченные платежи по указанным абонементам
                var paymentsToUpdate = db.MembershipPayments
                    .Where(mp => mp.ClientId == AuthorizationWin.currentUser.Client.ClientId &&
                                 mp.PaymentDate == null &&
                                 membershipIds.Contains(mp.MembershipId))
                    .ToList();

                // Помечаем как оплаченные
                foreach (var payment in paymentsToUpdate)
                {
                    payment.PaymentDate = todayUtc;
                }

                db.SaveChanges();

                // Обновляем отображение задолженности
                LoadMembershipDebts();
            }
        }

        public void LoadLockerDebt()
        {
            using (var db = new AppDbContext())
            {
                var lockerDebt = db.RentedLockers
                    .Include(rl => rl.Locker)
                    .Include(rl => rl.ClientMembership)
                    .Where(rl => rl.ClientMembership.ClientId == AuthorizationWin.currentUser.Client.ClientId &&
                                 rl.PaymentDate == null)
                    .Select(rl => new LockerDebt
                    {
                        RentLockerId = rl.RentLockerId,
                        LockerId = rl.LockerId,
                        LockerZone = rl.Locker.LockerZone,
                        StartDate = rl.StartDate,
                        EndDate = rl.EndDate,
                        RentPrice = rl.RentPrice
                    })
                    .FirstOrDefault();

                if (lockerDebt != null)
                {
                    lockerPaymentPanel.DataContext = lockerDebt;
                }
                else
                {
                    lockerPaymentPanel.Visibility = Visibility.Collapsed;
                    lockerTitle.Text = "Нет задолженности по шкафчику";
                }
            }
        }

        public void LoadClassDebts()
        {
            using (var db = new AppDbContext())
            {
                var classDebts = db.ClassPayments
                    .Include(cp => cp.Class)
                        .ThenInclude(c => c.ClassInfo)
                    .Include(cp => cp.Class)
                        .ThenInclude(c => c.ClassType)
                    .Include(cp => cp.Class)
                        .ThenInclude(c => c.WorkSchedule)
                    .Where(cp => cp.ClientId == AuthorizationWin.currentUser.Client.ClientId &&
                                 cp.PaymentDate == null &&
                                 cp.Class.WorkSchedule != null &&
                                 cp.Class.WorkSchedule.WorkDate != null &&
                                 cp.Class.WorkScheduleId != null)
                    .Select(cp => new ClassDebt
                    {
                        ClassId = cp.ClassId,
                        ClassName = cp.Class.ClassInfo.ClassName,
                        Description = cp.Class.ClassInfo.Description,
                        TrainerName = $"{cp.Class.WorkSchedule.Trainer.LastName} {cp.Class.WorkSchedule.Trainer.LastName} {cp.Class.WorkSchedule.Trainer.Patronymic}",
                        Date = cp.Class.WorkSchedule.WorkDate,
                        StartTime = cp.Class.StartTime,
                        Price = cp.Price
                    })
                    .ToList();

                if (classDebts.Any())
                {
                    unPayedClassList.ItemsSource = classDebts;
                    double total = classDebts.Sum(x => x.Price);
                    payAllClassesBtn.Content = $"Оплатить все ({total} руб.)";
                }
                else
                {
                    unPayedClassList.Visibility = Visibility.Collapsed;
                    payAllClassesBtn.Visibility = Visibility.Collapsed;
                    classTitle.Text = "Нет занятий для оплаты";
                }
            }
        }

        public void UpdateLockerPayment(int rentLockerId)
        {
            using (var db = new AppDbContext())
            {
                var locker = db.RentedLockers
                    .FirstOrDefault(rl => rl.RentLockerId == rentLockerId &&
                                          rl.ClientMembership.ClientId == AuthorizationWin.currentUser.Client.ClientId &&
                                          rl.PaymentDate == null);

                if (locker != null)
                {
                    locker.PaymentDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
                    db.SaveChanges();
                    LoadLockerDebt();
                }
            }
        }

        public void UpdateMultipleClassPayments(List<int> classIds)
        {
            using (var db = new AppDbContext())
            {
                var classPayments = db.ClassPayments
                    .Where(cp => cp.ClientId == AuthorizationWin.currentUser.Client.ClientId &&
                                 cp.PaymentDate == null &&
                                 classIds.Contains(cp.ClassId))
                    .ToList();

                var todayUtc = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);

                foreach (var cp in classPayments)
                {
                    cp.PaymentDate = todayUtc;
                }

                db.SaveChanges();
                LoadClassDebts();
            }
        }

        private void payService_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedItem = button?.DataContext as ServiceDebt;
            if (selectedItem != null)
            {
                var paymentWindow = new PaymentWindow($"услугу '{selectedItem.ServiceName}'", (int)selectedItem.Price);
                if (paymentWindow.ShowDialog() == true && paymentWindow.PaymentSuccess)
                {
                    // Обновляем только одну оплаченную услугу
                    UpdatePaidServices(new List<int> { selectedItem.ServiceId });
                }
            }
        }

        private void payForAllServicesDebts_Click(object sender, RoutedEventArgs e)
        {
            var allDebts = unPayedServicesList.ItemsSource as List<ServiceDebt>;
            if (allDebts == null || !allDebts.Any())
            {
                MessageBox.Show("Нет услуг для оплаты");
                return;
            }

            double totalSum = allDebts.Sum(d => d.Price);
            var paymentWindow = new PaymentWindow("все услуги", (int)totalSum);

            if (paymentWindow.ShowDialog() == true && paymentWindow.PaymentSuccess)
            {
                UpdatePaidServices(allDebts.Select(d => d.ServiceId).ToList());
                payForAllServicesDebts.Visibility = Visibility.Collapsed;
            }
        }

        private void payMembership_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedItem = button?.DataContext as MembershipDebt;
            if (selectedItem != null)
            {
                var paymentWindow = new PaymentWindow($"'{selectedItem.MembershipName}'", (int)selectedItem.Price);
                if (paymentWindow.ShowDialog() == true && paymentWindow.PaymentSuccess)
                {
                    // Обновляем только один оплаченный абонемент
                    UpdatePaidMemberships(new List<int> { selectedItem.MembershipId});
                }
            }
        }

        private void payForAllMembershipDebts_Click(object sender, RoutedEventArgs e)
        {
            var allDebts = unPayedMembershipList.ItemsSource as List<MembershipDebt>;
            if (allDebts == null || !allDebts.Any())
            {
                MessageBox.Show("Нет абонементов для оплаты");
                return;
            }

            double totalSum = allDebts.Sum(d => d.Price);
            var paymentWindow = new PaymentWindow("все абонементы", (int)totalSum);

            if (paymentWindow.ShowDialog() == true && paymentWindow.PaymentSuccess)
            {
                UpdatePaidMemberships(allDebts.Select(d => d.MembershipId).ToList());
                payForAllMembershipDebts.Visibility = Visibility.Collapsed;
            }
        }

        private void payLockerBtn_Click(object sender, RoutedEventArgs e)
        {
            var debt = lockerPaymentPanel.DataContext as LockerDebt;
            if (debt != null)
            {
                var paymentWindow = new PaymentWindow($"аренду шкафчика #{debt.LockerId}", debt.RentPrice);
                if (paymentWindow.ShowDialog() == true && paymentWindow.PaymentSuccess)
                {
                    UpdateLockerPayment(debt.RentLockerId);
                }
            }
        }

        private void payAllClassesBtn_Click(object sender, RoutedEventArgs e)
        {
            var allDebts = unPayedClassList.ItemsSource as List<ClassDebt>;

            if (allDebts == null || !allDebts.Any())
            {
                MessageBox.Show("Нет занятий для оплаты");
                return;
            }

            double totalSum = allDebts.Sum(d => d.Price);
            var paymentWindow = new PaymentWindow("все занятия", (int)totalSum);

            if (paymentWindow.ShowDialog() == true && paymentWindow.PaymentSuccess)
            {
                UpdateMultipleClassPayments(allDebts.Select(d => d.ClassId).ToList());
                payAllClassesBtn.Visibility = Visibility.Collapsed;
            }
        }

        private void payClass_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var debt = button?.DataContext as ClassDebt;

            if (debt != null)
            {
                var paymentWindow = new PaymentWindow($"занятие '{debt.ClassName}'", (int)debt.Price);
                if (paymentWindow.ShowDialog() == true && paymentWindow.PaymentSuccess)
                {
                    UpdateMultipleClassPayments(new List<int> { debt.ClassId });
                }
            }
        }
    }

    public class ServiceDebt
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceType { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public DateTime? ProvisionDate { get; set; }
    }

    public class MembershipDebt
    {
        public int MembershipId { get; set; }
        public string MembershipName { get; set; }
        public string MembershipTypeName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Price { get; set; }
    }

    public class LockerDebt
    {
        public int RentLockerId { get; set; }
        public int LockerId { get; set; }
        public string LockerZone { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RentPrice { get; set; }
    }

    public class ClassDebt
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public string TrainerName { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public double Price { get; set; }
    }

}
