using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    [Table("services_payments")]
    public class ServicesPayments
    {
        [Key]
        [Column("service_payment_id")]
        public int ServicePaymentId { get; set; }
        [Column("service_id")]
        public int ServiceId { get; set; }
        public virtual Service Service { get; set; }
        [Column("payment_date")]
        public DateTime? PaymentDate { get; set; }
        [Column("price")]
        public double Price { get; set; }
        [Column("client_id")]
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
    }
}
