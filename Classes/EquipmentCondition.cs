using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    [Table("equipmentcondition")]
    public class EquipmentCondition
    {
        [Key]
        [Column("equipment_condition_id")]
        public int EquipmentConditionId { get; set; }

        [Required]
        [Column("equipment_condition_description", TypeName = "varchar")]
        public string EquipmentConditionDescription { get; set; }

        public virtual ICollection<Equipment> Equipments { get; set; }
    }
}
