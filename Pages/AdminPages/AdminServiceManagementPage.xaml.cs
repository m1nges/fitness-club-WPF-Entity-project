using fitness_club.Model;
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

namespace fitness_club.Pages.AdminPages
{
    /// <summary>
    /// Логика взаимодействия для AdminServiceManagementPage.xaml
    /// </summary>
    public partial class AdminServiceManagementPage : Page
    {
        private List<ServiceEditableRow> serviceRows;

        public AdminServiceManagementPage()
        {
            InitializeComponent();
            LoadServiceTypes();
            LoadServices();
        }

        private void LoadServiceTypes()
        {
            using var db = new AppDbContext();
            ServiceTypeComboBox.ItemsSource = db.ServiceTypes.ToList();
        }

        private void LoadServices()
        {
            using var db = new AppDbContext();

            serviceRows = db.Services
                .Select(s => new ServiceEditableRow
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName,
                    Description = s.Description,
                    Price = s.Price,
                    FreeUsageLimit = s.FreeUsageLimit,
                    ServiceTypeName = s.ServiceType.TypeName
                })
                .ToList();

            ServiceGrid.ItemsSource = serviceRows;
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();

            foreach (var row in serviceRows)
            {
                var service = db.Services.FirstOrDefault(s => s.ServiceId == row.ServiceId);
                if (service != null)
                {
                    service.Description = row.Description;
                    service.Price = row.Price;
                    service.FreeUsageLimit = row.FreeUsageLimit;
                }
            }

            db.SaveChanges();
            MessageBox.Show("Изменения сохранены.");
        }

        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(NewPriceBox.Text, out var price) || !int.TryParse(NewFreeLimitBox.Text, out var limit))
            {
                MessageBox.Show("Проверьте корректность цены и лимита.");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewServiceNameBox.Text) || ServiceTypeComboBox.SelectedValue == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            using var db = new AppDbContext();
            db.Services.Add(new Service
            {
                ServiceName = NewServiceNameBox.Text.Trim(),
                Description = NewServiceDescriptionBox.Text?.Trim(),
                Price = price,
                FreeUsageLimit = limit,
                ServiceTypeId = (int)ServiceTypeComboBox.SelectedValue
            });

            db.SaveChanges();
            LoadServices();
            MessageBox.Show("Услуга добавлена.");
        }

        private void AddServiceType_Click(object sender, RoutedEventArgs e)
        {
            var name = NewServiceTypeBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Введите название типа услуги.");
                return;
            }

            using var db = new AppDbContext();
            db.ServiceTypes.Add(new ServiceType
            {
                TypeName = name
            });

            db.SaveChanges();
            LoadServiceTypes();
            MessageBox.Show("Тип услуги добавлен.");
        }

        public class ServiceEditableRow
        {
            public int ServiceId { get; set; }
            public string ServiceName { get; set; }
            public string ServiceTypeName { get; set; }
            public string Description { get; set; }
            public double? Price { get; set; }
            public int FreeUsageLimit { get; set; }
        }
    }
}
