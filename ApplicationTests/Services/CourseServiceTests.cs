using Application.Services;
using Domain.Entities;
using FluentAssertions;

namespace ApplicationTests.Services;
public class CourseServiceTests : BaseServiceTests {
    private Course CreateCourse(Guid? id = null, Guid? teacherId = null) {
        return new Course {
            Id = id ?? Guid.NewGuid(),
            Name = "Test Course",
            Description = "Description",
            Code = 101,
            TeacherId = teacherId ?? Guid.NewGuid(),
            CourseFiles = new List<CourseFile>()
        };
    }

    private Student CreateStudent(Guid? id = null) {
        return new Student {
            Id = id ?? Guid.NewGuid(),
            Username = "student",
            FirstName = "Jane",
            LastName = "Doe",
            Email = "student@example.com",
            PasswordHash = "hash",
            RefreshTokens = new List<RefreshToken>()
        };
    }

    [Fact]
    public async Task Create_AddsCourse() {
        var db = CreateDbContext();
        var service = new CourseService(db);
        var course = CreateCourse();

        var result = await service.Create(course, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(course.Id, result.Id);
        Assert.Single(db.Courses);
    }

    [Fact]
    public async Task GetById_ReturnsCourse_WhenExists() {
        var db = CreateDbContext();
        var course = CreateCourse();
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var service = new CourseService(db);
        var result = await service.GetById(course.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(course.Id, result!.Id);
    }

    [Fact]
    public async Task GetByIdWithFiles_ReturnsCourseWithFiles() {
        var db = CreateDbContext();
        var course = CreateCourse();
        course.CourseFiles.Add(new CourseFile {
            Id = Guid.NewGuid(),
            FileName = "file.txt",
            FileType = "text/plain",
            FileSize = 123
        });
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var service = new CourseService(db);
        var result = await service.GetByIdWithFiles(course.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result!.CourseFiles);
        Assert.Equal("file.txt", result.CourseFiles.First().FileName);
    }

    [Fact]
    public async Task GetAll_ReturnsAllCourses() {
        var db = CreateDbContext();
        db.Courses.AddRange(CreateCourse(), CreateCourse());
        await db.SaveChangesAsync();

        var service = new CourseService(db);
        var courses = await service.GetAll(CancellationToken.None);

        Assert.Equal(2, courses.Count);
    }

    [Fact]
    public async Task GetAllOfTeacher_ReturnsTeacherCourses() {
        var db = CreateDbContext();
        var teacherId = Guid.NewGuid();
        db.Courses.AddRange(CreateCourse(teacherId: teacherId), CreateCourse());
        await db.SaveChangesAsync();

        var service = new CourseService(db);
        var courses = await service.GetAllOfTeacher(teacherId, CancellationToken.None);

        Assert.Single(courses);
        Assert.Equal(teacherId, courses.First().TeacherId);
    }

    [Fact]
    public async Task GetAllOfStudent_ReturnsEnrolledCourses() {
        var db = CreateDbContext();
        var student = CreateStudent();
        var course1 = CreateCourse();
        var course2 = CreateCourse();
        db.Students.Add(student);
        db.Courses.AddRange(course1, course2);
        db.Enrollments.Add(new Enrollment { StudentId = student.Id, CourseId = course1.Id });
        await db.SaveChangesAsync();

        var service = new CourseService(db);
        var courses = await service.GetAllOfStudent(student.Id, CancellationToken.None);

        Assert.Single(courses);
        Assert.Equal(course1.Id, courses.First().Id);
    }

    [Fact]
    public async Task Update_ChangesCourseProperties() {
        var db = CreateDbContext();
        var course = CreateCourse();
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var service = new CourseService(db);
        var updated = await service.Update(course.Id, "New Name", "New Desc", 999, CancellationToken.None);

        Assert.NotNull(updated);
        Assert.Equal("New Name", updated!.Name);
        Assert.Equal(999, updated.Code);
    }

    [Fact]
    public async Task Delete_RemovesCourse() {
        var db = CreateDbContext();
        var course = CreateCourse();
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var service = new CourseService(db);
        var result = await service.Delete(course.Id, CancellationToken.None);

        Assert.True(result);
        Assert.Empty(db.Courses);
    }

    [Fact]
    public async Task EnrollStudent_AddsEnrollment_WhenValid() {
        var db = CreateDbContext();
        var student = CreateStudent();
        var course = CreateCourse();
        db.Students.Add(student);
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var service = new CourseService(db);
        var enrollment = await service.EnrollStudent(student.Id, course.Id, CancellationToken.None);

        Assert.NotNull(enrollment);
        Assert.Equal(student.Id, enrollment!.StudentId);
        Assert.Equal(course.Id, enrollment.CourseId);
    }

    [Fact]
    public async Task EnrollStudent_ReturnsNull_WhenInvalid() {
        var db = CreateDbContext();
        var service = new CourseService(db);

        var enrollment = await service.EnrollStudent(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        Assert.Null(enrollment);
    }

    [Fact]
    public async Task UnenrollStudent_RemovesEnrollment() {
        var db = CreateDbContext();
        var student = CreateStudent();
        var course = CreateCourse();
        var enrollment = new Enrollment { StudentId = student.Id, CourseId = course.Id };
        db.Students.Add(student);
        db.Courses.Add(course);
        db.Enrollments.Add(enrollment);
        await db.SaveChangesAsync();

        var service = new CourseService(db);
        var result = await service.UnenrollStudent(student.Id, course.Id, CancellationToken.None);

        Assert.True(result);
        Assert.Empty(db.Enrollments);
    }

    [Fact]
    public async Task IsTeacherOwner_ReturnsTrue_WhenTeacherOwnsCourse() {
        var db = CreateDbContext();
        var teacherId = Guid.NewGuid();
        var course = CreateCourse(teacherId: teacherId);
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var service = new CourseService(db);
        var isOwner = await service.IsTeacherOwner(course.Id, teacherId, CancellationToken.None);

        Assert.True(isOwner);
    }

    [Fact]
    public async Task IsTeacherOwner_ReturnsFalse_WhenTeacherDoesNotOwnCourse() {
        var db = CreateDbContext();
        var course = CreateCourse(teacherId: Guid.NewGuid());
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var service = new CourseService(db);
        var isOwner = await service.IsTeacherOwner(course.Id, Guid.NewGuid(), CancellationToken.None);

        Assert.False(isOwner);
    }
}
