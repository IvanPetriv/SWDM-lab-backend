namespace Domain.Entities;
public class Student : User {
    // Navigations
    public ICollection<Enrollment> Enrollments { get; set; } = [];
    public virtual ICollection<TestSubmission> TestSubmissions { get; set; } = [];
}
