using fitness_club.Model;
using fitness_club.Windows;
using fitness_club.Classes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;

namespace fitness_club.Pages.ClientPages
{
    public partial class ClientTrainerDetailsWindow : Window
    {
        string zaglushka = System.IO.Path.GetFullPath("../../../images/zagluska.png");
        int trainerId;
        private bool isEditMode = false;
        private int currentRating = 0;
        private int hoverRating = 0;
        double avgRating;
        public ClientTrainerDetailsWindow(int trainerId)
        {
            InitializeComponent();
            this.trainerId = trainerId;
            GridLoaded();
        }

        public void GridLoaded()
        {
            LoadTrainerReviews();
            LoadUserReview();
            LoadTrainerInfo();
        }

        public void LoadTrainerInfo()
        {
            using (var db = new AppDbContext())
            {
                var trainerInfo = db.Trainers
                    .Include(t => t.TrainerReviews)
                    .Include(t => t.Specialization)
                    .FirstOrDefault(t => t.TrainerId == trainerId);
                if (trainerInfo == null)
                {
                    MessageBox.Show("Не удалось загрузить информацию о тренере");
                    return;
                }
                trainerFullNameTb.Text = $"{trainerInfo.FirstName} {trainerInfo.LastName} {trainerInfo.Patronymic}";
                string? trainerPicPath = trainerInfo.Photo;
                string resolvedPath = !string.IsNullOrWhiteSpace(trainerPicPath)
                    ? System.IO.Path.GetFullPath(trainerPicPath)
                    : zaglushka;

                if (!File.Exists(resolvedPath))
                    resolvedPath = zaglushka;

                trainerPhoto.Source = new BitmapImage(new Uri(resolvedPath, UriKind.Absolute));

                trainerSpecializationTb.Text = $"Специализация тренера: {trainerInfo.Specialization?.SpecializationName ?? "Не указана"}";
                trainerExperienceTb.Text = $"Стаж работы: {CalculateExperience(trainerInfo.DateOfEmployment)}";
                trainerAgeTb.Text = $"Возраст тренера: {DateTime.Now.Year - trainerInfo.BirthDate.Year} лет";
                individualPriceTb.Text = $"Цена персональной тренировки(час): {trainerInfo.IndividualPrice} руб.";
                CalculateAvgRating(trainerId);
                avgTrainerRating.Text = $"Средняя оценка: {(avgRating != 0 ? $"{avgRating}" : "Нет оценки")}";

                var hasUserClasses = db.ClassVisits
                    .Count(cv => cv.Class.WorkSchedule.TrainerId == trainerId && cv.ClientMembership.ClientId == AuthorizationWin.currentUser.Client.ClientId && cv.Class.ClassTypeId == 2);

                if (hasUserClasses > 0)
                {
                    trainerPhoneTb.Text = $"Телефон: {trainerInfo.PhoneNumber}";
                }
                else
                {
                    trainerPhoneTb.Text = "Телефон: Не доступен, тк у вас не было занятий с тренером";
                    reviewsBlockTb.Text = "Вы не можете оставить отзыв, так как у вас не было занятий с тренером!";
                    sendReviewSP.IsEnabled = false;

                }

            }
        }

        public void CalculateAvgRating(int trainerId)
        {
            using (var db = new AppDbContext())
            {
                var trainer = db.Trainers
                    .Include(t => t.TrainerReviews)
                    .FirstOrDefault(t => t.TrainerId == trainerId);
                if (trainer != null && trainer.TrainerReviews.Count > 0)
                {
                    avgRating = trainer.TrainerReviews.Average(r => r.ReviewGrade);
                }
                else
                {
                    avgRating = 0;
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


        public void LoadTrainerReviews()
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
                        ClientName = r.Author.IsAddAuthorName ? r.Author.LastName + " " + r.Author.FirstName : "Аноним",
                    })
                    .ToList();
                reviewsListView.ItemsSource = reviews;
            }
        }


        private void LoadUserReview()
        {
            using (var db = new AppDbContext())
            {
                var clientId = AuthorizationWin.currentUser.Client.ClientId;

                var existingReview = db.TrainerReviews
                    .FirstOrDefault(r => r.AuthorId == clientId && r.TrainerId == trainerId);

                if (existingReview != null)
                {
                    isEditMode = true;
                    reviewsBlockTb.Text = "Ваш отзыв на это занятие";
                    reviewTextBox.Text = existingReview.ReviewContent;
                    currentRating = existingReview.ReviewGrade;
                    HighlightStars(currentRating);
                    submitReviewButton.Content = "Изменить отзыв";
                    submitReviewButton.IsEnabled = false;
                }
            }
        }

        public void UpdateReview()
        {
            using (var db = new AppDbContext())
            {
                var clientId = AuthorizationWin.currentUser.Client.ClientId;

                var existingReview = db.TrainerReviews
                    .FirstOrDefault(r => r.TrainerId == trainerId && r.TrainerId == trainerId);

                if (existingReview != null)
                {
                    existingReview.ReviewContent = reviewTextBox.Text;
                    existingReview.ReviewGrade = currentRating;
                    db.SaveChanges();
                }
            }
        }


        private void CheckIfReviewChanged()
        {
            using (var db = new AppDbContext())
            {

                var clientId = AuthorizationWin.currentUser.Client.ClientId;

                var existingReview = db.TrainerReviews
                    .FirstOrDefault(tr => tr.AuthorId == clientId && tr.TrainerId == trainerId);

                if (existingReview != null)
                {
                    if (reviewTextBox.Text == existingReview.ReviewContent && currentRating == existingReview.ReviewGrade)
                        submitReviewButton.IsEnabled = false;
                    else
                        submitReviewButton.IsEnabled = true;
                }
                else
                {
                    submitReviewButton.IsEnabled = !string.IsNullOrWhiteSpace(reviewTextBox.Text) || currentRating > 0;
                }
            }
        }

        private void HighlightStars(int rating)
        {
            for (int i = 1; i <= 5; i++)
            {
                System.Windows.Shapes.Path star = (System.Windows.Shapes.Path)FindName($"Star{i}");
                if (star != null)
                {
                    if (i <= rating)
                    {
                        star.Fill = System.Windows.Media.Brushes.Gold;
                    }
                    else
                    {
                        star.Fill = System.Windows.Media.Brushes.Gray;
                    }
                }
            }
        }

        private void Star_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is System.Windows.Shapes.Path star && int.TryParse(star.Tag?.ToString(), out int rating))
            {
                hoverRating = rating;
                HighlightStars(rating);
            }
        }

        private void Star_MouseLeave(object sender, MouseEventArgs e)
        {
            HighlightStars(currentRating);
        }

        private void Star_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Shapes.Path star && int.TryParse(star.Tag?.ToString(), out int rating))
            {
                currentRating = rating;
                HighlightStars(currentRating);
                CheckIfReviewChanged();
            }
        }

        public void SendReview()
        {
            using (var db = new AppDbContext())
            {
                var trainerReview = new TrainerReview
                {
                    TrainerId = trainerId,
                    ReviewContent = reviewTextBox.Text,
                    ReviewGrade = currentRating,
                    AuthorId = AuthorizationWin.currentUser.Client.ClientId,
                };
                db.TrainerReviews.Add(trainerReview);
                db.SaveChanges();
            }
        }

        private void SubmitReviewButton_Click(object sender, RoutedEventArgs e)
        {
            if (reviewTextBox.Text.Length <= 10 || currentRating == 0)
            {
                MessageBox.Show("Отзыв должен содержать не менее 10 символов и должна быть выбрана оценка.");
            }
            else
            {
                try
                {
                    if (isEditMode)
                    {
                        UpdateReview();
                        MessageBox.Show("Отзыв успешно обновлён.");
                        CalculateAvgRating(trainerId);
                        avgTrainerRating.Text = $"Средняя оценка: {(avgRating != 0 ? $"{avgRating}" : "Нет оценки")}";
                    }
                    else
                    {
                        SendReview();
                        isEditMode = true;
                        MessageBox.Show("Отзыв успешно отправлен.");
                        CalculateAvgRating(trainerId);
                        avgTrainerRating.Text = $"Средняя оценка: {(avgRating != 0 ? $"{avgRating}" : "Нет оценки")}";
                        submitReviewButton.Content = "Изменить отзыв";
                    }

                    submitReviewButton.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отправке отзыва: {ex.Message}");
                }
            }
            LoadTrainerReviews();
        }


        private void reviewTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CheckIfReviewChanged();
        }

    }
}
