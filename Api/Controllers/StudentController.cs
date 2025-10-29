using Api.Dtos;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
///     Provides API endpoints for managing student entities in the university database.
/// </summary>
/// <remarks>This controller supports operations to retrieve and create student records.</remarks>
/// <param name="dbContext"></param>
/// <param name="mapper"></param>
/// <param name="logger"></param>
[ApiController]
[Route("[controller]")]
[Authorize]
public class StudentController(
    StudentService service,
    IMapper mapper,
    ILogger<StudentController> logger
) : ControllerBase
{
    /// <summary>
    ///     Retrieves a student by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the student to retrieve.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    ///     An <see cref="ActionResult{T}" /> containing a <see cref="StudentGetDto" /> if the student is found; otherwise, a
    ///     NotFound result.
    /// </returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<StudentGetDto>> Get(Guid id, CancellationToken ct)
    {
        var student = await service.GetStudent(id, ct);
        if (student is null) return NotFound();

        var studentDto = mapper.Map<StudentGetDto>(student);

        return Ok(studentDto);
    }


    /// <summary>
    ///     Creates a new student record based on the provided data transfer object.
    /// </summary>
    /// <param name="objDto">The data transfer object containing the details of the student to be created.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>
    ///     An <see cref="ActionResult{T}" /> containing the created student data transfer object if successful; otherwise, a
    ///     conflict result if a student with the given ID already exists.
    /// </returns>
    [HttpPost]
    public async Task<ActionResult<StudentGetDto>> Create(StudentGetDto objDto, CancellationToken ct)
    {
        var existingStudent = await service.GetStudent(objDto.Id, ct);
        if (existingStudent is not null) return Conflict("Student with the given ID already exists.");

        var obj = mapper.Map<Student>(objDto);
        var createdStudent = await service.CreateStudent(obj, ct);

        return Ok(createdStudent);
    }
}