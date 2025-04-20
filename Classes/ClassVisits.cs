using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    [Table("classvisits")]
    public class ClassVisits
    {
        [Key]
        [Column("visit_class_id")]
        public int ClassVisitId { get; set; }

        [ForeignKey("class")]
        [Column("class_id")]
        public int ClassId { get; set; }
        public virtual Class Class { get; set; }

        [Column("client_membership_id")]
        public int ClientMembershipId { get; set; }

        [ForeignKey("ClientMembershipId")]
        public virtual ClientMembership ClientMembership { get; set; }
    }
}
