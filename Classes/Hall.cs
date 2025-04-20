using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    [Table("hall")]
    public class Hall
    {
        [Key]
        [Column("hall_id")]
        public int HallId { get; set; }

        [Required]
        [Column("hall_name", TypeName = "varchar")]
        public string HallName { get; set; }

        [Column("capacity")]
        public int Capacity { get; set; }

        [Column("description", TypeName = "text")]
        public string Description { get; set; }

        [Column("area")]
        public double Area { get; set; }

        public virtual ICollection<Class> Classes { get; set; }
        public virtual ICollection<HallEquipment> HallEquipments { get; set; }
    }
}
