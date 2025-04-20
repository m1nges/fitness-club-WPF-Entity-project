using fitness_club.Classes;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("membership_services")]
public class MembershipService
{
    [Key]
    [Column("membershipservice_id")]
    public int MembershipServiceId { get; set; }

    [Column("client_membership_id")]
    public int ClientMembershipId { get; set; }

    [ForeignKey("ClientMembershipId")]
    public virtual ClientMembership ClientMembership { get; set; }

    [Column("service_id")]
    public int ServiceId { get; set; }

    [ForeignKey("ServiceId")]
    public virtual Service Service { get; set; }

    [Column("provision_date")]
    public DateTime? ProvisionDate { get; set; }
}
