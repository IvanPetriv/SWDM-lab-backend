using Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class FileController : ControllerBase
{

    [HttpPost("upload")]
    [Authorize(Roles = "Teacher,Administrator")]
    public async Task<ActionResult<CourseFileDto>> UploadFile([FromForm] IFormFile file, [FromForm] Guid courseId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> DownloadFile(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher,Administrator")]
    public async Task<ActionResult> DeleteFile(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
