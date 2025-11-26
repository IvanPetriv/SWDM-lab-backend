using EFCore;
using Microsoft.EntityFrameworkCore;

using UniversityDbContext context = new(); 
var seeder = new DatabaseSeeder();
seeder.Seed();
await seeder.SeedToDatabaseAsync(context);
