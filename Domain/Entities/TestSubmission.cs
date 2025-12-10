namespace Domain.Entities;

public class TestSubmission
{
    public Guid Id { get; set; }
    public Guid TestId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public double Grade { get; set; }

    // Navigations
    public virtual Test Test { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
    public virtual ICollection<SubmissionAnswer> Answers { get; set; } = [];
}
