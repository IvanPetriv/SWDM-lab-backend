using System.Collections.Generic;
using Domain.Entities;
using EFCore.Fakers;

namespace EFCore {
	public class DatabaseSeeder {
		// Generated entities
		public List<Teacher> Teachers { get; private set; } = [];
		public List<Administrator> Administrators { get; private set; } = [];
		public List<Course> Courses { get; private set; } = [];
		public List<Student> Students { get; private set; } = [];
		public List<Enrollment> Enrollments { get; private set; } = [];
		public List<TextMaterial> TextMaterials { get; private set; } = [];
		public List<MediaMaterial> MediaMaterials { get; private set; } = [];

		/// <summary>
		/// Seed all data in proper order.
		/// </summary>
		public void Seed(
			int teacherCount = 5,
			int adminCount = 5,
			int studentCount = 20,
			int courseCount = 10,
			int enrollmentCount = 30,
			int textMaterialCount = 20,
			int mediaMaterialCount = 20
		) {
			// Teachers
			var teacherFaker = new TeacherFaker();
			Teachers = teacherFaker.Generate(teacherCount);

			// Administrators
			var administratorFaker = new AdministratorFaker();
			Administrators = administratorFaker.Generate(adminCount);

			// Courses
			var courseFaker = new CourseFaker(Teachers);
			Courses = courseFaker.Generate(courseCount);

			// Students
			var studentFaker = new StudentFaker();
			Students = studentFaker.Generate(studentCount);

			// Enrollments
			var enrollmentFaker = new EnrollmentFaker(Students, Courses);
			Enrollments = enrollmentFaker.Generate(enrollmentCount);

			// Text Materials
			var textMaterialFaker = new TextMaterialFaker(Courses);
			TextMaterials = textMaterialFaker.Generate(textMaterialCount);

			// Media Materials
			var mediaMaterialFaker = new MediaMaterialFaker(Courses);
			MediaMaterials = mediaMaterialFaker.Generate(mediaMaterialCount);
		}

		public async Task SeedToDatabaseAsync(UniversityDbContext context) {
			if (context == null) throw new ArgumentNullException(nameof(context));

			// Use a transaction to ensure atomicity
			await using var transaction = await context.Database.BeginTransactionAsync();

			try {
				// Add entities in proper order to respect foreign keys
				await context.Teachers.AddRangeAsync(Teachers);
				await context.Administrators.AddRangeAsync(Administrators);
				await context.Students.AddRangeAsync(Students);
				await context.Courses.AddRangeAsync(Courses);
				await context.Enrollments.AddRangeAsync(Enrollments);
				await context.TextMaterials.AddRangeAsync(TextMaterials);
				await context.MediaMaterials.AddRangeAsync(MediaMaterials);

				await context.SaveChangesAsync();

				await transaction.CommitAsync();
			} catch {
				await transaction.RollbackAsync();
				throw;
			}
		}
	}
}
