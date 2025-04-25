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

namespace fitness_club.Pages.ClientPages
{
    public partial class ClassDetailsWindow : Window
    {
        public int classId = 0;
        string zaglushka = System.IO.Path.GetFullPath("../../../images/zagluska.png");
        private bool isEditMode = false;

        public ClassDetailsWindow(int classId, string parentForm)
        {
            InitializeComponent();
            this.classId = classId;
            CheckParentForm(parentForm);
            CheckIfIndividual();
            LoadClassReviews();
            LoadUserReview();
            LoadTrainerInfo();
            LoadClassInfo();
            LoadEquipmentData();
        }

        public void LoadClassReviews()
        {
            using (var db = new AppDbContext())
            {
                var classInfoId = db.Class
                    .Where(c => c.ClassId == classId)
                    .Select(c => c.ClassInfoId)
                    .FirstOrDefault();

                var reviews = db.ClassReviews
                    .Where(r => r.Class_info_id == classInfoId && r.Moderated) // только прошедшие модерацию
                    .Include(r => r.Client)
                    .Select(r => new
                    {
                        r.ReviewContent,
                        r.ReviewGrade,
                        ClientName = r.Client.IsAddAuthorName
                            ? r.Client.LastName + " " + r.Client.FirstName
                            : "Аноним"
                    })
                    .ToList();

                reviewsListView.ItemsSource = reviews;

                avgClassRating.Text = reviews.Count > 0
                    ? $"Средняя оценка: {reviews.Average(r => r.ReviewGrade):0.0}"
                    : "Пока нет оценок";
            }
        }


        public void CheckIfIndividual()
        {
            using (var db = new AppDbContext())
            {
                var classTypeId = db.Class
                    .Where(c => c.ClassId == classId)
                    .Select(c => c.ClassTypeId)
                    .FirstOrDefault();
                if (classTypeId == 2)
                {
                    sendReviewSP.Visibility = Visibility.Collapsed;
                    reviewsSP.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void LoadUserReview()
        {
            using (var db = new AppDbContext())
            {
                var classInfoId = db.Class
                    .Where(c => c.ClassId == classId)
                    .Select(c => c.ClassInfoId)
                    .FirstOrDefault();

                var clientId = AuthorizationWin.currentUser.Client.ClientId;

                var existingReview = db.ClassReviews
                    .FirstOrDefault(r => r.ClientId == clientId && r.Class_info_id == classInfoId);

                if (existingReview != null)
                {
                    isEditMode = true;
                    reviewsBlockTb.Text = "Ваш отзыв на это занятие. При измнение отзыв будет отправлен на модерацию!";
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
                var classInfoId = db.Class
                    .Where(c => c.ClassId == classId)
                    .Select(c => c.ClassInfoId)
                    .FirstOrDefault();

                var clientId = AuthorizationWin.currentUser.Client.ClientId;

                var existingReview = db.ClassReviews
                    .FirstOrDefault(r => r.ClientId == clientId && r.Class_info_id == classInfoId);

                if (existingReview != null)
                {
                    existingReview.ReviewContent = reviewTextBox.Text;
                    existingReview.ReviewGrade = currentRating;
                    existingReview.Moderated = false;
                    db.SaveChanges();
                }
            }
        }


        private void CheckIfReviewChanged()
        {
            using (var db = new AppDbContext())
            {
                var classInfoId = db.Class
                    .Where(c => c.ClassId == classId)
                    .Select(c => c.ClassInfoId)
                    .FirstOrDefault();

                var clientId = AuthorizationWin.currentUser.Client.ClientId;

                var existingReview = db.ClassReviews
                    .FirstOrDefault(r => r.ClientId == clientId && r.Class_info_id == classInfoId);

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



        public void CheckParentForm(string parentForm)
        {
            if (parentForm == "schedulePage")
            {
                sendReviewSP.Visibility = Visibility.Collapsed;
            }
        }

        public void LoadTrainerInfo()
        {
            DateTime today = DateTime.Now;
            using (var db = new AppDbContext())
            {
                var classTrainer = db.Class
                    .Include(c => c.WorkSchedule)
                        .ThenInclude(ws => ws.Trainer)
                            .ThenInclude(t => t.Specialization)
                    .FirstOrDefault(c => c.ClassId == classId);

                if (classTrainer == null || classTrainer.WorkSchedule == null || classTrainer.WorkSchedule.Trainer == null)
                    return;

                var trainer = classTrainer.WorkSchedule.Trainer;

                string trainerFullName = $"Ваш тренер: {trainer.LastName} {trainer.FirstName}";
                string trainerSpecialization = $"Специализация тренера: {trainer.Specialization.SpecializationName}";
                string trainerExperience = GetExperienceText(today, trainer.DateOfEmployment);

                fullNameOfTrainerTb.Text = trainerFullName;
                trainerSpecializationTb.Text = trainerSpecialization;
                trainerExperienceTb.Text = trainerExperience;

                string? trainerPicPath = trainer.Photo;
                string resolvedPath = !string.IsNullOrWhiteSpace(trainerPicPath)
                    ? System.IO.Path.GetFullPath(trainerPicPath)
                    : zaglushka;

                if (!File.Exists(resolvedPath))
                    resolvedPath = zaglushka;

                trainerPhoto.Source = new BitmapImage(new Uri(resolvedPath, UriKind.Absolute));
            }
        }

        private string GetExperienceText(DateTime today, DateTime start)
        {
            int years = today.Year - start.Year;
            int months = today.Month - start.Month;
            int days = today.Day - start.Day;

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

            return $"Стаж работы тренера: {years} лет {months} месяцев {days} дней";
        }

        public void LoadClassInfo()
        {
            using (var db = new AppDbContext())
            {
                var classInfo = db.Class
                    .Include(c => c.WorkSchedule)
                    .Include(c => c.Hall)
                    .Include(c => c.ClassInfo)
                    .FirstOrDefault(c => c.ClassId == classId);

                int visitorsNum = db.ClassVisits.Count(cv => cv.ClassId == classId);

                if (classInfo != null)
                {
                    classNameTb.Text = $"Название занятия: {classInfo.ClassInfo.ClassName}";
                    classDescriptionTb.Text = $"Описание занятия: {classInfo.ClassInfo.Description}";
                    classDateTimeTb.Text = $"Дата и время занятия: {classInfo.WorkSchedule.WorkDate:dd.MM.yyyy} {classInfo.StartTime} - {classInfo.EndTime}";

                    if (classInfo.ClassTypeId != 2) // не индивидуальные
                    {
                        classVisitorNumTb.Text = $"Количество посетителей: {visitorsNum}";
                        classFreeSpotsTb.Text = $"Количество свободных мест: {classInfo.PeopleQuantity - visitorsNum}";
                    }
                    else
                    {
                        classVisitorNumTb.Text = "";
                        classFreeSpotsTb.Text = "";
                    }
                }
            }
        }

        private void LoadEquipmentData()
        {
            try
            {
                var equipmentList = LoadHallEquipment(classId);
                EquipmentListView.ItemsSource = equipmentList;

                if (!equipmentList.Any())
                {
                    NoEquipmentTextBlock.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки оборудования: {ex.Message}");
            }
        }

        public List<EquipmentViewModel> LoadHallEquipment(int classId)
        {
            using (var db = new AppDbContext())
            {
                int hallId = db.Class
                    .Where(c => c.ClassId == classId)
                    .Select(c => c.HallId)
                    .FirstOrDefault();

                string? hallName = db.Halls
                    .Where(h => h.HallId == hallId)
                    .Select(h => h.HallName)
                    .FirstOrDefault();

                hallNameTb.Text = $"Зал: {hallName}";

                if (hallId == 0)
                    return new List<EquipmentViewModel>();

                return db.HallEquipments
                    .Where(he => he.HallId == hallId)
                    .Include(he => he.Equipment)
                        .ThenInclude(e => e.EquipmentCondition)
                    .Select(he => new EquipmentViewModel
                    {
                        EquipmentName = he.Equipment.EquipmentName,
                        EquipmentConditionDescription = he.Equipment.EquipmentCondition.EquipmentConditionDescription,
                        Quantity = he.Quantity
                    })
                    .ToList();
            }
        }

        private int currentRating = 0;
        private int hoverRating = 0;

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
                var classInfoId = db.Class
                    .Where(c => c.ClassId == classId)
                    .Select(c => c.ClassInfoId)
                    .FirstOrDefault();

                var classReview = new ClassReviews
                {
                    Class_info_id = classInfoId,
                    ReviewContent = reviewTextBox.Text,
                    ReviewGrade = currentRating,
                    ClientId = AuthorizationWin.currentUser.Client.ClientId,
                    Moderated = false,
                };
                db.ClassReviews.Add(classReview);
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
                    }
                    else
                    {
                        SendReview();
                        isEditMode = true;
                        MessageBox.Show("Отзыв успешно отправлен.");
                        submitReviewButton.Content = "Изменить отзыв";
                    }

                    submitReviewButton.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отправке отзыва: {ex.Message}");
                }
            }
            LoadClassReviews();
        }


        private void reviewTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            CheckIfReviewChanged();
        }
    }
}
