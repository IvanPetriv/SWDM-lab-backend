namespace Domain.Entities;

public class Post
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid AuthorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
    public byte[]? ImageData { get; set; }
    public string? ImageContentType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation
    public virtual Course Course { get; set; } = null!;
}
