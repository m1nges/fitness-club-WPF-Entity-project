using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    [Table("gender")]
    public class Gender
    {
        [Key]
        [Column("gender_id", TypeName = "char(2)")]
        public string GenderId { get; set; }

        [Required]
        [Column("gender_name", TypeName = "varchar")]
        public string GenderName { get; set; }
    }
}
