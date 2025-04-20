using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
        [Table("membershiptype")]
        public class MembershipType
        {
            [Key]
            [Column("membership_type_id")]
            public int MembershipTypeId { get; set; }

            [Required]
            [Column("membership_type_name", TypeName = "varchar")]
            public string MembershipTypeName { get; set; }

            [Column("duration_months")]
            public int? DurationMonths { get; set; }

            public virtual ICollection<Membership> Memberships { get; set; }
        }
}
