using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fitness_club.Classes
{
    [Table("class")]
    public class Class
    {
        [Key]
        [Column("class_id")]
        public int ClassId { get; set; }

        [ForeignKey("ClassInfo")]
        [Column("class_info_id")]
        public int ClassInfoId { get; set; }
        public virtual ClassInfo ClassInfo { get; set; }

        [ForeignKey("WorkSchedule")]
        [Column("work_schedule_id")]
        public int? WorkScheduleId { get; set; }
        public virtual WorkSchedule? WorkSchedule { get; set; }

        [Column("start_time")]
        public TimeSpan StartTime { get; set; }

        [Column("end_time")]
        public TimeSpan EndTime { get; set; }

        [ForeignKey("Hall")]
        [Column("hall_id")]
        public int HallId { get; set; }
        public virtual Hall Hall { get; set; }

        [ForeignKey("ClassType")]
        [Column("class_type_id")]
        public int ClassTypeId { get; set; }
        public virtual ClassType ClassType { get; set; }

        [Column("people_quantity")]
        public int PeopleQuantity { get; set; }

        [Column("price")]
        public double? Price { get; set; }

        [Column("trainer_checked")]
        public bool TrainerChecked { get; set; }

        public virtual ICollection<ClassVisits> ClassVisits { get; set; }
    }
}
