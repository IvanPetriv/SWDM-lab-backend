using Application.Services;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTests.Services;
public abstract class BaseServiceTests {
    protected static UniversityDbContext CreateDbContext() {
        var options = new DbContextOptionsBuilder<UniversityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new UniversityDbContext(options);
    }
}
