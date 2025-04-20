using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fitness_club.Classes
{
    [Table("class_info")]
    public class ClassInfo
    {
        [Key]
        [Column("class_info_id")]
        public int ClassInfoId { get; set; }

        [Required]
        [Column("class_name", TypeName = "varchar")]
        public string ClassName { get; set; }

        [Column("description", TypeName = "text")]
        public string? Description { get; set; }

        public virtual ICollection<Class> Classes { get; set; }
    }
}
