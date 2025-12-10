using Api.Dtos;
using Api.Extensions;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TestController(TestService testService) : ControllerBase
{
    /// <summary>
    /// Gets all tests for a specific course.
    /// </summary>
    /// <param name="courseId">Course ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Collection of tests for the course.</returns>
    [HttpGet("course/{courseId}")]
    public async Task<ActionResult<IEnumerable<TestGetDto>>> GetByCourseId(Guid courseId, CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var hasAccess = await testService.UserHasAccessToCourse(courseId, userId.Value, ct);
        if (!hasAccess)
            return Forbid();

        var tests = await testService.GetByCourseId(courseId, ct);

        var testsDto = tests.Select(t => new TestGetDto
        {
            Id = t.Id,
            CourseId = t.CourseId,
            Title = t.Title,
            Description = t.Description,
            DueDate = t.DueDate,
            CreatedAt = t.CreatedAt,
            Questions = t.Questions.OrderBy(q => q.OrderIndex).Select(q => new QuestionGetDto
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                OrderIndex = q.OrderIndex,
                Options = q.Options.OrderBy(o => o.OrderIndex).Select(o => new QuestionOptionGetDto
                {
                    Id = o.Id,
                    OptionText = o.OptionText,
                    OrderIndex = o.OrderIndex
                }).ToList()
            }).ToList()
        }).ToList();

        return Ok(testsDto);
    }

    /// <summary>
    /// Gets a specific test by ID.
    /// </summary>
    /// <param name="id">Test ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Test details.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<TestGetDto>> GetById(Guid id, CancellationToken ct)
    {
        var test = await testService.GetById(id, ct);
        if (test is null)
            return NotFound();

        var userId = this.GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var hasAccess = await testService.UserHasAccessToCourse(test.CourseId, userId.Value, ct);
        if (!hasAccess)
            return Forbid();

        var testDto = new TestGetDto
        {
            Id = test.Id,
            CourseId = test.CourseId,
            Title = test.Title,
            Description = test.Description,
            DueDate = test.DueDate,
            CreatedAt = test.CreatedAt,
            Questions = test.Questions.OrderBy(q => q.OrderIndex).Select(q => new QuestionGetDto
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                OrderIndex = q.OrderIndex,
                Options = q.Options.OrderBy(o => o.OrderIndex).Select(o => new QuestionOptionGetDto
                {
                    Id = o.Id,
                    OptionText = o.OptionText,
                    OrderIndex = o.OrderIndex
                }).ToList()
            }).ToList()
        };

        return Ok(testDto);
    }

    /// <summary>
    /// Gets a test with correct answers (teacher only).
    /// </summary>
    /// <param name="id">Test ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Test details with correct answers.</returns>
    [HttpGet("{id}/with-answers")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<TestWithCorrectAnswersDto>> GetByIdWithAnswers(Guid id, CancellationToken ct)
    {
        var test = await testService.GetById(id, ct);
        if (test is null)
            return NotFound();

        var userId = this.GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var isTeacher = await testService.IsTeacherOfCourse(test.CourseId, userId.Value, ct);
        if (!isTeacher)
            return Forbid();

        var testDto = new TestWithCorrectAnswersDto
        {
            Id = test.Id,
            CourseId = test.CourseId,
            Title = test.Title,
            Description = test.Description,
            DueDate = test.DueDate,
            CreatedAt = test.CreatedAt,
            Questions = test.Questions.OrderBy(q => q.OrderIndex).Select(q => new QuestionWithCorrectAnswerDto
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                OrderIndex = q.OrderIndex,
                Options = q.Options.OrderBy(o => o.OrderIndex).Select(o => new QuestionOptionWithCorrectDto
                {
                    Id = o.Id,
                    OptionText = o.OptionText,
                    IsCorrect = o.IsCorrect,
                    OrderIndex = o.OrderIndex
                }).ToList()
            }).ToList()
        };

        return Ok(testDto);
    }

    /// <summary>
    /// Creates a new test for a course. Only teachers can create tests.
    /// </summary>
    /// <param name="testDto">Test creation data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Created test.</returns>
    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<TestGetDto>> Create([FromBody] TestCreateDto testDto, CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var courseExists = await testService.CourseExists(testDto.CourseId, ct);
        if (!courseExists)
            return NotFound("Course not found");

        var isTeacher = await testService.IsTeacherOfCourse(testDto.CourseId, userId.Value, ct);
        if (!isTeacher)
            return Forbid();

        // Validate that each question has at least one correct answer
        foreach (var question in testDto.Questions)
        {
            if (!question.Options.Any(o => o.IsCorrect))
                return BadRequest($"Question '{question.QuestionText}' must have at least one correct answer");
        }

        var test = new Test
        {
            CourseId = testDto.CourseId,
            Title = testDto.Title,
            Description = testDto.Description,
            DueDate = DateTime.SpecifyKind(testDto.DueDate, DateTimeKind.Utc)
        };

        var questions = testDto.Questions.Select(q => new Question
        {
            QuestionText = q.QuestionText,
            Options = q.Options.Select(o => new QuestionOption
            {
                OptionText = o.OptionText,
                IsCorrect = o.IsCorrect
            }).ToList()
        }).ToList();

        var createdTest = await testService.Create(test, questions, ct);

        var resultDto = new TestGetDto
        {
            Id = createdTest.Id,
            CourseId = createdTest.CourseId,
            Title = createdTest.Title,
            Description = createdTest.Description,
            DueDate = createdTest.DueDate,
            CreatedAt = createdTest.CreatedAt,
            Questions = createdTest.Questions.OrderBy(q => q.OrderIndex).Select(q => new QuestionGetDto
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                OrderIndex = q.OrderIndex,
                Options = q.Options.OrderBy(o => o.OrderIndex).Select(o => new QuestionOptionGetDto
                {
                    Id = o.Id,
                    OptionText = o.OptionText,
                    OrderIndex = o.OrderIndex
                }).ToList()
            }).ToList()
        };

        return CreatedAtAction(nameof(GetById), new { id = createdTest.Id }, resultDto);
    }

    /// <summary>
    /// Deletes a test. Only teachers can delete tests.
    /// </summary>
    /// <param name="id">Test ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var test = await testService.GetById(id, ct);
        if (test is null)
            return NotFound();

        var isTeacher = await testService.IsTeacherOfCourse(test.CourseId, userId.Value, ct);
        if (!isTeacher)
            return Forbid();

        var deleted = await testService.Delete(id, ct);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Submits a test. Students can only submit once and before the due date.
    /// </summary>
    /// <param name="submissionDto">Test submission data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Submission details with grade.</returns>
    [HttpPost("submit")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<TestSubmissionGetDto>> SubmitTest([FromBody] TestSubmissionCreateDto submissionDto, CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var test = await testService.GetById(submissionDto.TestId, ct);
        if (test is null)
            return NotFound("Test not found");

        var hasAccess = await testService.UserHasAccessToCourse(test.CourseId, userId.Value, ct);
        if (!hasAccess)
            return Forbid();

        var alreadySubmitted = await testService.HasStudentSubmitted(submissionDto.TestId, userId.Value, ct);
        if (alreadySubmitted)
            return BadRequest("You have already submitted this test");

        if (DateTime.UtcNow > test.DueDate)
            return BadRequest("The due date for this test has passed");

        var answers = submissionDto.Answers
            .Select(a => (a.QuestionId, a.SelectedOptionId))
            .ToList();

        var submission = await testService.SubmitTest(submissionDto.TestId, userId.Value, answers, ct);
        if (submission is null)
            return BadRequest("Failed to submit test");

        var submissionWithDetails = await testService.GetSubmissionById(submission.Id, ct);
        if (submissionWithDetails is null)
            return NotFound();

        var resultDto = new TestSubmissionGetDto
        {
            Id = submissionWithDetails.Id,
            TestId = submissionWithDetails.TestId,
            StudentId = submissionWithDetails.StudentId,
            StudentName = $"{submissionWithDetails.Student.FirstName} {submissionWithDetails.Student.LastName}",
            SubmittedAt = submissionWithDetails.SubmittedAt,
            Grade = submissionWithDetails.Grade,
            Answers = submissionWithDetails.Answers.Select(a => new SubmissionAnswerGetDto
            {
                QuestionId = a.QuestionId,
                QuestionText = a.Question.QuestionText,
                SelectedOptionId = a.SelectedOptionId,
                SelectedOptionText = a.SelectedOption.OptionText,
                IsCorrect = a.SelectedOption.IsCorrect
            }).ToList()
        };

        return Ok(resultDto);
    }

    /// <summary>
    /// Gets all submissions for a test (teacher only).
    /// </summary>
    /// <param name="testId">Test ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Collection of submissions.</returns>
    [HttpGet("{testId}/submissions")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<IEnumerable<TestSubmissionGetDto>>> GetSubmissionsByTestId(Guid testId, CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var test = await testService.GetById(testId, ct);
        if (test is null)
            return NotFound();

        var isTeacher = await testService.IsTeacherOfCourse(test.CourseId, userId.Value, ct);
        if (!isTeacher)
            return Forbid();

        var submissions = await testService.GetSubmissionsByTestId(testId, ct);

        var submissionsDto = submissions.Select(s => new TestSubmissionGetDto
        {
            Id = s.Id,
            TestId = s.TestId,
            StudentId = s.StudentId,
            StudentName = $"{s.Student.FirstName} {s.Student.LastName}",
            SubmittedAt = s.SubmittedAt,
            Grade = s.Grade,
            Answers = s.Answers.Select(a => new SubmissionAnswerGetDto
            {
                QuestionId = a.QuestionId,
                QuestionText = a.Question.QuestionText,
                SelectedOptionId = a.SelectedOptionId,
                SelectedOptionText = a.SelectedOption.OptionText,
                IsCorrect = a.SelectedOption.IsCorrect
            }).ToList()
        }).ToList();

        return Ok(submissionsDto);
    }

    /// <summary>
    /// Gets students who didn't submit a test (teacher only).
    /// </summary>
    /// <param name="testId">Test ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Collection of students who didn't submit.</returns>
    [HttpGet("{testId}/not-submitted")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<IEnumerable<StudentNotSubmittedDto>>> GetStudentsWhoDidNotSubmit(Guid testId, CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var test = await testService.GetById(testId, ct);
        if (test is null)
            return NotFound();

        var isTeacher = await testService.IsTeacherOfCourse(test.CourseId, userId.Value, ct);
        if (!isTeacher)
            return Forbid();

        var students = await testService.GetStudentsWhoDidNotSubmit(testId, ct);

        var studentsDto = students.Select(s => new StudentNotSubmittedDto
        {
            StudentId = s.Id,
            StudentName = $"{s.FirstName} {s.LastName}",
            StudentEmail = s.Email
        }).ToList();

        return Ok(studentsDto);
    }

    /// <summary>
    /// Gets all submissions for the current student.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Collection of student's submissions.</returns>
    [HttpGet("my-submissions")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<IEnumerable<TestSubmissionGetDto>>> GetMySubmissions(CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var submissions = await testService.GetSubmissionsByStudentId(userId.Value, ct);

        var submissionsDto = submissions.Select(s => new TestSubmissionGetDto
        {
            Id = s.Id,
            TestId = s.TestId,
            StudentId = s.StudentId,
            StudentName = $"{s.Test.Title}",
            SubmittedAt = s.SubmittedAt,
            Grade = s.Grade,
            Answers = s.Answers.Select(a => new SubmissionAnswerGetDto
            {
                QuestionId = a.QuestionId,
                QuestionText = a.Question.QuestionText,
                SelectedOptionId = a.SelectedOptionId,
                SelectedOptionText = a.SelectedOption.OptionText,
                IsCorrect = a.SelectedOption.IsCorrect
            }).ToList()
        }).ToList();

        return Ok(submissionsDto);
    }

    /// <summary>
    /// Gets a specific submission by ID.
    /// </summary>
    /// <param name="id">Submission ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Submission details.</returns>
    [HttpGet("submission/{id}")]
    public async Task<ActionResult<TestSubmissionGetDto>> GetSubmissionById(Guid id, CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();
        if (userId is null)
            return Unauthorized();

        var submission = await testService.GetSubmissionById(id, ct);
        if (submission is null)
            return NotFound();

        var test = await testService.GetById(submission.TestId, ct);
        if (test is null)
            return NotFound();

        // Check if user is either the student who submitted or the teacher
        var isStudent = submission.StudentId == userId.Value;
        var isTeacher = await testService.IsTeacherOfCourse(test.CourseId, userId.Value, ct);

        if (!isStudent && !isTeacher)
            return Forbid();

        var submissionDto = new TestSubmissionGetDto
        {
            Id = submission.Id,
            TestId = submission.TestId,
            StudentId = submission.StudentId,
            StudentName = $"{submission.Student.FirstName} {submission.Student.LastName}",
            SubmittedAt = submission.SubmittedAt,
            Grade = submission.Grade,
            Answers = submission.Answers.Select(a => new SubmissionAnswerGetDto
            {
                QuestionId = a.QuestionId,
                QuestionText = a.Question.QuestionText,
                SelectedOptionId = a.SelectedOptionId,
                SelectedOptionText = a.SelectedOption.OptionText,
                IsCorrect = a.SelectedOption.IsCorrect
            }).ToList()
        };

        return Ok(submissionDto);
    }
}
