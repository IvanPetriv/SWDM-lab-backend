namespace Domain.Entities;
public class Course {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public virtual ICollection<Enrollment> Enrollments { get; set; } = [];
}
