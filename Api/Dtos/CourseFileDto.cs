namespace Api.Dtos;

public class CourseFileDto
{
    public Guid Id { get; set; }
    //public Guid CourseId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    //public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public byte[] FileContent { get; set; } = [];
    public DateTime UploadedAt { get; set; }
}
