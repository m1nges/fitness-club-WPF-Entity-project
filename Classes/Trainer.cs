using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    [Table("trainer")]
    public class Trainer
    {
        [Key]
        [Column("trainer_id")]
        public int TrainerId { get; set; }

        [Column("user_id")]
        public int? UserId { get; set; }
        public virtual Users User { get; set; }

        [Required]
        [Column("first_name", TypeName = "varchar")]
        public string FirstName { get; set; }

        [Required]
        [Column("last_name", TypeName = "varchar")]
        public string LastName { get; set; }

        [Column("patronymic", TypeName = "varchar")]
        public string Patronymic { get; set; }

        [Required]
        [Column("birth_date", TypeName = "date")]
        public DateTime BirthDate { get; set; }

        [Required]
        [Column("gender_id", TypeName = "char(2)")]
        public string GenderId { get; set; }
        public virtual Gender Gender { get; set; }

        [Column("passport_series", TypeName = "char(4)")]
        public string PassportSeries { get; set; }

        [Column("passport_number", TypeName = "char(6)")]
        public string PassportNumber { get; set; }

        [Column("passport_kem_vidan", TypeName = "varchar")]
        public string PassportKemVidan { get; set; }

        [Column("passport_kogda_vidan", TypeName = "date")]
        public DateTime PassportKogdaVidan { get; set; }

        [Column("date_of_employment", TypeName = "date")]
        public DateTime DateOfEmployment { get; set; }

        [Column("phone_number", TypeName = "varchar")]
        public string PhoneNumber { get; set; }

        [Column("email", TypeName = "varchar")]
        public string Email { get; set; }

        [Column("photo", TypeName = "text")]
        public string? Photo { get; set; }

        [Column("individual_price")]
        public double? IndividualPrice { get; set; }

        [ForeignKey("Specialization")]
        [Column("specialization_id")]
        public int SpecializationId { get; set; }
        public virtual Specialization Specialization { get; set; }

        public virtual ICollection<WorkSchedule> WorkSchedules { get; set; }
        public virtual ICollection<TrainerReview> TrainerReviews { get; set; }
    }
}
