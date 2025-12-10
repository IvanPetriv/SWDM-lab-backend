namespace Domain.Entities;

public class Test
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigations
    public virtual Course Course { get; set; } = null!;
    public virtual ICollection<Question> Questions { get; set; } = [];
    public virtual ICollection<TestSubmission> Submissions { get; set; } = [];
}
