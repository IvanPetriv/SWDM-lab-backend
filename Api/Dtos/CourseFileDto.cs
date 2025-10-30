namespace Api.Dtos;

public class CourseFileDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime UploadedAt { get; set; }
}
