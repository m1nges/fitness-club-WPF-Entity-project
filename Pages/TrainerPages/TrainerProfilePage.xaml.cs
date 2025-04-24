using fitness_club.Classes;
using fitness_club.Model;
using fitness_club.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace fitness_club.Pages.TrainerPages
{
    /// <summary>
    /// Логика взаимодействия для TrainerProfilePage.xaml
    /// </summary>
    public partial class TrainerProfilePage : Page
    {
        private string projectImageFolderPath;
        private string relativeImagePath;
        public TrainerProfilePage()
        {
            InitializeComponent();
            projectImageFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\images");
            LoadTrainerData();
        }

        private void LoadTrainerData()
        {
            var user = AuthorizationWin.currentUser;

            using (var db = new AppDbContext())
            {
                var fullUser = db.Users
                    .Include(u => u.Role)
                    .Include(u => u.Trainer)
                        .ThenInclude(t => t.Gender)
                    .Include(u => u.Trainer.Specialization)
                    .FirstOrDefault(u => u.UserId == user.UserId);

                if (fullUser?.Trainer == null)
                {
                    MessageBox.Show("Тренер не найден.");
                    return;
                }

                var trainer = fullUser.Trainer;

                // Левая колонка
                dateOfEmployment.Text = trainer.DateOfEmployment.ToShortDateString();
                individualPrice.Text = trainer.IndividualPrice?.ToString();

                // Средняя колонка
                LastNameTb.Text = trainer.LastName;
                FirstNameTb.Text = trainer.FirstName;
                PatronymicTb.Text = trainer.Patronymic;
                GenderTb.Text = trainer.Gender?.GenderName ?? "Не указано";
                BirthDateTb.Text = trainer.BirthDate.ToShortDateString();
                KemVidanTb.Text = trainer.PassportKemVidan;
                KogdaVidanTb.Text = trainer.PassportKogdaVidan.ToShortDateString();
                SpecializationTb.Text = trainer.Specialization?.SpecializationName ?? "Не указана";

                // Правая колонка
                PassportSeriesTb.Text = trainer.PassportSeries;
                PassportNumberTb.Text = trainer.PassportNumber;
                PhoneTb.Text = trainer.PhoneNumber;
                EmailTb.Text = trainer.Email;
                LoginTb.Text = fullUser.Login;
                RoleTb.Text = fullUser.Role?.RoleName ?? "Не указана";

                // Аватар
                if (!string.IsNullOrEmpty(trainer.Photo))
                {
                    string fullImagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, trainer.Photo);
                    if (File.Exists(fullImagePath))
                    {
                        AvatarPreview.Source = new BitmapImage(new Uri(fullImagePath));
                    }
                }
            }
        }


        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                string fileName = System.IO.Path.GetFileName(selectedFilePath);

                string destFilePath = System.IO.Path.Combine(projectImageFolderPath, fileName);

                // Копируем изображение в папку проекта (images)
                File.Copy(selectedFilePath, destFilePath, true);

                // Отображаем превью
                AvatarPreview.Source = new BitmapImage(new Uri(destFilePath));

                // Формируем относительный путь для БД
                relativeImagePath = System.IO.Path.Combine("../../../images", fileName).Replace("\\", "/");
            }
        }

        //private void SaveAvatar_Click(object sender, RoutedEventArgs e)
        //{
        //    if (string.IsNullOrEmpty(relativeImagePath))
        //    {
        //        MessageBox.Show("Сначала выберите изображение.");
        //        return;
        //    }

        //    using (var db = new AppDbContext())
        //    {
        //        var trainerId = AuthorizationWin.currentUser.Trainer.TrainerId;

        //        // 1. Получаем тренера из базы
        //        var trainer = db.Trainers.FirstOrDefault(t => t.TrainerId == trainerId);

        //        if (trainer != null)
        //        {
        //            // 2. Обновляем поле
        //            trainer.Photo = relativeImagePath;

        //            // 3. Сохраняем изменения
        //            db.SaveChanges();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Тренер не найден.");
        //        }
        //    }


        //    MessageBox.Show("Аватар сохранён!\nПуть: " + relativeImagePath);
        //}

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users
                    .Include(u => u.Trainer)
                    .FirstOrDefault(u => u.UserId == AuthorizationWin.currentUser.UserId);

                if (user?.Trainer == null)
                {
                    MessageBox.Show("Пользователь или тренер не найден.");
                    return;
                }

                var trainer = user.Trainer;

                // Проверки телефона, email, логина
                string phone = PhoneTb.Text.Trim();
                string email = EmailTb.Text.Trim().ToLower();
                string login = LoginTb.Text.Trim();

                if (!Regex.IsMatch(phone, @"^\+7\d{10}$") && !Regex.IsMatch(phone, @"^8\d{10}$"))
                {
                    MessageBox.Show("Телефон должен быть в формате +7XXXXXXXXXX или 8XXXXXXXXXX");
                    return;
                }

                if (!Regex.IsMatch(email, @"^[a-z0-9]+@[a-z0-9]+\.[a-z]+$"))
                {
                    MessageBox.Show("Введите корректный email.");
                    return;
                }

                if (login.Length < 4)
                {
                    MessageBox.Show("Логин должен быть не короче 4 символов.");
                    return;
                }

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

                // Обновляем поля
                user.Login = login;
                trainer.PhoneNumber = phone;
                trainer.Email = email;

                if (double.TryParse(individualPrice.Text.Trim(), out double price))
                {
                    trainer.IndividualPrice = price;
                }

                // Сохраняем аватар, если был выбран
                if (!string.IsNullOrEmpty(relativeImagePath))
                {
                    trainer.Photo = relativeImagePath;
                }

                db.SaveChanges();
                MessageBox.Show("Изменения успешно сохранены!");
            }
        }

    }
}
