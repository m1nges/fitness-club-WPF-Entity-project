using fitness_club.Model;
using fitness_club.Classes;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace fitness_club.Pages.TrainerPages
{
    public partial class ClassVisitorsListPage : Page
    {
        private readonly int classId;

        // простой список для сохранения
        private List<VisitRow> visitors;

        public ClassVisitorsListPage(int classId)
        {
            InitializeComponent();
            this.classId = classId;
            LoadVisitors();
        }

        private void LoadVisitors()
        {
            using var db = new AppDbContext();

            classNameTb.Text = $"Участники занятия {db.Class
                .Where(c => c.ClassId == classId)
                .Select(c => c.ClassInfo.ClassName)
                .FirstOrDefault()}";

            visitors = db.ClassVisits
                .Include(cv => cv.ClientMembership)
                    .ThenInclude(cm => cm.Client)
                .Where(cv => cv.ClassId == classId)
                .Select(cv => new VisitRow
                {
                    ClientMembershipId = cv.ClientMembershipId,
                    FullName = cv.ClientMembership.Client.LastName + " " + cv.ClientMembership.Client.FirstName,
                    Visited = cv.Visited
                })
                .ToList();

            VisitorsDataGrid.ItemsSource = visitors;
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();

            foreach (var v in visitors)
            {
                var visit = db.ClassVisits
                    .FirstOrDefault(x => x.ClassId == classId && x.ClientMembershipId == v.ClientMembershipId);

                if (visit != null)
                {
                    visit.Visited = v.Visited;
                }
            }

            // Обновляем trainerChecked
            var training = db.Class.FirstOrDefault(c => c.ClassId == classId);
            if (training != null)
            {
                training.TrainerChecked = true;
            }

            db.SaveChanges();
            MessageBox.Show("Статусы посещения сохранены.");
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }

    public class VisitRow
    {
        public int ClientMembershipId { get; set; }
        public string FullName { get; set; }
        public bool Visited { get; set; }
    }

}
