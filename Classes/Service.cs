using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("services")]
public class Service
{
    [Key]
    [Column("service_id")]
    public int ServiceId { get; set; }

    [Required]
    [Column("service_name")]
    [MaxLength(100)]
    public string ServiceName { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("price")]
    public double? Price { get; set; }

    [Column("free_usage_limit")]
    public int FreeUsageLimit { get; set; } = 0;

    [ForeignKey("ServiceType")]
    [Column("service_type_id")]
    public int ServiceTypeId { get; set; }

    public virtual ServiceType ServiceType { get; set; }

    public virtual ICollection<MembershipService> MembershipServices { get; set; }
}
