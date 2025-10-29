namespace Domain.Entities;
public class Course {
    public Guid Id { get; set; }
    public Guid TeacherId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int Code { get; set; }

    // Navigations
    public virtual ICollection<Enrollment> Enrollments { get; set; } = [];
}
