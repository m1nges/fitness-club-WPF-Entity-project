using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fitness_club.Classes
{
    [Table("client")]
    public class Client
    {
        [Key]
        [Column("client_id")]
        public int ClientId { get; set; }

        [Required]
        [Column("last_name", TypeName = "varchar")]
        public string LastName { get; set; }

        [Required]
        [Column("first_name", TypeName = "varchar")]
        public string FirstName { get; set; }

        [Column("patronymic", TypeName = "varchar")]
        public string? Patronymic { get; set; }

        [Column("birth_date", TypeName = "date")]
        public DateTime BirthDate { get; set; }

        [Column("phone_number", TypeName = "bpchar")]
        public string PhoneNumber { get; set; }

        [Column("email", TypeName = "varchar")]
        public string Email { get; set; }

        [Column("gender_id", TypeName = "bpchar")]
        public string GenderId { get; set; }

        [Column("passport_series", TypeName = "bpchar")]
        public string PassportSeries { get; set; }

        [Column("passport_number", TypeName = "bpchar")]
        public string PassportNumber { get; set; }

        [Column("passport_kem_vidan", TypeName = "varchar")]
        public string PassportKemVidan { get; set; }

        [Column("passport_kogda_vidan", TypeName = "date")]
        public DateTime PassportKogdaVidan { get; set; }

        [Column("user_id")]
        [ForeignKey("User")]
        public int? UserId { get; set; }

        public Users User { get; set; }
        [ForeignKey("GenderId")]
        public virtual Gender Gender { get; set; }

        [Column("add_author_name")]
        public bool IsAddAuthorName { get; set; }

        [Column("balance")]
        public decimal Balance { get; set; }

    }
}
