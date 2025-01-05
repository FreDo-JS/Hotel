using Hotel.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class RoleBasedAuthorizationFilter : IActionFilter
{
    private readonly string _requiredRole;
    private readonly HotelDbContext _context;

    public RoleBasedAuthorizationFilter(string requiredRole, HotelDbContext context)
    {
        _requiredRole = requiredRole;
        _context = context;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        
        var userIdString = context.HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            context.Result = new RedirectToActionResult("", "Login", null);
            return;
        }

        
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null || !RoleHierarchy.HasAccess(user.Role, _requiredRole))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            return;
        }

        
        context.HttpContext.Session.SetString("UserRole", user.Role);
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}

public static class RoleHierarchy
{
    private static readonly Dictionary<string, int> RoleLevels = new()
    {
        { "rezydent", 1 },
        { "pracownik", 2 },
        { "administrator", 3 }
    };

    public static bool HasAccess(string userRole, string requiredRole)
    {
        if (!RoleLevels.ContainsKey(userRole) || !RoleLevels.ContainsKey(requiredRole))
        {
            return false;
        }

        return RoleLevels[userRole] >= RoleLevels[requiredRole];
    }
}
