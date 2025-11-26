using Bogus;
using Domain.Entities;
using System;
using System.Collections.Generic;

namespace EFCore.Fakers {
	internal class EnrollmentFaker : Faker<Enrollment> {
		private readonly List<Student> _students;
		private readonly List<Course> _courses;
		private readonly HashSet<(Guid, Guid)> _existingKeys = [];

		public EnrollmentFaker(List<Student> students, List<Course> courses) {
			_students = students;
			_courses = courses;

			RuleFor(e => e.EnrolledAt, f => f.Date.Past(1).ToUniversalTime());
			RuleFor(e => e.Grade, f => f.Random.Int(0, 100));

			// Custom generator for StudentId and CourseId
			CustomInstantiator(f => {
				int attempts = 0;
				while (true) {
					attempts++;
					var studentId = _students[Random.Shared.Next(_students.Count)].Id;
					var courseId = _courses[Random.Shared.Next(_courses.Count)].Id;

					if (_existingKeys.Add((studentId, courseId)) || attempts > 100) {
						return new Enrollment {
							StudentId = studentId,
							CourseId = courseId,
							EnrolledAt = f.Date.Past(1).ToUniversalTime(),
							Grade = f.Random.Int(0, 100)
						};
					}
				}
			});
		}
	}
}
