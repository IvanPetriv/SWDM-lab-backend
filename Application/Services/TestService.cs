using Domain.Entities;
using EFCore;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class TestService(UniversityDbContext dbContext)
{
    public async Task<Test?> GetById(Guid id, CancellationToken ct)
    {
        return await dbContext.Tests
            .Include(t => t.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<ICollection<Test>> GetByCourseId(Guid courseId, CancellationToken ct)
    {
        return await dbContext.Tests
            .Include(t => t.Questions)
                .ThenInclude(q => q.Options)
            .Where(t => t.CourseId == courseId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<Test> Create(Test test, List<Question> questions, CancellationToken ct)
    {
        test.Id = Guid.NewGuid();
        test.CreatedAt = DateTime.UtcNow;

        for (int i = 0; i < questions.Count; i++)
        {
            var question = questions[i];
            question.Id = Guid.NewGuid();
            question.TestId = test.Id;
            question.OrderIndex = i;

            for (int j = 0; j < question.Options.Count; j++)
            {
                var option = question.Options.ToList()[j];
                option.Id = Guid.NewGuid();
                option.QuestionId = question.Id;
                option.OrderIndex = j;
            }
        }

        test.Questions = questions;

        await dbContext.Tests.AddAsync(test, ct);
        await dbContext.SaveChangesAsync(ct);

        return test;
    }

    public async Task<bool> Delete(Guid id, CancellationToken ct)
    {
        var test = await dbContext.Tests
            .Include(t => t.Questions)
                .ThenInclude(q => q.Options)
            .Include(t => t.Submissions)
                .ThenInclude(s => s.Answers)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (test is null)
            return false;

        dbContext.Tests.Remove(test);
        await dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> CourseExists(Guid courseId, CancellationToken ct)
    {
        return await dbContext.Courses.AnyAsync(c => c.Id == courseId, ct);
    }

    public async Task<bool> UserHasAccessToCourse(Guid courseId, Guid userId, CancellationToken ct)
    {
        var isTeacher = await dbContext.Courses
            .AnyAsync(c => c.Id == courseId && c.TeacherId == userId, ct);

        if (isTeacher)
            return true;

        var isEnrolled = await dbContext.Enrollments
            .AnyAsync(e => e.CourseId == courseId && e.StudentId == userId, ct);

        return isEnrolled;
    }

    public async Task<bool> IsTeacherOfCourse(Guid courseId, Guid userId, CancellationToken ct)
    {
        return await dbContext.Courses
            .AnyAsync(c => c.Id == courseId && c.TeacherId == userId, ct);
    }

    public async Task<TestSubmission?> SubmitTest(Guid testId, Guid studentId, List<(Guid questionId, Guid selectedOptionId)> answers, CancellationToken ct)
    {
        var test = await dbContext.Tests
            .Include(t => t.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(t => t.Id == testId, ct);

        if (test is null)
            return null;

        // Check if student already submitted
        var existingSubmission = await dbContext.TestSubmissions
            .AnyAsync(s => s.TestId == testId && s.StudentId == studentId, ct);

        if (existingSubmission)
            return null;

        // Check if due date has passed
        if (DateTime.UtcNow > test.DueDate)
            return null;

        // Calculate grade
        int correctAnswers = 0;
        var submissionAnswers = new List<SubmissionAnswer>();

        foreach (var answer in answers)
        {
            var question = test.Questions.FirstOrDefault(q => q.Id == answer.questionId);
            if (question is null)
                continue;

            var selectedOption = question.Options.FirstOrDefault(o => o.Id == answer.selectedOptionId);
            if (selectedOption is null)
                continue;

            if (selectedOption.IsCorrect)
                correctAnswers++;

            submissionAnswers.Add(new SubmissionAnswer
            {
                Id = Guid.NewGuid(),
                QuestionId = answer.questionId,
                SelectedOptionId = answer.selectedOptionId
            });
        }

        double grade = test.Questions.Count > 0
            ? (double)correctAnswers / test.Questions.Count * 100
            : 0;

        var submission = new TestSubmission
        {
            Id = Guid.NewGuid(),
            TestId = testId,
            StudentId = studentId,
            SubmittedAt = DateTime.UtcNow,
            Grade = Math.Round(grade, 2),
            Answers = submissionAnswers
        };

        await dbContext.TestSubmissions.AddAsync(submission, ct);
        await dbContext.SaveChangesAsync(ct);

        return submission;
    }

    public async Task<TestSubmission?> GetSubmissionById(Guid submissionId, CancellationToken ct)
    {
        return await dbContext.TestSubmissions
            .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
            .Include(s => s.Answers)
                .ThenInclude(a => a.SelectedOption)
            .Include(s => s.Student)
            .FirstOrDefaultAsync(s => s.Id == submissionId, ct);
    }

    public async Task<ICollection<TestSubmission>> GetSubmissionsByTestId(Guid testId, CancellationToken ct)
    {
        return await dbContext.TestSubmissions
            .Include(s => s.Student)
            .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
            .Include(s => s.Answers)
                .ThenInclude(a => a.SelectedOption)
            .Where(s => s.TestId == testId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync(ct);
    }

    public async Task<ICollection<TestSubmission>> GetSubmissionsByStudentId(Guid studentId, CancellationToken ct)
    {
        return await dbContext.TestSubmissions
            .Include(s => s.Test)
            .Include(s => s.Answers)
                .ThenInclude(a => a.Question)
            .Include(s => s.Answers)
                .ThenInclude(a => a.SelectedOption)
            .Where(s => s.StudentId == studentId)
            .OrderByDescending(s => s.SubmittedAt)
            .ToListAsync(ct);
    }

    public async Task<bool> HasStudentSubmitted(Guid testId, Guid studentId, CancellationToken ct)
    {
        return await dbContext.TestSubmissions
            .AnyAsync(s => s.TestId == testId && s.StudentId == studentId, ct);
    }

    public async Task<ICollection<Student>> GetStudentsWhoDidNotSubmit(Guid testId, CancellationToken ct)
    {
        var test = await dbContext.Tests
            .Include(t => t.Course)
            .FirstOrDefaultAsync(t => t.Id == testId, ct);

        if (test is null)
            return [];

        var enrolledStudents = await dbContext.Enrollments
            .Where(e => e.CourseId == test.CourseId)
            .Select(e => e.StudentId)
            .ToListAsync(ct);

        var submittedStudents = await dbContext.TestSubmissions
            .Where(s => s.TestId == testId)
            .Select(s => s.StudentId)
            .ToListAsync(ct);

        var notSubmittedStudentIds = enrolledStudents.Except(submittedStudents).ToList();

        return await dbContext.Students
            .Where(s => notSubmittedStudentIds.Contains(s.Id))
            .ToListAsync(ct);
    }
}
