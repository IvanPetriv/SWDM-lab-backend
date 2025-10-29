using Api.Dtos;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using EFCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;


/// <summary>
/// Provides API endpoints for managing student entities in the university database.
/// </summary>
/// <remarks>This controller supports operations to retrieve and create student records.</remarks>
/// <param name="dbContext"></param>
/// <param name="mapper"></param>
/// <param name="logger"></param>
[ApiController]
[Route("[controller]")]
public class CourseController(
    CourseService service,
    IMapper mapper,
    ILogger<CourseController> logger
) : ControllerBase {
    /// <summary>
    /// Retrieves a student by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the student to retrieve.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="ActionResult{T}"/> containing a <see cref="StudentGetDto"/> if the student is found; otherwise, a
    /// NotFound result.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CourseGetDto>> Get(Guid id, CancellationToken ct) {
        Course? course = await service.GetById(id, ct);
        if (course is null) {
            return NotFound();
        }

        CourseGetDto studentDto = mapper.Map<CourseGetDto>(course);

        return Ok(studentDto);
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseGetDto>>> GetAll(CancellationToken ct) {
        var courses = await service.GetAll(ct);
        var result = mapper.Map<IEnumerable<CourseGetDto>>(courses);
        return Ok(result);
    }


    /// <summary>
    /// Creates a new student record based on the provided data transfer object.
    /// </summary>
    /// <param name="objDto">The data transfer object containing the details of the student to be created.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="ActionResult{T}"/> containing the created student data transfer object if successful; otherwise, a
    /// conflict result if a student with the given ID already exists.</returns>
    [HttpPost]
    public async Task<ActionResult<CourseGetDto>> Create(CourseGetDto objDto, CancellationToken ct) {
        var obj = mapper.Map<Course>(objDto);
        var createdStudent = await service.Create(obj, ct);

        var resultDto = mapper.Map<CourseGetDto>(createdStudent);
        return CreatedAtAction(nameof(Get), new { id = createdStudent.Id }, resultDto);
    }


    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] CourseGetDto objDto, CancellationToken ct) {
        Course obj = mapper.Map<Course>(objDto);
        var updated = await service.Update(id, obj, ct);
        if (updated is null)
            return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct) {
        var deleted = await service.Delete(id, ct);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
