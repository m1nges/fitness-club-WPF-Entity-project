using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("lockers")]
public class Locker
{
    [Key]
    [Column("locker_id")]
    public int LockerId { get; set; }

    [Column("locker_zone")]
    public string LockerZone { get; set; }

    [Column("month_price")]
    public float MonthPrice { get; set; }

    public virtual ICollection<RentedLocker> RentedLockers { get; set; }
}