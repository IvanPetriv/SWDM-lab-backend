using Api.Dtos;
using Application.Services;
using Application.Services.Users;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    public async Task<ActionResult<UserGetDto>> GetCurrentUser(CancellationToken ct) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var user = await service.GetAsync(userId, ct);
        if (user == null)
            return NotFound();

        var dto = mapper.Map<UserGetDto>(user);
        return Ok(dto);
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserGetDto>> UpdateCurrentUser([FromBody] UserGetDto userDto, CancellationToken ct) {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var userToUpdate = mapper.Map<User>(userDto);
        var updatedUser = await service.UpdateAsync(userId, userToUpdate, ct);
        if (updatedUser == null)
            return NotFound();

        return Ok(mapper.Map<UserGetDto>(updatedUser));
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<UserSearchDto>>> SearchUsers([FromQuery] string username, CancellationToken ct) {
        var allUsers = await service.GetByUsernameAsync(username, ct);

        var dtos = mapper.Map<IEnumerable<UserSearchDto>>(allUsers);
        return Ok(dtos);
    }

    [HttpGet("search/students")]
    public async Task<ActionResult<IEnumerable<UserSearchDto>>> SearchStudents([FromQuery] string username, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<UserGetDto>> CreateUser([FromBody] UserGetDto userDto, CancellationToken ct) {
        var user = mapper.Map<User>(userDto);
        var createdUser = await service.CreateAsync(user, ct);
        var dto = mapper.Map<UserGetDto>(createdUser);

        return CreatedAtAction(nameof(GetCurrentUser), new { id = dto.Id }, dto);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> DeleteUser(Guid id, CancellationToken ct) {
        var success = await service.DeleteAsync(id, ct);
        if (!success)
            return NotFound();
        return NoContent();
    }
}
