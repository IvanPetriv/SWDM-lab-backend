using Api.Dtos;
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
public class StudentController(
    UniversityDbContext dbContext,
    IMapper mapper,
    ILogger<StudentController> logger
) : ControllerBase {
    [HttpGet("{id}")]
    public async Task<ActionResult<StudentGetDto>> Get(Guid id, CancellationToken ct) {
        var student = await dbContext.Students.SingleOrDefaultAsync(e => e.Id == id, ct);
        if (student is null) {
            return NotFound();
        }

        var studentDto = mapper.Map<StudentGetDto>(student);

        return Ok(studentDto);
    }

    [HttpPost]
    public async Task<ActionResult<StudentGetDto>> Create(StudentGetDto objDto, CancellationToken ct) {
        if (dbContext.Students.Any(e => e.Id == objDto.Id)) {
            return Conflict("Student with the given ID already exists.");
        }

        var obj = mapper.Map<Student>(objDto);
        var student = await dbContext.Students.AddAsync(obj, ct);
        await dbContext.SaveChangesAsync(ct);

        return Ok(student);
    }
}
