namespace Domain.Entities;

public class SubmissionAnswer
{
    public Guid Id { get; set; }
    public Guid SubmissionId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid SelectedOptionId { get; set; }

    // Navigations
    public virtual TestSubmission Submission { get; set; } = null!;
    public virtual Question Question { get; set; } = null!;
    public virtual QuestionOption SelectedOption { get; set; } = null!;
}
