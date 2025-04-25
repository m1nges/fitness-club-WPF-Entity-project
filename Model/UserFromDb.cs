using System;
using System.Linq;
using System.Windows;
using fitness_club.Classes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace fitness_club.Model
{
    public class UserFromDb
    {
        public bool CheckPassword(string password)
        {
            if (password.Length < 6)
            {
                MessageBox.Show("Длина пароля не может быть короче 6 символов");
                return false;
            }

            bool hasDigit = false, hasUpper = false, hasSpecial = false;
            string specialChars = "!@#$^";

            foreach (char c in password)
            {
                if (char.IsDigit(c)) hasDigit = true;
                if (char.IsUpper(c)) hasUpper = true;
                if (specialChars.Contains(c)) hasSpecial = true;
            }

            if (!hasDigit || !hasUpper)
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну цифру и одну заглавную букву!");
                return false;
            }

            if (!hasSpecial)
            {
                MessageBox.Show("Пароль должен содержать один из символов !@#$^");
                return false;
            }

            return true;
        }

        public void AddTrainerAndUpdateUser(
                string login,
                string password,
                int roleId,
                string lastName,
                string firstName,
                string patronymic,
                DateTime birthDate,
                string phoneNumber,
                string email,
                string genderId,
                string passportSeries,
                string passportNumber,
                string passportKemVidan,
                DateTime passportKogdaVidan)
        {
            using (var db = new AppDbContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // 1. Создаём пользователя
                        var user = new Users
                        {
                            Login = login,
                            Password = Verification.GetSHA512Hash(password),
                            RoleId = roleId
                        };
                        db.Users.Add(user);
                        db.SaveChanges(); // чтобы получить user.UserId

                        // 2. Создаём тренера
                        var trainer = new Trainer
                        {
                            LastName = lastName,
                            FirstName = firstName,
                            Patronymic = patronymic,
                            BirthDate = birthDate,
                            PhoneNumber = phoneNumber,
                            Email = email,
                            GenderId = genderId,
                            PassportSeries = passportSeries,
                            PassportNumber = passportNumber,
                            PassportKemVidan = passportKemVidan,
                            PassportKogdaVidan = passportKogdaVidan,
                            DateOfEmployment = DateTime.Today,
                            SpecializationId = 1, // или выбрать из UI
                            UserId = user.UserId
                        };
                        db.Trainers.Add(trainer);
                        db.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        string inner = ex.InnerException?.Message ?? "Нет деталей";
                        throw new Exception($"Ошибка при добавлении тренера: {ex.Message}\nINNER: {inner}");
                    }
                }
            }
        }



        public bool CheckUser(string login)
        {
            using (var db = new AppDbContext())
            {
                try
                {
                    bool userExists = db.Users.Any(u => u.Login == login);
                    if (userExists)
                    {
                        MessageBox.Show("Такой логин уже есть");
                        return false;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при проверке логина: {ex.Message}");
                    return false;
                }
            }
        }

        public Users GetUser(string login, string password)
        {
            using (var db = new AppDbContext())
            {
                try
                {

                    var user = db.Users
                        .FirstOrDefault(u => u.Login == login);

                    if (user == null)
                    {
                        MessageBox.Show("Пользователь с таким логином не найден.");
                        return null;
                    }

                    // Проверка пароля
                    if (!Verification.VerifySHA512Hash(password, user.Password))
                    {
                        MessageBox.Show("Неверный пароль.");
                        return null;
                    }

                    if (user.RoleId == 1)
                    {
                        var trainer = db.Users
                            .Include(u => u.Trainer)
                            .FirstOrDefault(u => u.Login == login);
                        return trainer;
                    }

                    else if (user.RoleId == 2)
                    {
                        var client = db.Users
                            .Include(u => u.Client)
                            .FirstOrDefault(u => u.Login == login);
                        return client;
                    }
                    else
                    {
                        var admin = db.Users
                            .FirstOrDefault(u => u.Login == login);
                        return admin;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении пользователя: {ex.Message}");
                    return null;
                }
            }
        }


        public void AddClientAndUpdateUser(
            string login,
            string password,
            int roleId,
            string lastName,
            string firstName,
            string patronymic,
            DateTime birthDate,
            string phoneNumber,
            string email,
            string genderId,
            string passportSeries,
            string passportNumber,
            string passportKemVidan,
            DateTime passportKogdaVidan)
        {
            using (var db = new AppDbContext())
            {
                try
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        // 1. Добавляем клиента
                        var client = new Client
                        {
                            LastName = lastName,
                            FirstName = firstName,
                            Patronymic = patronymic,
                            BirthDate = birthDate,
                            PhoneNumber = phoneNumber,
                            Email = email,
                            GenderId = genderId,
                            PassportSeries = passportSeries,
                            PassportNumber = passportNumber,
                            PassportKemVidan = passportKemVidan,
                            PassportKogdaVidan = passportKogdaVidan
                        };

                        db.Clients.Add(client);
                        db.SaveChanges();

                        // 2. Берем последнего пользователя (считаем что это он)
                        var user = db.Users
                            .OrderByDescending(u => u.UserId)
                            .FirstOrDefault();

                        if (user == null)
                            throw new Exception("Не найден ни один пользователь");

                        // 3. Обновляем его поля
                        user.Login = login;
                        user.Password = Verification.GetSHA512Hash(password);
                        user.RoleId = roleId;

                        db.SaveChanges();
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    var inner = ex.InnerException?.Message ?? "Нет деталей";
                    throw new Exception($"Ошибка при добавлении клиента: {ex.Message}\nINNER: {inner}");
                }
            }
        }
    }
}
