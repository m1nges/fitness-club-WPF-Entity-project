using System.ComponentModel.DataAnnotations.Schema;

namespace fitness_club.Classes
{
    [Table("hallequipment")]
    public class HallEquipment
    {
        [Column("hall_id")]
        public int HallId { get; set; }
        public virtual Hall Hall { get; set; }

        [Column("equipment_id")]
        public int EquipmentId { get; set; }
        public virtual Equipment Equipment { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }
    }
}
