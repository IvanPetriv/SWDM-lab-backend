using Api.Dtos;
using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UserController : ControllerBase
{

    [HttpGet("me")]
    public async Task<ActionResult<UserGetDto>> GetCurrentUser(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpPut("me")]
    public async Task<ActionResult> UpdateCurrentUser([FromBody] UserGetDto userDto, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<UserSearchDto>>> SearchUsers([FromQuery] string username, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpGet("search/students")]
    public async Task<ActionResult<IEnumerable<UserSearchDto>>> SearchStudents([FromQuery] string username, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<UserGetDto>> CreateUser([FromBody] UserGetDto userDto, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> DeleteUser(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
