namespace Domain.Entities;
public class MediaMaterial {
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public byte[] FileContent { get; set; } = [];
    public DateTime CreatedAt { get; set; }

    // Navigations
    public virtual Course Course { get; set; } = null!;
}
