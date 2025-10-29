namespace Domain.Entities;
public class Teacher : User {
    // Navigations
    public ICollection<Enrollment> Enrollments { get; set; } = [];
}
