using Api.Dtos;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using EFCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult<IEnumerable<CourseGetDto>>> GetMyCourses(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CourseGetDto>> Get(Guid id, CancellationToken ct)
    {
        Course? course = await service.GetById(id, ct);
        if (course is null)
        {
            return NotFound();
        }

        CourseGetDto studentDto = mapper.Map<CourseGetDto>(course);

        return Ok(studentDto);
    }

    [HttpGet("{id}/files")]
    public async Task<ActionResult<CourseWithFilesDto>> GetWithFiles(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseGetDto>>> GetAll(CancellationToken ct)
    {
        var courses = await service.GetAll(ct);
        var result = mapper.Map<IEnumerable<CourseGetDto>>(courses);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher,Administrator")]
    public async Task<ActionResult<CourseGetDto>> Create(CourseGetDto objDto, CancellationToken ct)
    {
        var obj = mapper.Map<Course>(objDto);
        var createdStudent = await service.Create(obj, ct);

        var resultDto = mapper.Map<CourseGetDto>(createdStudent);
        return CreatedAtAction(nameof(Get), new { id = createdStudent.Id }, resultDto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher,Administrator")]
    public async Task<ActionResult> Update(Guid id, [FromBody] CourseGetDto objDto, CancellationToken ct)
    {
        Course obj = mapper.Map<Course>(objDto);
        var updated = await service.Update(id, obj, ct);
        if (updated is null)
            return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher,Administrator")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await service.Delete(id, ct);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
