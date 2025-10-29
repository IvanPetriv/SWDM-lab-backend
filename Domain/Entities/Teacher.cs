namespace Domain.Entities;
public class Teacher : User {
    // Navigations
    public ICollection<Course> Courses { get; set; } = [];
}
