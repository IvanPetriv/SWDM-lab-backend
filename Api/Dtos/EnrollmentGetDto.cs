namespace Api.Dtos;
public class EnrollmentGetDto {
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }

    public DateTime EnrolledAt { get; set; }
    public string Grade { get; set; }
}
