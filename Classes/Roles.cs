using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    [Table("roles")]
    public class Roles
    {
        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }

        [Required]
        [Column("role_name", TypeName = "varchar")]
        public string RoleName { get; set; }

        public virtual ICollection<Users> Users { get; set; }
    }
}
