namespace Api.Dtos;

public class PostGetDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid AuthorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string TextContent { get; set; } = string.Empty;
    public bool HasImage { get; set; }
    public string? ImageContentType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
