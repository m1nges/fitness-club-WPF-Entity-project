using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    [Table("clientmembership")]
    public class ClientMembership
    {
        [Key]
        [Column("client_membership_id")]
        public int ClientMembershipId { get; set; }

        [ForeignKey("client")]
        [Column("client_id")]
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }

        [ForeignKey("membership")]
        [Column("membership_id")]
        public int MembershipId { get; set; }
        public virtual Membership Membership { get; set; }

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }
    }
}
