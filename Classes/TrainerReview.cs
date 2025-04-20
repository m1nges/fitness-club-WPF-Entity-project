using fitness_club.Classes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("trainer_reviews")]
public class TrainerReview
{
    [Key]
    [Column("trainer_review_id")]
    public int TrainerReviewId { get; set; }

    [Column("trainer_id")]
    public int TrainerId { get; set; }

    [ForeignKey("TrainerId")]
    public virtual Trainer Trainer { get; set; }

    [Column("author_id")]
    public int? AuthorId { get; set; }

    [ForeignKey("AuthorId")]
    public virtual Client? Author { get; set; }

    [Column("review_grade")]
    [Range(1, 5)]
    public int ReviewGrade { get; set; }

    [Column("review_content")]
    public string? ReviewContent { get; set; }
}
