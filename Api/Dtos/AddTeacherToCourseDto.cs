namespace Api.Dtos;

public class AddTeacherToCourseDto
{
    public Guid TeacherId { get; set; }
    public Guid CourseId { get; set; }
}
