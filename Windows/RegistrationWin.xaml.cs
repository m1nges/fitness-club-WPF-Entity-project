using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
using fitness_club.Model;
using System.Configuration;
using System.Text.RegularExpressions;

namespace fitness_club.Windows
{
    public partial class RegistrationWin : Window
    {
        private static Random rnd = new Random();
        UserFromDb ufDb = new UserFromDb();
        protected string password;
        protected string login;

        public RegistrationWin()
        {
            InitializeComponent();
            secretCodeTrainerSp.Visibility = Visibility.Collapsed;
        }

        private void trainerOrClientCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (trainerOrClientCmb.SelectedIndex == 0)
                secretCodeTrainerSp.Visibility = Visibility.Visible;
            else
                secretCodeTrainerSp.Visibility = Visibility.Collapsed;
        }


        private void registrationBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(firstNameTb.Text) ||
                string.IsNullOrWhiteSpace(lastNameTb.Text) ||
                string.IsNullOrWhiteSpace(emailTb.Text) ||
                string.IsNullOrWhiteSpace(phoneTb.Text) ||
                string.IsNullOrWhiteSpace(passportSeriesTb.Text) ||
                string.IsNullOrWhiteSpace(passportNumberTb.Text) ||
                string.IsNullOrWhiteSpace(passportKemVidanTb.Text) ||
                genderCmb.SelectedIndex == -1 ||
                dateOfBirthDp.SelectedDate == null ||
                dateOfPassportVidanDp.SelectedDate == null)
            {
                MessageBox.Show("Необходимо заполнить все поля!");
                return;
            }

            string email = emailTb.Text.ToLower();
            Regex emailRegex = new Regex("^[a-z0-9]+@[a-z0-9]+\\.[a-z]+$");

            if (!emailRegex.IsMatch(email))
            {
                MessageBox.Show("Введите корректный email");
                return;
            }

            var birthDate = dateOfBirthDp.SelectedDate.Value;
            int age = DateTime.Now.Year - birthDate.Year;
            if (birthDate > DateTime.Now.AddYears(-age)) age--;

            if (age < 14)
            {
                MessageBox.Show("Возраст должен быть не менее 14 лет");
                return;
            }

            string phone = phoneTb.Text.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
            if (!(Regex.IsMatch(phone, @"^\+7\d{10}$") || Regex.IsMatch(phone, @"^8\d{10}$")))
            {
                MessageBox.Show("Введите корректный номер телефона");
                return;
            }

            if (!int.TryParse(passportSeriesTb.Text, out _) || passportSeriesTb.Text.Length != 4 ||
                !int.TryParse(passportNumberTb.Text, out _) || passportNumberTb.Text.Length != 6)
            {
                MessageBox.Show("Некорректные паспортные данные");
                return;
            }

            password = GeneratePassword();
            login = GeneratePassword();

            if (!ufDb.CheckPassword(password) || !ufDb.CheckUser(login))
                return;

            int roleId = trainerOrClientCmb.SelectedIndex == 0 ? 1 : 2;
            string genderId = genderCmb.SelectedIndex == 0 ? "Me" : "Fe";

            // Проверка тренера
            if (roleId == 1)
            {
                if (codeWordTb.Text.Trim() != "iamtruetrainer")
                {
                    MessageBox.Show("Неверное кодовое слово для регистрации тренера!");
                    return;
                }
                else
                {
                    try
                    {
                        ufDb.AddTrainerAndUpdateUser(
                            login, password, roleId,
                            lastNameTb.Text, firstNameTb.Text, patronymicTb.Text,
                            birthDate, phone, email, genderId,
                            passportSeriesTb.Text, passportNumberTb.Text,
                            passportKemVidanTb.Text, dateOfPassportVidanDp.SelectedDate ?? DateTime.Now
                        );
                        SendMail();
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        var inner = ex.InnerException?.Message ?? "Нет деталей";
                        throw new Exception($"Ошибка при добавлении клиента: {ex.Message}\nINNER: {inner}");
                    }
                }

            }
            else
            {
                ufDb.AddClientAndUpdateUser(
                    login, password, roleId,
                    lastNameTb.Text, firstNameTb.Text, patronymicTb.Text,
                    birthDate, phone, email, genderId,
                    passportSeriesTb.Text, passportNumberTb.Text,
                    passportKemVidanTb.Text, dateOfPassportVidanDp.SelectedDate ?? DateTime.Now
                );
                SendMail();
                this.Close();
            }
        }


        public static string GeneratePassword()
        {
            const string uppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^";
            const string allChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^";

            Random rndLength = new Random();
            char[] password = new char[rndLength.Next(8, 16)];

            password[0] = uppercaseLetters[rnd.Next(uppercaseLetters.Length)];
            password[1] = digits[rnd.Next(digits.Length)];
            password[2] = specialChars[rnd.Next(specialChars.Length)];

            for (int i = 3; i < password.Length; i++)
            {
                password[i] = allChars[rnd.Next(allChars.Length)];
            }

            ShuffleArray(password);

            return new string(password);
        }

        private static void ShuffleArray(char[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                int j = rnd.Next(array.Length);
                char temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }

        private void SendMail()
        {
            // отправитель - устанавливаем адрес и отображаемое в письме имя
            MailAddress from = new MailAddress("egorka201806@yandex.ru", "m1nges Fitness Club");
            // кому отправляем
            MailAddress to = new MailAddress($"{emailTb.Text}");
            // создаем объект сообщения
            MailMessage m = new MailMessage(from, to);
            // тема письма
            m.Subject = "Ваши данные для авторизации в m1nges Fitness Club";
            // текст письма
            m.Body = $"<h2>Ваш логин: {login}<br>Ваш пароль: {password}</h2>";
            // письмо представляет код html
            m.IsBodyHtml = true;
            // адрес smtp-сервера и порт, с которого будем отправлять письмо
            SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 587);
            // логин и пароль
            smtp.Credentials = new NetworkCredential("egorka201806@yandex.ru", "dcfdboifdvnikcod");
            smtp.EnableSsl = true;
            smtp.Send(m);
        }
    }
}
