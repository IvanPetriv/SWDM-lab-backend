using Application.Services;
using Domain.Entities;

namespace ApplicationTests.Services;
public class AdministratorServiceTests : BaseServiceTests {
    private Administrator CreateAdmin(Guid? id = null) {
        return new Administrator {
            Id = id ?? Guid.NewGuid(),
            Username = "admin",
            FirstName = "John",
            LastName = "Doe",
            Email = "admin@example.com",
            PasswordHash = "hashedpassword",
            RefreshTokens = new List<RefreshToken>()
        };
    }

    [Fact]
    public async Task Create_AddsAdministrator() {
        var db = CreateDbContext();
        var service = new AdministratorService(db);
        var admin = CreateAdmin();

        var result = await service.Create(admin, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(admin.Id, result.Id);
        Assert.Single(db.Administrators);
    }

    [Fact]
    public async Task GetById_ReturnsAdministrator_WhenExists() {
        var db = CreateDbContext();
        var admin = CreateAdmin();
        db.Administrators.Add(admin);
        await db.SaveChangesAsync();
        var service = new AdministratorService(db);

        var result = await service.GetById(admin.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(admin.Id, result!.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNull_WhenNotExists() {
        var db = CreateDbContext();
        var service = new AdministratorService(db);

        var result = await service.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAll_ReturnsAllAdministrators() {
        var db = CreateDbContext();
        db.Administrators.AddRange(CreateAdmin(), CreateAdmin());
        await db.SaveChangesAsync();
        var service = new AdministratorService(db);

        var result = await service.GetAll(CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Update_UpdatesAdministrator_WhenExists() {
        var db = CreateDbContext();
        var admin = CreateAdmin();
        db.Administrators.Add(admin);
        await db.SaveChangesAsync();
        var service = new AdministratorService(db);

        var updatedAdmin = new Administrator {
            Id = admin.Id,
            Username = "updatedadmin",
            FirstName = "Jane",
            LastName = "Smith",
            Email = "updated@example.com",
            PasswordHash = "newhash",
            RefreshTokens = new List<RefreshToken>()
        };

        var result = await service.Update(admin.Id, updatedAdmin, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("updatedadmin", result!.Username);
        Assert.Equal("Jane", result.FirstName);
        Assert.Equal("updated@example.com", result.Email);
    }

    [Fact]
    public async Task Update_ReturnsNull_WhenNotExists() {
        var db = CreateDbContext();
        var service = new AdministratorService(db);

        var updatedAdmin = CreateAdmin();
        var result = await service.Update(Guid.NewGuid(), updatedAdmin, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_RemovesAdministrator_WhenExists() {
        var db = CreateDbContext();
        var admin = CreateAdmin();
        db.Administrators.Add(admin);
        await db.SaveChangesAsync();
        var service = new AdministratorService(db);

        var result = await service.Delete(admin.Id, CancellationToken.None);

        Assert.True(result);
        Assert.Empty(db.Administrators);
    }

    [Fact]
    public async Task Delete_ReturnsFalse_WhenNotExists() {
        var db = CreateDbContext();
        var service = new AdministratorService(db);

        var result = await service.Delete(Guid.NewGuid(), CancellationToken.None);

        Assert.False(result);
    }
}
