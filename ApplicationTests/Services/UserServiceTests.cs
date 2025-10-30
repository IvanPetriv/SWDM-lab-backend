using Application.Services;
using Application.Services.Users;
using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ApplicationTests.Services;
public class UserServiceTests : BaseServiceTests {
    private Student CreateStudent(Guid? id = null, string username = "student") {
        return new Student {
            Id = id ?? Guid.NewGuid(),
            Username = username,
            FirstName = "Jane",
            LastName = "Doe",
            Email = "student@example.com",
            PasswordHash = "hash",
            RefreshTokens = new List<RefreshToken>()
        };
    }

    private Teacher CreateTeacher(Guid? id = null, string username = "teacher") {
        return new Teacher {
            Id = id ?? Guid.NewGuid(),
            Username = username,
            FirstName = "John",
            LastName = "Smith",
            Email = "teacher@example.com",
            PasswordHash = "hash",
            RefreshTokens = new List<RefreshToken>()
        };
    }

    private Administrator CreateAdmin(Guid? id = null, string username = "admin") {
        return new Administrator {
            Id = id ?? Guid.NewGuid(),
            Username = username,
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@example.com",
            PasswordHash = "hash",
            RefreshTokens = new List<RefreshToken>()
        };
    }

    [Fact]
    public async Task Create_AddsDifferentUserTypes() {
        var db = CreateDbContext();
        var service = new UserService(db);

        var student = CreateStudent();
        var teacher = CreateTeacher();
        var admin = CreateAdmin();

        await service.Create(student, CancellationToken.None);
        await service.Create(teacher, CancellationToken.None);
        await service.Create(admin, CancellationToken.None);

        Assert.Single(db.Students);
        Assert.Single(db.Teachers);
        Assert.Single(db.Administrators);
    }

    [Fact]
    public async Task GetById_ReturnsCorrectUser() {
        var db = CreateDbContext();
        var service = new UserService(db);
        var student = CreateStudent();
        db.Students.Add(student);
        await db.SaveChangesAsync();

        var result = await service.GetById(student.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<Student>(result);
        Assert.Equal(student.Id, result.Id);
    }

    [Fact]
    public async Task GetAll_ReturnsAllUsers() {
        var db = CreateDbContext();
        db.Students.Add(CreateStudent());
        db.Teachers.Add(CreateTeacher());
        db.Administrators.Add(CreateAdmin());
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var users = await service.GetAll(CancellationToken.None);

        Assert.Equal(3, users.Count());
    }

    [Fact]
    public async Task Update_ChangesUserProperties() {
        var db = CreateDbContext();
        var student = CreateStudent();
        db.Students.Add(student);
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var updated = await service.Update(
            student.Id,
            "newuser",
            "NewFirst",
            "NewLast",
            "new@example.com",
            CancellationToken.None
        );

        Assert.NotNull(updated);
        Assert.Equal("newuser", updated!.Username);
        Assert.Equal("NewFirst", updated.FirstName);
        Assert.Equal("new@example.com", updated.Email);
    }

    [Fact]
    public async Task SearchByUsername_FindsAllMatchingUsers() {
        var db = CreateDbContext();
        db.Students.Add(CreateStudent(username: "alice"));
        db.Teachers.Add(CreateTeacher(username: "alice_teacher"));
        db.Administrators.Add(CreateAdmin(username: "admin_alice"));
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var results = await service.SearchByUsername("alice", CancellationToken.None);

        Assert.Equal(3, results.Count());
    }

    [Fact]
    public async Task SearchStudentsByUsername_FindsOnlyStudents() {
        var db = CreateDbContext();
        db.Students.Add(CreateStudent(username: "bob"));
        db.Teachers.Add(CreateTeacher(username: "bob_teacher"));
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var results = await service.SearchStudentsByUsername("bob", CancellationToken.None);

        Assert.Single(results);
        Assert.IsType<Student>(results.First());
    }

    [Fact]
    public async Task Delete_RemovesUser() {
        var db = CreateDbContext();
        var teacher = CreateTeacher();
        db.Teachers.Add(teacher);
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var deleted = await service.Delete(teacher.Id, CancellationToken.None);

        Assert.True(deleted);
        Assert.Empty(db.Teachers);
    }

    [Fact]
    public async Task Delete_ReturnsFalse_WhenUserNotFound() {
        var db = CreateDbContext();
        var service = new UserService(db);

        var deleted = await service.Delete(Guid.NewGuid(), CancellationToken.None);

        Assert.False(deleted);
    }
}
