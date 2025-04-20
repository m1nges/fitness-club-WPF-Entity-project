using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("service_type")]
public class ServiceType
{
    [Key]
    [Column("service_type_id")]
    public int ServiceTypeId { get; set; }

    [Required]
    [Column("type_name")]
    [MaxLength(100)]
    public string TypeName { get; set; }

    public virtual ICollection<Service> Services { get; set; }
}
