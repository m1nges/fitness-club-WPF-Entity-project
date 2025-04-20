using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fitness_club.Classes
{
    [Table("class_reviews")]
    public class ClassReviews
    {
        [Key]
        [Column("review_id")]
        public int ReviewId { get; set; }

        [ForeignKey("ClassInfo")]
        [Column("class_info_id")]
        public int Class_info_id { get; set; }
        public virtual ClassInfo ClassInfo { get; set; }

        [ForeignKey("Client")]
        [Column("author_id")]
        public int? ClientId { get; set; }
        public virtual Client? Client { get; set; }

        [Required]
        [Column("review_grade")]
        [Range(1, 5, ErrorMessage = "Оценка должна быть от 1 до 5.")]
        public int ReviewGrade { get; set; }

        [Column("review_content", TypeName = "text")]
        public string? ReviewContent { get; set; }
    }
}
