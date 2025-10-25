namespace Domain.Entities;
public class Student {
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public virtual ICollection<Enrollment> Enrollments { get; set; } = [];
}
