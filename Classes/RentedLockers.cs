using fitness_club.Classes;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

[Table("rented_lockers")]
public class RentedLocker
{
    [Key]
    [Column("rent_locker_id")]
    public int RentLockerId { get; set; }

    [ForeignKey("Locker")]
    [Column("locker_id")]
    public int LockerId { get; set; }

    public virtual Locker Locker { get; set; }

    [ForeignKey("ClientMembership")]
    [Column("client_membership_id")]
    public int ClientMembershipId { get; set; }

    public virtual ClientMembership ClientMembership { get; set; }

    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Column("end_date")]
    public DateTime EndDate { get; set; }
    [Column("rent_price")]
    public int RentPrice { get; set; }
    [Column("payment_date")]
    public DateTime? PaymentDate { get; set; }
}
