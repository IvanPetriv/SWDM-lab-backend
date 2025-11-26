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
    FileService service
) : ControllerBase
{
	private static async Task<byte[]> ReadFileBytesAsync(IFormFile file, CancellationToken ct) {
		byte[] buffer = new byte[file.Length];
		using var stream = file.OpenReadStream();
		int totalRead = 0;

		while (totalRead < buffer.Length) {
			int read = await stream.ReadAsync(buffer.AsMemory(totalRead), ct);
			if (read == 0)
				break;
			totalRead += read;
		}

		return buffer;
	}

	private static string NormalizeExtension(string? ext) {
		if (string.IsNullOrWhiteSpace(ext))
			return string.Empty;

		return ext.Trim().TrimStart('.').ToLowerInvariant();
	}

	[HttpPost("upload")]
    [Authorize(Roles = "Teacher,Administrator")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<CourseFileDto>> UploadFile([FromForm] IFormFile file, [FromForm] Guid courseId, CancellationToken ct)
    {
		if (file is null || file.Length == 0)
			return BadRequest("File is empty.");

		var content = await ReadFileBytesAsync(file, ct);
		string ext = NormalizeExtension(Path.GetExtension(file.FileName));

        var uploadedFile = await service.UploadFileAsync(
            content,
            file.FileName,
            ext,
            file.Length,
            courseId,
            ct
        );


        var dto = new CourseFileDto
        {
            Id = uploadedFile.Id,
            FileName = uploadedFile.FileName,
            Type = uploadedFile.FileType,
            Size = uploadedFile.FileSize,
        };

        return Ok(dto);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> DownloadFile(Guid id, CancellationToken ct)
    {
        var file = await service.GetFileAsync(id, ct);
        if (file == null)
        {
            return NotFound();
        }

		string ext = NormalizeExtension(Path.GetExtension(file.FileName));
		string contentType = ext switch {
			"pdf" => "application/pdf",
			"txt" => "text/plain",
			"jpg" or "jpeg" => "image/jpeg",
			"png" => "image/png",
			// fallback
			_ => "application/octet-stream",
		};

		return File(file.FileContent, contentType, file.FileName);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher,Administrator")]
    public async Task<IActionResult> DeleteFile(Guid id, CancellationToken ct)
    {
        var file = await service.GetFileAsync(id, ct);
        if (file == null)
        {
            return NotFound();
        }

        await service.DeleteFileAsync(id, ct);
        return NoContent();
    }
}