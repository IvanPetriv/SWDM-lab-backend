namespace Domain.Entities;
public class MediaMaterial {
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid CourseFile { get; set; }

    // Navigations
    public virtual Course Course { get; set; } = null!;
    public virtual CourseFile File { get; set; } = null!;
}
