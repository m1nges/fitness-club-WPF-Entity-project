using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fitness_club.Classes
{
    [Table("training_plan")]
    public class TrainingPlan
    {
        [Key]
        [Column("training_plan_id")]
        public int TrainingPlanId { get; set; }

        [Column("client_id")]
        public int ClientId { get; set; }

        [Column("trainer_id")]
        public int TrainerId { get; set; }

        [Column("plan", TypeName = "text")]
        public string Plan { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Client Client { get; set; }
        public virtual Trainer Trainer { get; set; }
    }
}
