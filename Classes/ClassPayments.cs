using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    [Table("class_payments")]
    public class ClassPayments
    {
        [Key]
        [Column("class_payment_id")]
        public int ClassPaymentId { get; set; }
        [Column("class_id")]
        public int ClassId { get; set; }
        public virtual Class Class { get; set; }
        [Column("payment_date")]
        public DateTime? PaymentDate { get; set; }
        [Column("price")]
        public double Price { get; set; }
        [Column("client_id")]
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
    }
}
