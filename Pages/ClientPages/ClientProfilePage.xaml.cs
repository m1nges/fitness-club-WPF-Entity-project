using fitness_club.Classes;
using fitness_club.Model;
using fitness_club.Windows;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для ClientProfilePage.xaml
    /// </summary>
    public partial class ClientProfilePage : Page
    {
        public ClientProfilePage()
        {
            InitializeComponent();
            LoadClientData();
        }

        private void LoadClientData()
        {
            var user = AuthorizationWin.currentUser;

            using (var db = new AppDbContext())
            {

                var fullUser = db.Users
                    .Include(u => u.Role)
                    .Include(u => u.Client)
                        .ThenInclude(c => c.Gender)
                    .FirstOrDefault(u => u.UserId == user.UserId);

                if (fullUser == null || fullUser.Client == null)
                {
                    MessageBox.Show("Пользователь или клиент не найден.");
                    return;
                }

                // Левая колонка
                LastNameTb.Text = fullUser.Client.LastName;
                FirstNameTb.Text = fullUser.Client.FirstName;
                PatronymicTb.Text = fullUser.Client.Patronymic;
                GenderTb.Text = fullUser.Client.Gender?.GenderName ?? "Не указано";
                BirthDateTb.Text = fullUser.Client.BirthDate.ToShortDateString();
                KemVidanTb.Text = fullUser.Client.PassportKemVidan;
                KogdaVidanTb.Text = fullUser.Client.PassportKogdaVidan.ToShortDateString();
                addAuthorNameCb.IsChecked = fullUser.Client.IsAddAuthorName;

                // Правая колонка
                PassportSeriesTb.Text = fullUser.Client.PassportSeries;
                PassportNumberTb.Text = fullUser.Client.PassportNumber;
                PhoneTb.Text = fullUser.Client.PhoneNumber.Trim();
                EmailTb.Text = fullUser.Client.Email;
                LoginTb.Text = fullUser.Login;
                RoleTb.Text = fullUser.Role?.RoleName ?? "Не указана";
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users
                    .Include(u => u.Client)
                    .FirstOrDefault(u => u.UserId == AuthorizationWin.currentUser.UserId);

                if (user == null || user.Client == null)
                {
                    MessageBox.Show("Пользователь или клиент не найден.");
                    return;
                }

                // Телефон
                string phone = PhoneTb.Text.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                if (!(Regex.IsMatch(phone, @"^\+7\d{10}$") || Regex.IsMatch(phone, @"^8\d{10}$")))
                {
                    MessageBox.Show("Телефон должен быть в формате +7XXXXXXXXXX или 8XXXXXXXXXX");
                    return;
                }

                // Email
                string email = EmailTb.Text.ToLower();
                if (!Regex.IsMatch(email, @"^[a-z0-9]+@[a-z0-9]+\.[a-z]+$"))
                {
                    MessageBox.Show("Введите корректный email.");
                    return;
                }

                // Логин
                string login = LoginTb.Text.Trim();
                if (login.Length < 4)
                {
                    MessageBox.Show("Логин должен быть не короче 4 символов.");
                    return;
                }

                // Пароль
                string newPassword = NewPasswordPb.Password.Trim();
                string repeatPassword = RepeatPasswordPb.Password.Trim();

                if (!string.IsNullOrEmpty(newPassword))
                {
                    if (newPassword != repeatPassword)
                    {
                        MessageBox.Show("Пароли не совпадают.");
                        return;
                    }

                    var uf = new UserFromDb();
                    if (!uf.CheckPassword(newPassword)) return;

                    user.Password = Verification.GetSHA512Hash(newPassword);
                }

                // Обновляем
                user.Login = login;
                user.Client.PhoneNumber = phone;
                user.Client.Email = email;
                user.Client.IsAddAuthorName = addAuthorNameCb.IsChecked ?? false;

                db.SaveChanges();
                MessageBox.Show("Изменения успешно сохранены!");
            }
        }
    }
}
