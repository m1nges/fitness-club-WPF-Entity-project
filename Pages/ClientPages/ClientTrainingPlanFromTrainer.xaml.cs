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

namespace fitness_club.Pages.ClientPages
{
    public partial class ClientTrainingPlanFromTrainer : Page
    {
        public ClientTrainingPlanFromTrainer()
        {
            InitializeComponent();
            GetTrainingPlan();
        }

        public void GetTrainingPlan()
        {
            using(var db = new AppDbContext())
            {

                var trainingPlan = db.TrainingPlans
                    .Include(tp=>tp.Trainer)
                    .Where(tp => tp.ClientId == AuthorizationWin.currentUser.Client.ClientId)
                    .FirstOrDefault();


                if(trainingPlan != null)
                {
                    trainingPlanTbox.Text = trainingPlan.Plan;
                    trainerNameTb.Text = $"Ваш тренеровочный план от тренера {trainingPlan.Trainer.FirstName} {trainingPlan.Trainer.LastName}";
                }
                else
                {
                    trainingPlanTbox.Text = "У вас пока нет тренировочного плана от тренера.";
                }
            }
        }
    }
}
