using fitness_club.Classes;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("client_transaction")]
public class ClientTransaction
{
    [Key]
    [Column("transaction_id")]
    public int TransactionId { get; set; }

    [Required]
    [Column("client_id")]
    public int ClientId { get; set; }

    [ForeignKey("ClientId")]
    public virtual Client Client { get; set; }

    [Column("operation_description", TypeName = "text")]
    public string OperationDescription { get; set; }

    [Column("payment_way", TypeName = "varchar(50)")]
    public string PaymentWay { get; set; }

    [Column("amount", TypeName = "decimal(10,2)")]
    public decimal Amount { get; set; }

    [Column("transaction_type", TypeName = "varchar(20)")]
    public string TransactionType { get; set; } // "списание" или "возврат"

    [Column("transaction_date")]
    public DateTime TransactionDate { get; set; } = DateTime.Now;
}
