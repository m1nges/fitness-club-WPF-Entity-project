using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace fitness_club.Classes
{
    [Table("users")]
    public class Users
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Column("login", TypeName = "varchar")]
        public string Login { get; set; }

        [Column("password", TypeName = "varchar")]
        public string Password { get; set; }

        [Column("role_id")]
        public int? RoleId { get; set; }

        [ForeignKey("RoleId")]
        public virtual Roles Role { get; set; }

        public virtual Client Client { get; set; }
        public virtual Trainer Trainer { get; set; }
    }
}
