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
    public partial class AdminEquipmentManagementPage : Page
    {
        private List<EquipmentCondition> conditions;
        private List<EquipmentEditableRow> equipmentData;

        public AdminEquipmentManagementPage()
        {
            InitializeComponent();
            LoadConditions();
            LoadEquipment();
        }

        private void LoadConditions()
        {
            using var db = new AppDbContext();
            conditions = db.EquipmentConditions.ToList();
            ConditionColumn.ItemsSource = conditions;
        }

        private void LoadEquipment()
        {
            using var db = new AppDbContext();

            equipmentData = db.Equipment
                .Select(e => new EquipmentEditableRow
                {
                    EquipmentId = e.EquipmentId,
                    EquipmentName = e.EquipmentName,
                    EquipmentConditionId = e.EquipmentConditionId,
                    DeliveryDate = e.DeliveryDate.ToString("dd.MM.yyyy"),
                    LastMaintenanceDate = e.LastMaintenanceDate.ToString("dd.MM.yyyy"),
                    Quantity = e.Quantity
                }).ToList();

            EquipmentGrid.ItemsSource = equipmentData;
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();

            foreach (var row in equipmentData)
            {
                var entity = db.Equipment.FirstOrDefault(e => e.EquipmentId == row.EquipmentId);
                if (entity != null)
                {
                    entity.EquipmentConditionId = row.EquipmentConditionId;

                    if (DateTime.TryParseExact(row.LastMaintenanceDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    {
                        entity.LastMaintenanceDate = date;
                    }

                    entity.Quantity = row.Quantity;
                }
            }

            db.SaveChanges();
            MessageBox.Show("Изменения успешно сохранены!", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AssignEquipment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int id)
            {
                var win = new AssignEquipmentWindow(id);
                win.ShowDialog();
                LoadEquipment();
            }
        }
    }

    public class EquipmentEditableRow
    {
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public int EquipmentConditionId { get; set; }
        public string DeliveryDate { get; set; }
        public string LastMaintenanceDate { get; set; }
        public int Quantity { get; set; }
    }
}
