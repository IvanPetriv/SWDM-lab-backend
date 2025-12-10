namespace Domain.Entities;

public class Question
{
    public Guid Id { get; set; }
    public Guid TestId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int OrderIndex { get; set; }

    // Navigations
    public virtual Test Test { get; set; } = null!;
    public virtual ICollection<QuestionOption> Options { get; set; } = [];
}
