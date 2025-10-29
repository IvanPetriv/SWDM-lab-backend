using Api.Dtos;
using Api.Extensions;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UserController(
    UserService service,
    IMapper mapper
) : ControllerBase
{

    [HttpGet("me")]
    public async Task<ActionResult<CurrentUserDto>> GetCurrentUser(CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var user = await service.GetById(userId.Value, ct);
        if (user == null)
            return NotFound();

        var userDto = mapper.Map<CurrentUserDto>(user);
        userDto.Role = this.GetCurrentUserRole() ?? string.Empty;

        return Ok(userDto);
    }

    [HttpPut("me")]
    public async Task<ActionResult> UpdateCurrentUser([FromBody] UserGetDto userDto, CancellationToken ct)
    {
        var userId = this.GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var user = mapper.Map<User>(userDto);
        var updated = await service.Update(userId.Value, user, ct);

        if (updated == null)
            return NotFound();

        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<UserSearchDto>>> SearchUsers([FromQuery] string username, CancellationToken ct)
    {
        var users = await service.SearchByUsername(username, ct);
        var result = mapper.Map<IEnumerable<UserSearchDto>>(users);
        return Ok(result);
    }

    [HttpGet("search/students")]
    public async Task<ActionResult<IEnumerable<UserSearchDto>>> SearchStudents([FromQuery] string username, CancellationToken ct)
    {
        var students = await service.SearchStudentsByUsername(username, ct);
        var result = mapper.Map<IEnumerable<UserSearchDto>>(students);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<IEnumerable<UserSearchDto>>> GetAllUsers(CancellationToken ct)
    {
        var users = await service.GetAll(ct);
        var result = mapper.Map<IEnumerable<UserSearchDto>>(users);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<UserGetDto>> CreateUser([FromBody] CreateUserDto userDto, CancellationToken ct)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

        User user = userDto.Role.ToLowerInvariant() switch
        {
            "administrator" or "admin" => new Administrator
            {
                Username = userDto.Username,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PasswordHash = hashedPassword
            },
            "teacher" => new Teacher
            {
                Username = userDto.Username,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PasswordHash = hashedPassword
            },
            _ => new Student
            {
                Username = userDto.Username,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PasswordHash = hashedPassword
            }
        };

        var createdUser = await service.Create(user, ct);
        var resultDto = mapper.Map<UserGetDto>(createdUser);
        return CreatedAtAction(nameof(GetCurrentUser), resultDto);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> DeleteUser(Guid id, CancellationToken ct)
    {
        var deleted = await service.Delete(id, ct);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
