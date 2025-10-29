using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Api.Extensions;

public static class ControllerExtensions
{
    public static Guid? GetCurrentUserId(this ControllerBase controller)
    {
        var userIdClaim = controller.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            return null;

        return userId;
    }

    public static string? GetCurrentUserRole(this ControllerBase controller)
    {
        return controller.User.FindFirst(ClaimTypes.Role)?.Value;
    }

    public static (Guid? userId, string? role) GetCurrentUserInfo(this ControllerBase controller)
    {
        var userId = controller.GetCurrentUserId();
        var role = controller.GetCurrentUserRole();
        return (userId, role);
    }
}