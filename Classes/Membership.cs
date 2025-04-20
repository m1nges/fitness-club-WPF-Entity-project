using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    [Table("membership")]
    public class Membership
    {
        [Key]
        [Column("membership_id")]
        public int MembershipId { get; set; }

        [Required]
        [Column("membership_name", TypeName = "varchar")]

        public string MembershipName { get; set; }
        [Column("membership_description", TypeName = "text")]
        public string? MembershipDescription { get; set; }

        [Column("price")]
        public int Price { get; set; }

        [ForeignKey("membershiptype")]
        [Column("membership_type_id")]
        public int MembershipTypeId { get; set; }
        public virtual MembershipType MembershipType { get; set; }

        public virtual ICollection<ClientMembership> ClientMemberships { get; set; }
    }
}
