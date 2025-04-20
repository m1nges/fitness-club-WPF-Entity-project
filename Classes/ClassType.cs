using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    [Table("classtype")]
    public class ClassType
    {
        [Key]
        [Column("class_type_id")]
        public int ClassTypeId { get; set; }

        [Required]
        [Column("class_type_name", TypeName = "varchar")]
        public string ClassTypeName { get; set; }

        public virtual ICollection<Class> Classes { get; set; }
    }
}
