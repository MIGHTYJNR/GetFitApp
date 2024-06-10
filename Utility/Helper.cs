using GetFitApp.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace GetFitApp.Utility;

public class Helper
{
    public static async Task<(string userId, string userName)> GetCurrentUserIdAsync(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext!.User?.Identity?.IsAuthenticated == true)
        {
            var user = await userManager.GetUserAsync(httpContext.User);
            return (user!.Id, user.UserName!);
        }

        return (string.Empty, string.Empty); // Return empty strings if not authenticated
    }
}
