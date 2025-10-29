using Api.Dtos;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using EFCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class CourseController(
    CourseService service,
    IMapper mapper
) : ControllerBase
{
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<CourseGetDto>>> GetMyCourses(CancellationToken ct) {
        // Assumes user ID is in claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var courses = await service.GetByUserIdAsync(userId, ct);
        var dtos = mapper.Map<IEnumerable<CourseGetDto>>(courses);
        return Ok(dtos);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<CourseGetDto>> Get(Guid id, CancellationToken ct)
    {
        Course? course = await service.GetByIdAsync(id, ct);
        if (course is null)
        {
            return NotFound();
        }

        CourseGetDto studentDto = mapper.Map<CourseGetDto>(course);

        return Ok(studentDto);
    }


    [HttpGet("{id}/files")]
    public async Task<ActionResult<CourseWithFilesDto>> GetWithFiles(Guid id, CancellationToken ct) {
        var course = await service.GetWithFilesAsync(id, ct);
        if (course is null)
            return NotFound();

        var dto = mapper.Map<CourseWithFilesDto>(course);
        dto.Files = course.MediaMaterials
            .Select(m => mapper.Map<CourseFileDto>(m.CourseFile))
            .ToList();
        return Ok(dto);
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseGetDto>>> GetAll(CancellationToken ct)
    {
        var courses = await service.GetAllAsync(ct);
        var result = mapper.Map<IEnumerable<CourseGetDto>>(courses);
        return Ok(result);
    }


    [HttpPost]
    [Authorize(Roles = "Teacher,Administrator")]
    public async Task<ActionResult<CourseGetDto>> Create(CourseGetDto objDto, CancellationToken ct)
    {
        var obj = mapper.Map<Course>(objDto);
        var createdStudent = await service.CreateAsync(obj, ct);

        var resultDto = mapper.Map<CourseGetDto>(createdStudent);
        return CreatedAtAction(nameof(Get), new { id = createdStudent.Id }, resultDto);
    }


    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher,Administrator")]
    public async Task<ActionResult> Update(Guid id, [FromBody] CourseGetDto objDto, CancellationToken ct)
    {
        Course obj = mapper.Map<Course>(objDto);
        var updated = await service.UpdateAsync(id, obj, ct);
        if (updated is null)
            return NotFound();
        return NoContent();
    }


    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher,Administrator")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await service.DeleteAsync(id, ct);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
