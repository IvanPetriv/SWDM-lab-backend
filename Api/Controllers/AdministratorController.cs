using Api.Dtos;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using EFCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Api.Controllers;


/// <summary>
/// Provides API endpoints for managing administrator entities in the university database.
/// </summary>
/// <remarks>This controller supports operations to retrieve and create administrator records.</remarks>
[ApiController]
[Route("[controller]")]
public class AdministratorController(
    AdministratorService administratorService,
    CourseService courseService,
    UserService userService,
    IMapper mapper,
    ILogger<AdministratorController> logger
) : ControllerBase
{
    /// <summary>
    /// Retrieves a student by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the student to retrieve.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="ActionResult{T}"/> containing a <see cref="StudentGetDto"/> if the student is found; otherwise, a
    /// NotFound result.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<AdministratorGetDto>> Get(Guid id, CancellationToken ct)
    {
        var student = await administratorService.GetById(id, ct);
        if (student is null)
        {
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
    public async Task<ActionResult<AdministratorGetDto>> Create(AdministratorGetDto objDto, CancellationToken ct)
    {
        var obj = mapper.Map<Administrator>(objDto);
        var createdStudent = await administratorService.Create(obj, ct);

        return Ok(createdStudent);
    }

    /// <summary>
    /// Retrieves the courses associated with a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="ActionResult{T}"/> containing a list of <see cref="CourseGetDto"/> if the user is found; otherwise, a NotFound or BadRequest result.</returns>
    [HttpGet("users/{userId}/courses")]
    public async Task<ActionResult<IEnumerable<CourseGetDto>>> GetCoursesForUser(Guid userId, CancellationToken ct)
    {
        var user = await userService.GetById(userId, ct);
        if (user == null)
        {
            return NotFound();
        }

        ICollection<Course> courses;
        if (user is Teacher)
        {
            courses = await courseService.GetAllOfTeacher(userId, ct);
        }
        else if (user is Student)
        {
            courses = await courseService.GetAllOfStudent(userId, ct);
        }
        else
        {
            return BadRequest("User is not a teacher or student");
        }

        var result = mapper.Map<IEnumerable<CourseGetDto>>(courses);
        return Ok(result);
    }
}
