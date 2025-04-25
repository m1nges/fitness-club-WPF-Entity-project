using fitness_club.Classes;
using fitness_club.Model;
using System.Windows;


namespace fitness_club.Pages.TrainerPages
{
    public partial class WritePlanWindow : Window
    {
        private readonly int trainerId;
        private readonly int clientId;

        public WritePlanWindow(int trainerId, int clientId)
        {
            InitializeComponent();
            this.trainerId = trainerId;
            this.clientId = clientId;
            LoadExistingPlan();
        }

        private void LoadExistingPlan()
        {
            using var db = new AppDbContext();
            headerTb.Text = $"Введите индивидуальный план тренировок для {db.Clients.Where(c => c.ClientId == clientId).Select(c => c.FirstName + " " + c.LastName).FirstOrDefault()}";
            var existing = db.TrainingPlans
                .FirstOrDefault(p => p.ClientId == clientId && p.TrainerId == trainerId);

            if (existing != null)
            {
                PlanTextBox.Text = existing.Plan;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();

            var plan = db.TrainingPlans
                .FirstOrDefault(p => p.ClientId == clientId && p.TrainerId == trainerId);

            if (plan == null)
            {
                plan = new TrainingPlan
                {
                    ClientId = clientId,
                    TrainerId = trainerId,
                    Plan = PlanTextBox.Text
                };
                db.TrainingPlans.Add(plan);
            }
            else
            {
                plan.Plan = PlanTextBox.Text;
            }

            db.SaveChanges();
            MessageBox.Show("План сохранён успешно.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
