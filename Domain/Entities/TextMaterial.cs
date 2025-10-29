namespace Domain.Entities;
public class TextMaterial {
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Navigations
    public virtual Course Course { get; set; } = null!;
}
