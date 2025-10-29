using Api.Dtos;
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
    IMapper mapper
) : ControllerBase
{

    [HttpPost("student")]
    [Authorize(Roles = "Teacher,Administrator")]
    public async Task<ActionResult> AddStudentToCourse([FromBody] AddStudentToCourseDto dto, CancellationToken ct) {
        try {
            await service.AddStudentToCourseAsync(dto.StudentId, dto.CourseId, ct);
            return Ok();
        } catch (InvalidOperationException ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("course/{courseId}/students")]
    public async Task<ActionResult<IEnumerable<StudentGetDto>>> GetCourseStudents(Guid courseId, CancellationToken ct) {
        var students = await service.GetCourseStudentsAsync(courseId, ct);
        var dtos = mapper.Map<IEnumerable<StudentGetDto>>(students);
        return Ok(dtos);
    }

    [HttpPost("teacher")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> AddTeacherToCourse([FromBody] AddTeacherToCourseDto dto, CancellationToken ct)
    {
        try {
            await service.AddTeacherToCourseAsync(dto.TeacherId, dto.CourseId, ct);
            return Ok();
        } catch (InvalidOperationException ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("student")]
    [Authorize(Roles = "Teacher,Administrator")]
    public async Task<ActionResult> RemoveStudentFromCourse([FromQuery] Guid studentId, [FromQuery] Guid courseId, CancellationToken ct)
    {
        var removed = await service.RemoveStudentFromCourseAsync(studentId, courseId, ct);
        if (!removed)
            return NotFound();
        return NoContent();
    }
}
