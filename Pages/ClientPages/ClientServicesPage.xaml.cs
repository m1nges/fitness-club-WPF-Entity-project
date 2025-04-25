using fitness_club.Model;
using fitness_club.Windows;
using fitness_club.Classes;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace fitness_club.Pages.ClientPages
{
    public partial class ClientServicesPage : Page
    {
        int clientMemberShipId = 0;
        private List<FreeServiceViewModel> freeServicesGlobal;
        int rentedLockerId = 0;

        DateTime todayUtc = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        string? clientGender = "";
        public ClientServicesPage()
        {
            InitializeComponent();
            GridLoaded();
        }

        public void GridLoaded()
        {
            GetMemberShipClientId();
            clientGender = CheckClientGender();
            pickServiceProvisionDate.SelectedDate = todayUtc;
            pickServiceDateForMembership.SelectedDate = todayUtc;
            servicesHistoryComboBox.SelectedIndex = 0;
            //Методы для услуг
            LoadAvailableServicesByMemberShip();
            LoadAvailableServices();
            LoadClientsServices(servicesHistoryComboBox.SelectedIndex);
            //Методы для шкафчиков
            HasUserRentedLocker();
            rentEndDp.SelectedDate = todayUtc.AddDays(1);
            lockerZoneComboBox.ItemsSource = LoadLockerZone();
            if (clientGender == "Мужской")
                lockerZoneComboBox.SelectedItem = "Мужская раздевалка";
            else if (clientGender == "Женский")
                lockerZoneComboBox.SelectedItem = "Женская раздевалка";
            else
                lockerZoneComboBox.SelectedIndex = 0;

            if (lockerZoneComboBox.SelectedItem == null)
                return;
            if (rentedLockerId == 0)
            {
                LoadLockers();
            }
        }

        public void HasUserRentedLocker()
        {
            using (var db = new AppDbContext())
            {
                var rentedLocker = db.RentedLockers
                    .Where(rl => rl.ClientMembershipId == clientMemberShipId && rl.EndDate > todayUtc)
                    .FirstOrDefault();
                if (rentedLocker != null)
                {
                    rentedLockerId = rentedLocker.LockerId;
                    rentLockerSP.Visibility = Visibility.Collapsed;
                    rentedLockerIdTb.Text = $"Номер арендованного шкафчика: {rentedLockerId}";
                    userHasRentedLocker.Visibility = Visibility.Visible;
                }
            }
        }

        public string? CheckClientGender()
        {
            using (var db = new AppDbContext())
            {
                string? clientGender = db.Clients
                    .Where(c => c.ClientId == AuthorizationWin.currentUser.Client.ClientId)
                    .Select(c => c.Gender.GenderName)
                    .FirstOrDefault();
                return clientGender;
            }
        }

        public List<string> LoadLockerZone()
        {
            using (var db = new AppDbContext())
            {
                if (clientGender == "Мужчина")
                {
                    return db.Lockers
                        .Select(l => l.LockerZone)
                        .Distinct()
                        .Where(l => l != "Женская раздевалка")
                        .ToList();
                }
                else if (clientGender == "Женщина")
                {
                    return db.Lockers
                        .Select(l => l.LockerZone)
                        .Distinct()
                        .Where(l => l != "Мужская раздевалка")
                        .ToList();
                }
                else
                {
                    return db.Lockers
                        .Select(l => l.LockerZone)
                        .Distinct()
                        .ToList();
                }
            }
        }


        public void LoadLockers()
        {

            string selectedZone = lockerZoneComboBox.SelectedItem.ToString();

            using (var db = new AppDbContext())
            {
                if (rentedLockerId != 0)
                {
                    rentEndDp.SelectedDate = db.RentedLockers
                            .Where(rl => rl.LockerId == rentedLockerId)
                            .Select(rl => rl.EndDate)
                            .FirstOrDefault();
                }

                var rentedLockerIds = db.RentedLockers
                    .Where(rl => rl.EndDate >= todayUtc)
                    .Select(rl => rl.LockerId)
                    .ToHashSet();

                var availableLockers = db.Lockers
                    .Where(l => l.LockerZone == selectedZone && !rentedLockerIds.Contains(l.LockerId))
                    .ToList();

                availableLockersList.ItemsSource = availableLockers;
            }
        }


        public int CalculateLockerPrice(dynamic selectedLocker)
        {
            var endDate = rentEndDp.SelectedDate.HasValue
                            ? DateTime.SpecifyKind(rentEndDp.SelectedDate.Value, DateTimeKind.Utc)
                            : throw new Exception("Дата не выбрана");

            if (endDate <= todayUtc)
                MessageBox.Show("Дата окончания должна быть позже даты начала");

            int days = (endDate - todayUtc).Days;
            float dailyPrice = selectedLocker.MonthPrice / 30;
            float total = dailyPrice * days;

            return (int)Math.Floor(total);
        }

        public void RentLocker(dynamic selectedLocker)
        {
            using (var db = new AppDbContext())
            {
                if (rentedLockerId != 0)
                {
                    db.RentedLockers
                        .Remove(db.RentedLockers
                            .Where(rl => rl.ClientMembershipId == clientMemberShipId && rl.EndDate > todayUtc)
                            .FirstOrDefault());
                }

                var rentEndDate = rentEndDp.SelectedDate.HasValue
                            ? DateTime.SpecifyKind(rentEndDp.SelectedDate.Value, DateTimeKind.Utc)
                            : throw new Exception("Дата не выбрана");
                if (rentEndDate > db.ClientMemberships
                                    .Where(cm => cm.ClientMembershipId == clientMemberShipId)
                                    .Select(cm => cm.EndDate)
                                    .FirstOrDefault())
                {
                    MessageBox.Show("Дата окончания аренды не может быть позже даты окончания абонемента.");
                }
                else
                {
                    var rentedLocker = new RentedLocker
                    {
                        ClientMembershipId = clientMemberShipId,
                        LockerId = selectedLocker.LockerId,
                        StartDate = todayUtc,
                        EndDate = rentEndDate,
                        RentPrice = CalculateLockerPrice(selectedLocker),
                        PaymentDate = null
                    };
                    db.RentedLockers.Add(rentedLocker);
                    db.SaveChanges();
                    if (MessageBox.Show("Шкафчик успешно арендован.", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
                    {
                        HasUserRentedLocker();
                    }

                }
            }
        }

        public void LoadAvailableServicesByMemberShip()
        {
            using (var db = new AppDbContext())
            {
                freeServicesGlobal = db.Services
                    .Include(s => s.ServiceType)
                    .Where(s => s.FreeUsageLimit > 0)
                    .Select(s => new FreeServiceViewModel
                    {
                        ServiceId = s.ServiceId,
                        ServiceType = s.ServiceType.TypeName,
                        ServiceName = s.ServiceName,
                        Description = s.Description,
                        UsageCount = db.MembershipServices.Count(ms => ms.ServiceId == s.ServiceId && ms.ClientMembershipId == clientMemberShipId) > s.FreeUsageLimit
                            ? s.FreeUsageLimit
                            : db.MembershipServices.Count(ms => ms.ServiceId == s.ServiceId && ms.ClientMembershipId == clientMemberShipId),
                        RemainingCount = s.FreeUsageLimit - db.MembershipServices.Count(ms => ms.ServiceId == s.ServiceId && ms.ClientMembershipId == clientMemberShipId) < 0 ? 0
                            : s.FreeUsageLimit - db.MembershipServices.Count(ms => ms.ServiceId == s.ServiceId && ms.ClientMembershipId == clientMemberShipId)
                    })
                    .ToList();

                servicesAvailableByMembership.ItemsSource = freeServicesGlobal;
            }
        }


        public void GetMemberShipClientId()
        {
            using (var db = new AppDbContext())
            {
                int clientmsId = db.ClientMemberships
                    .Where(cm => cm.ClientId == AuthorizationWin.currentUser.Client.ClientId &&
                                 cm.EndDate >= todayUtc && cm.StartDate <= todayUtc)
                    .Select(cm => cm.ClientMembershipId)
                    .FirstOrDefault();
                clientMemberShipId = clientmsId;
            }
        }

        public void LoadClientsServices(int selectionIndex)
        {
            using (var db = new AppDbContext())
            {
                IQueryable<MembershipService> baseQuery = db.MembershipServices
                    .Include(ms => ms.Service)
                    .Include(ms => ms.ClientMembership)
                    .Where(ms => ms.ClientMembershipId == clientMemberShipId);

                switch (selectionIndex)
                {
                    case 0: // Все услуги
                        servicesHistoryList.ItemsSource = baseQuery
                            .OrderByDescending(ms => ms.ProvisionDate)
                            .Select(ms => new
                            {
                                ms.ServiceId,
                                ms.Service.ServiceName,
                                ms.Service.Description,
                                ms.Service.Price,
                                ms.ProvisionDate,
                            })
                            .ToList();
                        break;

                    case 1: // Использованные услуги (до текущей даты)
                        servicesHistoryList.ItemsSource = baseQuery
                            .Where(ms => ms.ProvisionDate < todayUtc)
                            .OrderByDescending(ms => ms.ProvisionDate)
                            .Select(ms => new
                            {
                                ms.ServiceId,
                                ms.Service.ServiceName,
                                ms.Service.Description,
                                ms.Service.Price,
                                ms.ProvisionDate,
                            })
                            .ToList();
                        break;

                    case 2: // Неиспользованные услуги (текущая дата и позже)
                        servicesHistoryList.ItemsSource = baseQuery
                            .Where(ms => ms.ProvisionDate >= todayUtc)
                            .OrderBy(ms => ms.ProvisionDate)
                            .Select(ms => new
                            {
                                ms.ServiceId,
                                ms.Service.ServiceName,
                                ms.Service.Description,
                                ms.Service.Price,
                                ms.ProvisionDate,
                            })
                            .ToList();
                        break;
                }
            }
        }

        public void LoadAvailableServices()
        {
            using (var db = new AppDbContext())
            {
                var paidOnly = db.Services
                    .Include(s => s.ServiceType)
                    .Where(s => s.FreeUsageLimit == 0)
                    .Select(s => new
                    {
                        s.ServiceName,
                        s.Description,
                        s.Price,
                        ServiceType = s.ServiceType.TypeName
                    })
                    .ToList();

                var exhaustedFreeServices = freeServicesGlobal
                    .Where(f => f.RemainingCount <= 0)
                    .Select(f => new
                    {
                        f.ServiceName,
                        f.Description,
                        Price = db.Services.Where(s => s.ServiceId == f.ServiceId).Select(s => s.Price).FirstOrDefault(),
                        f.ServiceType
                    })
                    .ToList();

                var combined = paidOnly
                    .Concat(exhaustedFreeServices)
                    .OrderBy(s => s.ServiceType)
                    .ThenBy(s => s.ServiceName)
                    .ToList();

                availableServicesList.ItemsSource = combined;
            }
        }


        public void OrderService(dynamic selectedItem, DateTime provisionDate, bool isServiceFree)
        {
            try
            {
                using (var db = new AppDbContext())
                {

                    if (selectedItem != null)
                    {

                        string serviceName = selectedItem.ServiceName;

                        var service = db.Services
                            .Where(s => s.ServiceName == serviceName)
                            .FirstOrDefault();

                        db.MembershipServices.Add(new MembershipService
                        {
                            ServiceId = service.ServiceId,
                            ClientMembershipId = clientMemberShipId,
                            ProvisionDate = provisionDate,
                        });
                        db.SaveChanges();

                        if (!isServiceFree)
                        {
                            db.ServicesPayments.Add(new ServicesPayments
                            {
                                ServiceId = service.ServiceId,
                                PaymentDate = null,
                                Price = (double)service.Price,
                                ClientId = AuthorizationWin.currentUser.Client.ClientId
                            });
                            db.SaveChanges();
                        }
                        else
                        {
                            db.ServicesPayments.Add(new ServicesPayments
                            {
                                ServiceId = service.ServiceId,
                                PaymentDate = provisionDate,
                                Price = 0,
                                ClientId = AuthorizationWin.currentUser.Client.ClientId
                            });
                            db.SaveChanges();
                        }



                        MessageBox.Show("Вы успешно оформили услугу");
                        LoadClientsServices(servicesHistoryComboBox.SelectedIndex);
                        LoadAvailableServices();
                        LoadAvailableServicesByMemberShip();
                    }
                }
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException?.Message ?? "Нет дополнительной информации.";
                MessageBox.Show("Ошибка при оформлении услуги:\n" + ex.Message + "\n\n" + inner);
            }

        }

        private void orderService_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = availableServicesList.SelectedItem;
            if (pickServiceProvisionDate.SelectedDate == null || pickServiceProvisionDate.SelectedDate < todayUtc)
            {
                MessageBox.Show("Выберите дату предоставления услуги(возможно вы указываете дату в прошлом).");
                return;
            }
            else
            {
                var provisionDate = pickServiceProvisionDate.SelectedDate.HasValue
                            ? DateTime.SpecifyKind(pickServiceProvisionDate.SelectedDate.Value, DateTimeKind.Utc)
                            : throw new Exception("Дата не выбрана");
                if (selectedItem == null)
                {
                    MessageBox.Show("Выберите услугу.");
                    return;
                }
                else
                {
                    OrderService(selectedItem, provisionDate, false);
                }
            }
        }

        private void servicesHistoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadClientsServices(servicesHistoryComboBox.SelectedIndex);
        }

        private void useServiceButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var selectedItem = button?.DataContext as FreeServiceViewModel;
            if (pickServiceDateForMembership.SelectedDate == null || pickServiceDateForMembership.SelectedDate < todayUtc)
            {
                MessageBox.Show("Выберите дату предоставления услуги(возможно вы указываете дату в прошлом).");
                return;
            }
            else
            {
                var provisionDate = pickServiceDateForMembership.SelectedDate.HasValue
                            ? DateTime.SpecifyKind(pickServiceDateForMembership.SelectedDate.Value, DateTimeKind.Utc)
                            : throw new Exception("Дата не выбрана");
                if (selectedItem == null)
                {
                    MessageBox.Show("Выберите услугу.");
                    return;
                }
                else
                {
                    OrderService(selectedItem, provisionDate, true);
                }
            }
        }

        private void lockerZoneComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadLockers();
        }

        private void rentLockerBtn_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedLocker = availableLockersList.SelectedItem as Locker;
            if (selectedLocker != null)
            {
                if (rentEndDp.SelectedDate == null || rentEndDp.SelectedDate < todayUtc)
                {
                    MessageBox.Show("Выберите дату окончания аренды(возможно вы указываете дату в прошлом).");
                    return;
                }
                else
                {
                    RentLocker(selectedLocker);
                }
            }
            else
            {
                MessageBox.Show("Выберите шкафчик для аренды.");
            }
        }


        private void rentEndDp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic selectedLocker = availableLockersList.SelectedItem as Locker;
            if (selectedLocker != null)
                lockerPriceForPerion.Text = $"Цена аренды: {CalculateLockerPrice(selectedLocker)}";
        }

        private void availableLockersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic selectedLocker = availableLockersList.SelectedItem as Locker;
            if (selectedLocker != null)
                lockerPriceForPerion.Text = $"Цена аренды: {CalculateLockerPrice(selectedLocker)}";
        }

        private void rentLockerUntilMembershipEnds_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                rentEndDp.SelectedDate = db.ClientMemberships
                    .Where(cm => cm.ClientMembershipId == clientMemberShipId)
                    .Select(cm => cm.EndDate)
                    .FirstOrDefault();
            }
        }

        private void changeRentedLockerBtn_Click(object sender, RoutedEventArgs e)
        {
            userHasRentedLocker.Visibility = Visibility.Collapsed;
            rentLockerSP.Visibility = Visibility.Visible;
            lockerPriceForPerion.Visibility = Visibility.Collapsed;
            rentEndDp.IsEnabled = false;
            rentLockerBtn.Content = "Изменить ящик";
            rentLockerUntilMembershipEnds.IsEnabled = false;
            LoadLockers();
        }

        private void CancelServiceClassContext_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.CommandParameter is int serviceId)
            {
                try
                {
                    using (var db = new AppDbContext())
                    {
                        var clientId = AuthorizationWin.currentUser.Client.ClientId;

                        var membershipService = db.MembershipServices
                            .FirstOrDefault(ms => ms.ServiceId == serviceId && ms.ClientMembershipId == clientMemberShipId);

                        if (membershipService == null)
                        {
                            MessageBox.Show("Услуга не найдена.");
                            return;
                        }

                        if (membershipService.ProvisionDate <= todayUtc)
                        {
                            MessageBox.Show("Нельзя отменить услугу, которая уже использована.");
                            return;
                        }

                        var payment = db.ServicesPayments
                            .Include(sp => sp.Service)
                            .FirstOrDefault(sp => sp.ServiceId == serviceId && sp.ClientId == clientId);


                        if (payment != null)
                        {
                            if (payment.PaymentDate != null)
                            {
                                // Возврат на баланс
                                var client = db.Clients.FirstOrDefault(c => c.ClientId == clientId);
                                if (client != null)
                                {
                                    client.Balance += (decimal)payment.Price;

                                    db.ClientTransactions.Add(new ClientTransaction
                                    {
                                        ClientId = client.ClientId,
                                        OperationDescription = $"Возврат за отмену услуги: {payment.Service.ServiceName}",
                                        PaymentWay = "На баланс",
                                        Amount = (decimal)payment.Price,
                                        TransactionType = "возврат"
                                    });
                                }
                            }

                            db.ServicesPayments.Remove(payment);
                        }

                        db.MembershipServices.Remove(membershipService);
                        db.SaveChanges();

                        LoadClientsServices(servicesHistoryComboBox.SelectedIndex);
                        LoadAvailableServices();
                        LoadAvailableServicesByMemberShip();

                        MessageBox.Show("Услуга успешно отменена.", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отмене услуги: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите услугу для отмены.", "Информация",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}

public class FreeServiceViewModel
{
    public int ServiceId { get; set; }
    public string ServiceType { get; set; }
    public string ServiceName { get; set; }
    public string Description { get; set; }
    public int UsageCount { get; set; }
    public int RemainingCount { get; set; }
}

