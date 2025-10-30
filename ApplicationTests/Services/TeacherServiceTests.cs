using Application.Services;
using Domain.Entities;

namespace ApplicationTests.Services;
public class TeacherServiceTests : BaseServiceTests {
    private Teacher CreateTeacher(Guid? id = null) {
        return new Teacher {
            Id = id ?? Guid.NewGuid(),
            Username = "teacher",
            FirstName = "John",
            LastName = "Doe",
            Email = "teacher@example.com",
            PasswordHash = "hash",
            RefreshTokens = new List<RefreshToken>()
        };
    }

    private Course CreateCourse(Guid? id = null, Guid? teacherId = null) {
        return new Course {
            Id = id ?? Guid.NewGuid(),
            Name = "Test Course",
            Description = "Desc",
            Code = 101,
            TeacherId = teacherId
        };
    }

    [Fact]
    public async Task Create_AddsTeacher() {
        var db = CreateDbContext();
        var service = new TeacherService(db);
        var teacher = CreateTeacher();

        var result = await service.Create(teacher, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(teacher.Id, result.Id);
        Assert.Single(db.Teachers);
    }

    [Fact]
    public async Task GetById_ReturnsTeacher_WhenExists() {
        var db = CreateDbContext();
        var teacher = CreateTeacher();
        db.Teachers.Add(teacher);
        await db.SaveChangesAsync();

        var service = new TeacherService(db);
        var result = await service.GetById(teacher.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(teacher.Id, result!.Id);
    }

    [Fact]
    public async Task GetAll_ReturnsAllTeachers() {
        var db = CreateDbContext();
        db.Teachers.AddRange(CreateTeacher(), CreateTeacher());
        await db.SaveChangesAsync();

        var service = new TeacherService(db);
        var teachers = await service.GetAll(CancellationToken.None);

        Assert.Equal(2, teachers.Count);
    }

    [Fact]
    public async Task Update_ChangesTeacherProperties() {
        var db = CreateDbContext();
        var teacher = CreateTeacher();
        db.Teachers.Add(teacher);
        await db.SaveChangesAsync();

        var service = new TeacherService(db);
        var updated = new Teacher {
            Id = teacher.Id,
            Username = "newusername",
            FirstName = "NewFirst",
            LastName = "NewLast",
            Email = "new@example.com"
        };

        var result = await service.Update(teacher.Id, updated, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("newusername", result!.Username);
        Assert.Equal("NewFirst", result.FirstName);
        Assert.Equal("NewLast", result.LastName);
        Assert.Equal("new@example.com", result.Email);
    }

    [Fact]
    public async Task Delete_RemovesTeacher() {
        var db = CreateDbContext();
        var teacher = CreateTeacher();
        db.Teachers.Add(teacher);
        await db.SaveChangesAsync();

        var service = new TeacherService(db);
        var result = await service.Delete(teacher.Id, CancellationToken.None);

        Assert.True(result);
        Assert.Empty(db.Teachers);
    }

    [Fact]
    public async Task GetCoursesOfTeacher_ReturnsAssignedCourses() {
        var db = CreateDbContext();
        var teacher = CreateTeacher();
        var course1 = CreateCourse(teacherId: teacher.Id);
        var course2 = CreateCourse();
        db.Teachers.Add(teacher);
        db.Courses.AddRange(course1, course2);
        await db.SaveChangesAsync();

        var service = new TeacherService(db);
        var courses = await service.GetCoursesOfTeacher(teacher.Id, CancellationToken.None);

        Assert.Single(courses);
        Assert.Equal(course1.Id, courses.First().Id);
    }

    [Fact]
    public async Task AssignCourse_AssignsTeacherToCourse() {
        var db = CreateDbContext();
        var teacher = CreateTeacher();
        var course = CreateCourse();
        db.Teachers.Add(teacher);
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var service = new TeacherService(db);
        var result = await service.AssignCourse(teacher.Id, course.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(teacher.Id, db.Courses.First().TeacherId);
    }

    [Fact]
    public async Task AssignCourse_ReturnsNull_WhenTeacherOrCourseNotFound() {
        var db = CreateDbContext();
        var service = new TeacherService(db);

        var result = await service.AssignCourse(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task UnassignCourse_RemovesTeacherFromCourse() {
        var db = CreateDbContext();
        var teacher = CreateTeacher();
        var course = CreateCourse(teacherId: teacher.Id);
        db.Teachers.Add(teacher);
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var service = new TeacherService(db);
        var result = await service.UnassignCourse(course.Id, CancellationToken.None);

        Assert.True(result);
        Assert.Null(db.Courses.First().TeacherId);
    }

    [Fact]
    public async Task UnassignCourse_ReturnsFalse_WhenCourseNotFound() {
        var db = CreateDbContext();
        var service = new TeacherService(db);

        var result = await service.UnassignCourse(Guid.NewGuid(), CancellationToken.None);
        Assert.False(result);
    }
}
