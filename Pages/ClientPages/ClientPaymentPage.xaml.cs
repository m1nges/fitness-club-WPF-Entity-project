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
    /// Логика взаимодействия для ClientPaymentPage.xaml
    /// </summary>
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
}
