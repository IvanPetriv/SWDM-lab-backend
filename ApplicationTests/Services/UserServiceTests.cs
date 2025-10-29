using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Services.Users;
using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ApplicationTests.Services;
public class UserServiceTests : BaseServiceTests {
    [Fact]
    public async Task GetByEmailAsync_ReturnsStudent_WhenStudentExists() {
        using var db = CreateDbContext();
        var student = new Student { Id = Guid.NewGuid(), Email = "student@example.com", Username = "student1" };
        db.Students.Add(student);
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var result = await service.GetByEmailAsync("student@example.com", CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<Student>(result);
        Assert.Equal(student.Id, result!.Id);
    }

    [Fact]
    public async Task GetByUsernameAsync_ReturnsTeacher_WhenTeacherExists() {
        using var db = CreateDbContext();
        var teacher = new Teacher { Id = Guid.NewGuid(), Email = "t@x.com", Username = "teacher1" };
        db.Teachers.Add(teacher);
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var result = await service.GetByUsernameAsync("teacher1", CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<Teacher>(result);
    }

    [Fact]
    public async Task GetAsync_ReturnsAdministrator_WhenAdminExists() {
        using var db = CreateDbContext();
        var admin = new Administrator { Id = Guid.NewGuid(), Email = "a@x.com", Username = "admin" };
        db.Administrators.Add(admin);
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var result = await service.GetAsync(admin.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.IsType<Administrator>(result);
    }

    [Fact]
    public async Task CreateAsync_AddsCorrectUserType() {
        using var db = CreateDbContext();
        var service = new UserService(db);
        var teacher = new Teacher { Id = Guid.NewGuid(), Email = "t2@x.com", Username = "teach2" };

        var created = await service.CreateAsync(teacher, CancellationToken.None);

        Assert.Equal(1, await db.Teachers.CountAsync());
        Assert.Same(teacher, created);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingUser() {
        using var db = CreateDbContext();
        var student = new Student { Id = Guid.NewGuid(), Email = "old@x.com", Username = "old" };
        db.Students.Add(student);
        await db.SaveChangesAsync();

        var updated = new Student { Id = student.Id, Email = "new@x.com", Username = "new" };
        var service = new UserService(db);

        var result = await service.UpdateAsync(student.Id, updated, CancellationToken.None);

        Assert.NotNull(result);
        var fromDb = await db.Students.FirstAsync();
        Assert.Equal("new@x.com", fromDb.Email);
    }

    [Fact]
    public async Task DeleteAsync_RemovesCorrectUserType() {
        using var db = CreateDbContext();
        var teacher = new Teacher { Id = Guid.NewGuid(), Email = "t@x.com", Username = "t" };
        db.Teachers.Add(teacher);
        await db.SaveChangesAsync();

        var service = new UserService(db);
        var deleted = await service.DeleteAsync(teacher.Id, CancellationToken.None);

        Assert.True(deleted);
        Assert.Empty(db.Teachers);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenUserNotFound() {
        using var db = CreateDbContext();
        var service = new UserService(db);

        var deleted = await service.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.False(deleted);
    }
}
