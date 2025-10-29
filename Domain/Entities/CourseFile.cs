namespace Domain.Entities;
public class CourseFile {
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public byte[] FileContent { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}