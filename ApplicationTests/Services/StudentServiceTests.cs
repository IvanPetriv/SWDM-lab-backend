using Application.Services;
using Domain.Entities;

namespace ApplicationTests.Services;
public class StudentServiceTests : BaseServiceTests {
    private Student CreateStudent(Guid? id = null) {
        return new Student {
            Id = id ?? Guid.NewGuid(),
            Username = "student",
            FirstName = "Jane",
            LastName = "Doe",
            Email = "student@example.com",
            PasswordHash = "hashedpassword",
            RefreshTokens = new List<RefreshToken>()
        };
    }

    private Course CreateCourse(Guid? id = null) {
        return new Course {
            Id = id ?? Guid.NewGuid(),
            Name = "Math 101"
        };
    }

    [Fact]
    public async Task CreateStudent_AddsStudent() {
        var db = CreateDbContext();
        var service = new StudentService(db);
        var student = CreateStudent();

        var result = await service.CreateStudent(student, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(student.Id, result.Id);
        Assert.Single(db.Students);
    }

    [Fact]
    public async Task GetById_ReturnsStudent_WhenExists() {
        var db = CreateDbContext();
        var student = CreateStudent();
        db.Students.Add(student);
        await db.SaveChangesAsync();
        var service = new StudentService(db);

        var result = await service.GetById(student.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(student.Id, result!.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNull_WhenNotExists() {
        var db = CreateDbContext();
        var service = new StudentService(db);

        var result = await service.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAll_ReturnsAllStudents() {
        var db = CreateDbContext();
        db.Students.AddRange(CreateStudent(), CreateStudent());
        await db.SaveChangesAsync();
        var service = new StudentService(db);

        var result = await service.GetAll(CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Update_UpdatesStudent_WhenExists() {
        var db = CreateDbContext();
        var student = CreateStudent();
        db.Students.Add(student);
        await db.SaveChangesAsync();
        var service = new StudentService(db);

        var updatedStudent = new Student {
            Id = student.Id,
            Username = "updatedstudent",
            FirstName = "Alice",
            LastName = "Smith",
            Email = "updated@example.com",
            PasswordHash = "newhash",
            RefreshTokens = new List<RefreshToken>()
        };

        var result = await service.Update(student.Id, updatedStudent, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("updatedstudent", result!.Username);
        Assert.Equal("Alice", result.FirstName);
        Assert.Equal("updated@example.com", result.Email);
    }

    [Fact]
    public async Task Update_ReturnsNull_WhenNotExists() {
        var db = CreateDbContext();
        var service = new StudentService(db);

        var result = await service.Update(Guid.NewGuid(), CreateStudent(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_RemovesStudent_WhenExists() {
        var db = CreateDbContext();
        var student = CreateStudent();
        db.Students.Add(student);
        await db.SaveChangesAsync();
        var service = new StudentService(db);

        var result = await service.Delete(student.Id, CancellationToken.None);

        Assert.True(result);
        Assert.Empty(db.Students);
    }

    [Fact]
    public async Task Delete_ReturnsFalse_WhenNotExists() {
        var db = CreateDbContext();
        var service = new StudentService(db);

        var result = await service.Delete(Guid.NewGuid(), CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task GetStudentsInCourse_ReturnsCorrectStudents() {
        var db = CreateDbContext();
        var course = CreateCourse();
        db.Courses.Add(course);

        var student1 = CreateStudent();
        var student2 = CreateStudent();
        db.Students.AddRange(student1, student2);

        db.Enrollments.AddRange(
            new Enrollment { CourseId = course.Id, StudentId = student1.Id },
            new Enrollment { CourseId = course.Id, StudentId = student2.Id }
        );
        await db.SaveChangesAsync();

        var service = new StudentService(db);

        var studentsInCourse = await service.GetStudentsInCourse(course.Id, CancellationToken.None);

        Assert.Equal(2, studentsInCourse.Count);
        Assert.Contains(studentsInCourse, s => s.Id == student1.Id);
        Assert.Contains(studentsInCourse, s => s.Id == student2.Id);
    }
}
