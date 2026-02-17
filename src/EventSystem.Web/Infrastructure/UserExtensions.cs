using System.Security.Claims;

namespace EventSystem.Web.Infrastructure;

public static class UserExtensions
{
    public static string? GetUserId(this ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.NameIdentifier);
}
