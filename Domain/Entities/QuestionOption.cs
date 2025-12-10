namespace Domain.Entities;

public class QuestionOption
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int OrderIndex { get; set; }

    // Navigation
    public virtual Question Question { get; set; } = null!;
}
