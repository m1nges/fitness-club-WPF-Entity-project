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

        private void AddEquipment_Click(object sender, RoutedEventArgs e)
        {
            string equipmentName = NewEquipmentNameTb.Text.Trim();
            if (!int.TryParse(NewEquipmentQtyTb.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество (>0)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(equipmentName))
            {
                MessageBox.Show("Введите название оборудования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using var db = new AppDbContext();

            // По умолчанию ставим состояние "Новое" или 1
            int defaultConditionId = db.EquipmentConditions
                .OrderBy(ec => ec.EquipmentConditionId)
                .Select(ec => ec.EquipmentConditionId)
                .FirstOrDefault();

            var newEquipment = new Equipment
            {
                EquipmentName = equipmentName,
                EquipmentConditionId = defaultConditionId,
                DeliveryDate = DateTime.UtcNow,
                LastMaintenanceDate = DateTime.UtcNow,
                Quantity = quantity
            };

            db.Equipment.Add(newEquipment);
            db.SaveChanges();

            MessageBox.Show("Оборудование успешно добавлено!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            // Обновляем таблицу
            LoadEquipment();

            // Очищаем поля
            NewEquipmentNameTb.Text = "";
            NewEquipmentQtyTb.Text = "";
        }

        private void DeleteEquipment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int equipmentId)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить это оборудование?\nОно также будет удалено из всех залов.",
                                             "Подтверждение удаления",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return;

                try
                {
                    using var db = new AppDbContext();

                    // Удаляем все связи в hall_equipment
                    var hallEquipments = db.HallEquipments.Where(he => he.EquipmentId == equipmentId).ToList();
                    db.HallEquipments.RemoveRange(hallEquipments);

                    // Удаляем оборудование
                    var equipment = db.Equipment.FirstOrDefault(e => e.EquipmentId == equipmentId);
                    if (equipment != null)
                    {
                        db.Equipment.Remove(equipment);
                        db.SaveChanges();
                        MessageBox.Show("Оборудование успешно удалено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadEquipment(); // Обновить таблицу
                    }
                    else
                    {
                        MessageBox.Show("Оборудование не найдено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении оборудования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
