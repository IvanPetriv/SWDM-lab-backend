using Api.Dtos;
using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class FileController(
    FileService service,
    IMapper mapper
) : ControllerBase
{

    [HttpPost("upload")]
    [Authorize(Roles = "Teacher,Administrator")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<CourseFileDto>> UploadFile( IFormFile file, Guid courseId, CancellationToken ct) {
        byte[] content;
        using (var ms = new MemoryStream()) {
            await file.CopyToAsync(ms, ct);
            content = ms.ToArray();
        }
        var uploadedFile = await service.UploadFileAsync(
            content, file.FileName, courseId, ct);


        var dto = new CourseFileDto {
            Id = uploadedFile.Id,
            FileName = uploadedFile.FileName,
            FileType = uploadedFile.FileType,
            FileSize = uploadedFile.FileSize,
        };

        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> DownloadFile(Guid id, CancellationToken ct) {
        var file = await service.GetFileAsync(id, ct);
        if (file == null) {
            return NotFound();
        }

        return File(file.FileContent, "application/octet-stream", file.FileName);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher,Administrator")]
    public async Task<IActionResult> DeleteFile(Guid id, CancellationToken ct) {
        var file = await service.GetFileAsync(id, ct);
        if (file == null) {
            return NotFound();
        }

        await service.DeleteFileAsync(id, ct);
        return NoContent();
    }
}
