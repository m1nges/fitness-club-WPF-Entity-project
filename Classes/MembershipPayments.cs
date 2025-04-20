using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    [Table("membership_payments")]
    public class MembershipPayments
    {
        [Key]
        [Column("membership_payment_id")]
        public int MembershipPaymentId { get; set; }

        [Column("membership_id")]
        public int MembershipId { get; set; }
        public virtual Membership Membership { get; set; }
        [Column("payment_date")]
        public DateTime? PaymentDate { get; set; }
        [Column("price")]
        public double Price { get; set; }
        [Column("client_id")]
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
    }
}
