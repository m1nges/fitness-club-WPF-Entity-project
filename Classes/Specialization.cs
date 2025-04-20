using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    [Table("specialization")]
    public class Specialization
    {
        [Key]
        [Column("specialization_id")]
        public int SpecializationId { get; set; }

        [Required]
        [Column("specialization_name", TypeName = "varchar")]
        public string SpecializationName { get; set; }

        [Column("achievements", TypeName = "varchar")]
        public string Achievements { get; set; }

        [Column("education", TypeName = "varchar")]
        public string Education { get; set; }

        public virtual ICollection<Trainer> Trainers { get; set; }
    }
}
