namespace Api.Dtos;

public class CourseUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Code { get; set; }
}
