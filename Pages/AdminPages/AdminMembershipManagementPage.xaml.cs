using fitness_club.Classes;
using fitness_club.Model;
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

namespace fitness_club.Pages.AdminPages
{
    /// <summary>
    /// Логика взаимодействия для AdminMembershipManagementPage.xaml
    /// </summary>
    public partial class AdminMembershipManagementPage : Page
    {
        private List<MembershipEditableRow> memberships;

        public AdminMembershipManagementPage()
        {
            InitializeComponent();
            LoadMembershipTypes();
            LoadMemberships();
        }

        private void LoadMembershipTypes()
        {
            using var db = new AppDbContext();
            MembershipTypeComboBox.ItemsSource = db.MembershipTypes.ToList();
        }

        private void LoadMemberships()
        {
            using var db = new AppDbContext();
            memberships = db.Memberships
                .Select(m => new MembershipEditableRow
                {
                    MembershipId = m.MembershipId,
                    MembershipName = m.MembershipName,
                    MembershipDescription = m.MembershipDescription,
                    Price = m.Price,
                    MembershipTypeName = m.MembershipType.MembershipTypeName,
                    DurationMonths = m.MembershipType.DurationMonths ?? 0
                })
                .ToList();

            MembershipGrid.ItemsSource = memberships;
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();

            foreach (var row in memberships)
            {
                var membership = db.Memberships
                    .Include(m => m.MembershipType)
                    .FirstOrDefault(m => m.MembershipId == row.MembershipId);

                if (membership != null)
                {
                    membership.MembershipDescription = row.MembershipDescription;
                    membership.Price = row.Price;

                    if (membership.MembershipType != null)
                    {
                        membership.MembershipType.DurationMonths = row.DurationMonths;
                    }
                }
            }

            db.SaveChanges();
            MessageBox.Show("Изменения сохранены!");
            LoadMemberships();
        }

        private void AddMembership_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewMembershipNameBox.Text) ||
                !int.TryParse(NewMembershipPriceBox.Text, out int price) ||
                MembershipTypeComboBox.SelectedValue == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
                return;
            }

            using var db = new AppDbContext();
            db.Memberships.Add(new Membership
            {
                MembershipName = NewMembershipNameBox.Text.Trim(),
                MembershipDescription = NewMembershipDescriptionBox.Text?.Trim(),
                Price = price,
                MembershipTypeId = (int)MembershipTypeComboBox.SelectedValue
            });

            db.SaveChanges();
            MessageBox.Show("Абонемент добавлен!");
            LoadMemberships();
        }

        private void AddMembershipType_Click(object sender, RoutedEventArgs e)
        {
            var name = NewMembershipTypeNameBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name) ||
                !int.TryParse(NewDurationBox.Text, out int duration))
            {
                MessageBox.Show("Введите корректные данные.");
                return;
            }

            using var db = new AppDbContext();
            db.MembershipTypes.Add(new MembershipType
            {
                MembershipTypeName = name,
                DurationMonths = duration
            });

            db.SaveChanges();
            MessageBox.Show("Тип абонемента добавлен!");
            LoadMembershipTypes();
        }

        public class MembershipEditableRow
        {
            public int MembershipId { get; set; }
            public string MembershipName { get; set; }
            public string MembershipTypeName { get; set; }
            public string MembershipDescription { get; set; }
            public int Price { get; set; }
            public int DurationMonths { get; set; }
        }
    }
}
