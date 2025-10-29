using Api.Dtos;
using Api.Extensions;
using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class EnrollmentController(
    EnrollmentService service,
    CourseService courseService,
    IMapper mapper
) : ControllerBase
{

    [HttpPost("student")]
    [Authorize(Roles = "Teacher,Administrator")]
    public async Task<ActionResult> AddStudentToCourse([FromBody] AddStudentToCourseDto dto, CancellationToken ct)
    {
        // For teachers, verify they own the course
        var userRole = this.GetCurrentUserRole();
        if (userRole == "Teacher")
        {
            var userId = this.GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            if (!await courseService.IsTeacherOwner(dto.CourseId, userId.Value, ct))
                return Forbid();
        }

        var enrollment = await service.AddStudentToCourse(dto.StudentId, dto.CourseId, ct);
        if (enrollment == null)
            return NotFound("Student or course not found");

        return Ok(new { message = "Student successfully enrolled in course" });
    }

    [HttpGet("course/{courseId}/students")]
    public async Task<ActionResult<IEnumerable<StudentGetDto>>> GetCourseStudents(Guid courseId, CancellationToken ct)
    {
        var students = await service.GetCourseStudents(courseId, ct);
        var result = mapper.Map<IEnumerable<StudentGetDto>>(students);
        return Ok(result);
    }

    [HttpPost("teacher")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> AddTeacherToCourse([FromBody] AddTeacherToCourseDto dto, CancellationToken ct)
    {
        var course = await service.AddTeacherToCourse(dto.TeacherId, dto.CourseId, ct);
        if (course == null)
            return NotFound("Teacher or course not found");

        return Ok(new { message = "Teacher successfully assigned to course" });
    }

    [HttpDelete("student")]
    [Authorize(Roles = "Teacher,Administrator")]
    public async Task<ActionResult> RemoveStudentFromCourse([FromQuery] Guid studentId, [FromQuery] Guid courseId, CancellationToken ct)
    {
        // For teachers, verify they own the course
        var userRole = this.GetCurrentUserRole();
        if (userRole == "Teacher")
        {
            var userId = this.GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            if (!await courseService.IsTeacherOwner(courseId, userId.Value, ct))
                return Forbid();
        }

        var removed = await service.RemoveStudentFromCourse(studentId, courseId, ct);
        if (!removed)
            return NotFound("Enrollment not found");

        return NoContent();
    }

    [HttpDelete("teacher")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> RemoveTeacherFromCourse([FromQuery] Guid courseId, CancellationToken ct)
    {
        var removed = await service.RemoveTeacherFromCourse(courseId, ct);
        if (!removed)
            return NotFound("Course not found");

        return NoContent();
    }
}
