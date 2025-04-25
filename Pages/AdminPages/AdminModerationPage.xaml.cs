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
    public partial class AdminModerationPage : Page
    {
        public AdminModerationPage()
        {
            InitializeComponent();
            LoadReviews();
        }

        private void LoadReviews()
        {
            using var db = new AppDbContext();

            var trainerReviews = db.TrainerReviews
                .Include(r => r.Trainer)
                .Include(r => r.Author)
                .Where(r => !r.Moderated)
                .ToList()
                .Select(r => new ModerationItem
                {
                    ReviewType = "trainer",
                    ReviewId = r.TrainerReviewId,
                    ObjectName = $"Тренер: {r.Trainer.LastName} {r.Trainer.FirstName}",
                    ReviewContent = r.ReviewContent,
                    Grade = r.ReviewGrade,
                    AuthorName = r.Author.IsAddAuthorName ? $"{r.Author.LastName} {r.Author.FirstName}" : "Аноним"
                });

            var classReviews = db.ClassReviews
                .Include(r => r.ClassInfo)
                .Include(r => r.Client)
                .Where(r => !r.Moderated)
                .ToList()
                .Select(r => new ModerationItem
                {
                    ReviewType = "class",
                    ReviewId = r.ReviewId,
                    ObjectName = $"Занятие: {r.ClassInfo.ClassName}",
                    ReviewContent = r.ReviewContent,
                    Grade = r.ReviewGrade,
                    AuthorName = r.Client.IsAddAuthorName ? $"{r.Client.LastName} {r.Client.FirstName}" : "Аноним"
                });

            var allReviews = trainerReviews.Concat(classReviews)
                .OrderBy(r => r.ObjectName)
                .ToList();

            ReviewsListView.ItemsSource = allReviews;
        }


        private void ApproveReview_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ModerationItem review)
            {
                using var db = new AppDbContext();

                if (review.ReviewType == "trainer")
                {
                    var r = db.TrainerReviews.Find(review.ReviewId);
                    if (r != null)
                        r.Moderated = true;
                }
                else if (review.ReviewType == "class")
                {
                    var r = db.ClassReviews.Find(review.ReviewId);
                    if (r != null)
                        r.Moderated = true;
                }

                db.SaveChanges();
                MessageBox.Show("Отзыв одобрен.");
                LoadReviews();
            }
        }

        private void RejectReview_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ModerationItem review)
            {
                using var db = new AppDbContext();

                if (review.ReviewType == "trainer")
                {
                    var r = db.TrainerReviews.Find(review.ReviewId);
                    if (r != null)
                        db.TrainerReviews.Remove(r);
                }
                else if (review.ReviewType == "class")
                {
                    var r = db.ClassReviews.Find(review.ReviewId);
                    if (r != null)
                        db.ClassReviews.Remove(r);
                }

                db.SaveChanges();
                MessageBox.Show("Отзыв отклонён и удалён.");
                LoadReviews();
            }
        }
    }

    public class ModerationItem
    {
        public string ReviewType { get; set; }
        public int ReviewId { get; set; }
        public string ObjectName { get; set; }
        public string ReviewContent { get; set; }
        public int Grade { get; set; }
        public string AuthorName { get; set; }
    }
}
