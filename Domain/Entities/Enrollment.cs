namespace Domain.Entities;
public class Enrollment {
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime EnrolledAt { get; set; }
    public int Grade { get; set; }

    public Student Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
