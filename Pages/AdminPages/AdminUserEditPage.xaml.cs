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
using System.Windows.Shapes;

namespace fitness_club.Pages.AdminPages
{
    public partial class AdminUserEditPage : Page
    {
        private string selectedType = "Клиенты"; // по умолчанию
        private Client selectedClient;
        private Trainer selectedTrainer;

        public AdminUserEditPage()
        {
            InitializeComponent();
            LoadSpecializations();
            UserTypeComboBox.SelectedIndex = 0;
        }

        private void LoadSpecializations()
        {
            using var db = new AppDbContext();
            SpecializationComboBox.ItemsSource = db.Specializations
                .Select(s => new { s.SpecializationId, s.SpecializationName })
                .ToList();

            SpecializationComboBox.DisplayMemberPath = "SpecializationName";
            SpecializationComboBox.SelectedValuePath = "SpecializationId";
        }

        private void UserTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedType = ((ComboBoxItem)UserTypeComboBox.SelectedItem).Content.ToString();
            LoadUsers();
        }

        private void LoadUsers()
        {
            using var db = new AppDbContext();

            if (selectedType == "Клиенты")
            {
                var clients = db.Clients
                    .Include(c => c.User)
                    .Select(c => new
                    {
                        Entity = c,
                        FullName = c.LastName + " " + c.FirstName,
                        Phone = c.PhoneNumber
                    }).ToList();

                UserListView.ItemsSource = clients;
                SpecializationPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                var trainers = db.Trainers
                    .Include(t => t.User)
                    .Include(t => t.Specialization)
                    .Select(t => new
                    {
                        Entity = t,
                        FullName = t.LastName + " " + t.FirstName,
                        Phone = t.PhoneNumber
                    }).ToList();

                UserListView.ItemsSource = trainers;
                SpecializationPanel.Visibility = Visibility.Visible;
            }

            ClearFields();
        }

        private void UserListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearFields();

            dynamic item = UserListView.SelectedItem;

            if (item != null)
            {
                if (selectedType == "Клиенты")
                {
                    selectedClient = item.Entity;
                    FirstNameBox.Text = selectedClient.FirstName;
                    LastNameBox.Text = selectedClient.LastName;
                    PatronymicBox.Text = selectedClient.Patronymic;
                    PassportSeriesBox.Text = selectedClient.PassportSeries;
                    PassportNumberBox.Text = selectedClient.PassportNumber;
                    PassportKemBox.Text = selectedClient.PassportKemVidan;
                    PassportDateBox.SelectedDate = selectedClient.PassportKogdaVidan;
                    PhoneBox.Text = selectedClient.PhoneNumber;
                }
                else
                {
                    selectedTrainer = item.Entity;
                    FirstNameBox.Text = selectedTrainer.FirstName;
                    LastNameBox.Text = selectedTrainer.LastName;
                    PatronymicBox.Text = selectedTrainer.Patronymic;
                    PassportSeriesBox.Text = selectedTrainer.PassportSeries;
                    PassportNumberBox.Text = selectedTrainer.PassportNumber;
                    PassportKemBox.Text = selectedTrainer.PassportKemVidan;
                    PassportDateBox.SelectedDate = selectedTrainer.PassportKogdaVidan;
                    PhoneBox.Text = selectedTrainer.PhoneNumber;

                    if (selectedTrainer.SpecializationId.HasValue)
                        SpecializationComboBox.SelectedValue = selectedTrainer.SpecializationId;
                }
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (selectedType == "Клиенты" && selectedClient != null)
            {
                using var db = new AppDbContext();
                var client = db.Clients.Find(selectedClient.ClientId);

                client.FirstName = FirstNameBox.Text;
                client.LastName = LastNameBox.Text;
                client.Patronymic = PatronymicBox.Text;
                client.PassportSeries = PassportSeriesBox.Text;
                client.PassportNumber = PassportNumberBox.Text;
                client.PassportKemVidan = PassportKemBox.Text;
                client.PassportKogdaVidan = PassportDateBox.SelectedDate ?? DateTime.Now;
                client.PhoneNumber = PhoneBox.Text;

                db.SaveChanges();
                MessageBox.Show("Клиент обновлён.");
            }
            else if (selectedType == "Тренеры" && selectedTrainer != null)
            {
                using var db = new AppDbContext();
                var trainer = db.Trainers.Find(selectedTrainer.TrainerId);

                trainer.FirstName = FirstNameBox.Text;
                trainer.LastName = LastNameBox.Text;
                trainer.Patronymic = PatronymicBox.Text;
                trainer.PassportSeries = PassportSeriesBox.Text;
                trainer.PassportNumber = PassportNumberBox.Text;
                trainer.PassportKemVidan = PassportKemBox.Text;
                trainer.PassportKogdaVidan = PassportDateBox.SelectedDate ?? DateTime.Now;
                trainer.PhoneNumber = PhoneBox.Text;
                trainer.SpecializationId = SpecializationComboBox.SelectedValue as int?;

                db.SaveChanges();
                MessageBox.Show("Тренер обновлён.");
            }

            LoadUsers(); // обновить список
        }

        private void ClearFields()
        {
            selectedClient = null;
            selectedTrainer = null;

            FirstNameBox.Text = LastNameBox.Text = PatronymicBox.Text = "";
            PassportSeriesBox.Text = PassportNumberBox.Text = "";
            PassportKemBox.Text = "";
            PassportDateBox.SelectedDate = null;
            PhoneBox.Text = "";
            SpecializationComboBox.SelectedIndex = -1;
        }
    }
}
