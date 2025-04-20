using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    [Table("equipment")]
    public class Equipment
    {
        [Key]
        [Column("equipment_id")]
        public int EquipmentId { get; set; }

        [Required]
        [Column("equipment_name", TypeName = "varchar")]
        public string EquipmentName { get; set; }

        [ForeignKey("EquipmentCondition")]
        [Column("equipment_condition_id")]
        public int EquipmentConditionId { get; set; }
        public virtual EquipmentCondition EquipmentCondition { get; set; }

        [Column("delivery_date", TypeName = "date")]
        public DateTime DeliveryDate { get; set; }

        [Column("last_maintenance_date", TypeName = "date")]
        public DateTime LastMaintenanceDate { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        public virtual ICollection<HallEquipment> HallEquipments { get; set; }
    }
}
