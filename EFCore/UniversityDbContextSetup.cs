using dotenv.net;
using Microsoft.EntityFrameworkCore;

namespace EFCore;
public partial class UniversityDbContext : DbContext {
    public UniversityDbContext() { }

    public UniversityDbContext(DbContextOptions<UniversityDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured) {
            //DotEnv.Load();
            //string? dbConnectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
            //    ?? throw new NullReferenceException("Environment does not have 'DATABASE_CONNECTION_STRING' variable");
            //optionsBuilder.UseNpgsql(dbConnectionString);

            //Console.WriteLine($"Connecting to database: {dbConnectionString}");

            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=University-lab;Username=postgres;Password=ivan");

        }
    }
}
