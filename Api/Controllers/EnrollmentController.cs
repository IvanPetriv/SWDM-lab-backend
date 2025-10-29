using Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class EnrollmentController : ControllerBase
{

    [HttpPost("student")]
    [Authorize(Roles = "Teacher,Administrator")]
    public async Task<ActionResult> AddStudentToCourse([FromBody] AddStudentToCourseDto dto, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpGet("course/{courseId}/students")]
    public async Task<ActionResult<IEnumerable<StudentGetDto>>> GetCourseStudents(Guid courseId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpPost("teacher")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> AddTeacherToCourse([FromBody] AddTeacherToCourseDto dto, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("student")]
    [Authorize(Roles = "Teacher,Administrator")]
    public async Task<ActionResult> RemoveStudentFromCourse([FromQuery] Guid studentId, [FromQuery] Guid courseId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
