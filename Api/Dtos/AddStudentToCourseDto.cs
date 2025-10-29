namespace Api.Dtos;

public class AddStudentToCourseDto
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
}
