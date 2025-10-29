namespace Api.Dtos;

public class CourseWithFilesDto
{
    public Guid Id { get; set; }
    public Guid? TeacherId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Code { get; set; }
    public IEnumerable<CourseFileDto> Files { get; set; } = [];
}
