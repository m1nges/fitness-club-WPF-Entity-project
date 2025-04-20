using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    [Table("workschedule")]
    public class WorkSchedule
    {
        [Key]
        [Column("work_schedule_id")]
        public int WorkScheduleId { get; set; }

        [ForeignKey("Trainer")]
        [Column("trainer_id")]
        public int TrainerId { get; set; }
        public virtual Trainer Trainer { get; set; }

        [Column("work_date", TypeName = "date")]
        public DateTime WorkDate { get; set; }

        [Column("start_time")]
        public TimeSpan StartTime { get; set; }

        [Column("end_time")]
        public TimeSpan EndTime { get; set; }

        public virtual ICollection<Class> Classes { get; set; }
    }
}
