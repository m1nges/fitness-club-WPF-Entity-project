using fitness_club.Model;
using fitness_club.Windows;
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

namespace fitness_club.Pages.TrainerPages
{
    /// <summary>
    /// Логика взаимодействия для TrainerReviewsPage.xaml
    /// </summary>
    public partial class TrainerReviewsPage : Page
    {
        private int trainerId;
        public TrainerReviewsPage()
        {
            InitializeComponent();
            trainerId = AuthorizationWin.currentUser.Trainer.TrainerId;
            LoadTrainerReviews();
            CalculateAvgRating();
        }

        private void LoadTrainerReviews()
        {
            using (var db = new AppDbContext())
            {
                var reviews = db.TrainerReviews
                    .Where(tr => tr.TrainerId == trainerId)
                    .Include(r => r.Author)
                    .Select(r => new
                    {
                        r.ReviewContent,
                        r.ReviewGrade,
                        ClientName = r.Author != null && r.Author.IsAddAuthorName
                            ? $"{r.Author.LastName} {r.Author.FirstName}"
                            : "Аноним"
                    })
                    .ToList();

                reviewsListView.ItemsSource = reviews;
            }
        }

        private void CalculateAvgRating()
        {
            using (var db = new AppDbContext())
            {
                var trainer = db.Trainers
                    .Include(t => t.TrainerReviews)
                    .FirstOrDefault(t => t.TrainerId == trainerId);

                double avgRating = 0;

                if (trainer != null && trainer.TrainerReviews.Any())
                {
                    avgRating = trainer.TrainerReviews.Average(r => r.ReviewGrade);
                }

                avgTrainerRating.Text = $"Ваша средняя оценка: {avgRating:F1} / 5";
            }
        }

    }
}
