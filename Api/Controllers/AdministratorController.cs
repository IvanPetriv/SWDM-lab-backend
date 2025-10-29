using Api.Dtos;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using EFCore;
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
public class AdministratorController(
    AdministratorService service,
    IMapper mapper,
    ILogger<AdministratorController> logger
) : ControllerBase {
    /// <summary>
    /// Retrieves a student by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the student to retrieve.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="ActionResult{T}"/> containing a <see cref="StudentGetDto"/> if the student is found; otherwise, a
    /// NotFound result.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<AdministratorGetDto>> Get(Guid id, CancellationToken ct) {
        var student = await service.GetById(id, ct);
        if (student is null) {
            return NotFound();
        }

        var studentDto = mapper.Map<AdministratorGetDto>(student);

        return Ok(studentDto);
    }


    /// <summary>
    /// Creates a new student record based on the provided data transfer object.
    /// </summary>
    /// <param name="objDto">The data transfer object containing the details of the student to be created.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="ActionResult{T}"/> containing the created student data transfer object if successful; otherwise, a
    /// conflict result if a student with the given ID already exists.</returns>
    [HttpPost]
    public async Task<ActionResult<AdministratorGetDto>> Create(AdministratorGetDto objDto, CancellationToken ct) {
        var obj = mapper.Map<Administrator>(objDto);
        var createdStudent = await service.Create(obj, ct);

        return Ok(createdStudent);
    }
}
