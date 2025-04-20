using System;
using fitness_club.Model;
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
using Microsoft.EntityFrameworkCore;
using System.Collections;
using fitness_club.Classes;

namespace fitness_club.Pages.ClientPages
{
    /// <summary>
    /// Логика взаимодействия для ClientTrainerListPage.xaml
    /// </summary>
    public partial class ClientTrainerListPage : Page
    {
        string zaglushka = System.IO.Path.GetFullPath("../../../images/zagluska.png");
        public ClientTrainerListPage()
        {
            InitializeComponent();
            LoadTrainers();
        }

        public void LoadTrainers()
        {
            using (var db = new AppDbContext())
            {
                var trainersRaw = db.Trainers
                    .Include(t => t.TrainerReviews)
                    .Include(t => t.Specialization)
                    .ToList();

                var trainers = trainersRaw.Select(t => new
                {
                    t.TrainerId,
                    FullName = $"{t.FirstName} {t.LastName} {t.Patronymic}",
                    TrainerAge = $"{DateTime.Now.Year - t.BirthDate.Year}",
                    t.PhoneNumber,
                    Experience = CalculateExperience(t.DateOfEmployment),
                    AverageRating = CalculateAvgRating(t.TrainerId) != 0 ? $"{CalculateAvgRating(t.TrainerId)}" : "Нет оценок",
                    Photo = !string.IsNullOrWhiteSpace(t.Photo)
                        ? System.IO.Path.GetFullPath(t.Photo)
                        : zaglushka,
                    t.IndividualPrice,
                    Specialization = t.Specialization.SpecializationName
                }).ToList();

                TrainersItemsControl.ItemsSource = trainers;
            }
        }

        public double CalculateAvgRating(int trainerId)
        {
            using (var db = new AppDbContext())
            {
                var trainer = db.Trainers
                    .Include(t => t.TrainerReviews)
                    .FirstOrDefault(t => t.TrainerId == trainerId);
                if (trainer != null && trainer.TrainerReviews.Count > 0)
                {
                     double avgRating = trainer.TrainerReviews.Average(r => r.ReviewGrade);
                    return avgRating;
                }
                else
                {
                    return 0;
                }
            }
        }
        private string CalculateExperience(DateTime startDate)
        {
            DateTime today = DateTime.Now;
            int years = today.Year - startDate.Year;
            int months = today.Month - startDate.Month;
            int days = today.Day - startDate.Day;

            if (days < 0)
            {
                months--;
                days += DateTime.DaysInMonth(today.Year, today.Month);
            }

            if (months < 0)
            {
                years--;
                months += 12;
            }

            return $"{years} лет {months} месяцев {days} дней";
        }

        private void TrainerCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                dynamic selectedItem = border.DataContext;
                ClientTrainerDetailsWindow trainerDetailsWindow = new ClientTrainerDetailsWindow(selectedItem.TrainerId);
                trainerDetailsWindow.Show();
            }
        }
    }
}
