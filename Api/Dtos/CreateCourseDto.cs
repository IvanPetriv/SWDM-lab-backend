namespace Api.Dtos;

public class CreateCourseDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Code { get; set; }
    public Guid? TeacherId { get; set; }
}
