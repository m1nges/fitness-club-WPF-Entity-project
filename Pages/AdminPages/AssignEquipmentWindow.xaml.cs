using fitness_club.Classes;
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
using System.Windows.Shapes;

namespace fitness_club.Pages.AdminPages
{
    /// <summary>
    /// Логика взаимодействия для AssignEquipmentWindow.xaml
    /// </summary>
    public partial class AssignEquipmentWindow : Window
    {
        private readonly int equipmentId;
        private int equipmentTotal = 0;
        private List<HallAssignmentRow> hallRows;

        public AssignEquipmentWindow(int equipmentId)
        {
            InitializeComponent();
            this.equipmentId = equipmentId;
            LoadData();
        }

        private void LoadData()
        {
            using var db = new AppDbContext();

            var equipment = db.Equipment.Find(equipmentId);
            if (equipment == null)
            {
                MessageBox.Show("Оборудование не найдено.");
                Close();
                return;
            }

            equipmentTotal = equipment.Quantity;

            var allHalls = db.Halls.ToList();
            var existingAssignments = db.HallEquipments
                .Where(he => he.EquipmentId == equipmentId)
                .ToDictionary(he => he.HallId, he => he.Quantity);

            hallRows = allHalls.Select(h => new HallAssignmentRow
            {
                HallId = h.HallId,
                HallName = h.HallName,
                Quantity = existingAssignments.ContainsKey(h.HallId) ? existingAssignments[h.HallId] : 0
            }).ToList();

            HallGrid.ItemsSource = hallRows;

            UpdateRemainingText();
        }

        private void UpdateRemainingText()
        {
            int used = hallRows.Sum(r => r.Quantity);
            int left = equipmentTotal - used;
            RemainingTextBlock.Text = $"Доступно к распределению: {left} шт.";
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int totalAssigned = hallRows.Sum(r => r.Quantity);
            if (totalAssigned > equipmentTotal)
            {
                MessageBox.Show($"Общее количество ({totalAssigned}) превышает доступное ({equipmentTotal}).");
                return;
            }

            using var db = new AppDbContext();

            foreach (var row in hallRows)
            {
                var existing = db.HallEquipments
                    .FirstOrDefault(he => he.HallId == row.HallId && he.EquipmentId == equipmentId);

                if (existing != null)
                {
                    existing.Quantity = row.Quantity;
                }
                else if (row.Quantity > 0)
                {
                    db.HallEquipments.Add(new HallEquipment
                    {
                        HallId = row.HallId,
                        EquipmentId = equipmentId,
                        Quantity = row.Quantity
                    });
                }
            }

            db.SaveChanges();
            MessageBox.Show("Распределение успешно сохранено!");
            LoadData();
            Close();
        }

        public class HallAssignmentRow
        {
            public int HallId { get; set; }
            public string HallName { get; set; }
            public int Quantity { get; set; }
        }
    }
}
