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
        // Pobierz UserId z sesji
        var userIdString = context.HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            context.Result = new RedirectToActionResult("", "Login", null);
            return;
        }

        // Pobierz użytkownika z bazy danych
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user == null || user.Role != _requiredRole)
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            return;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
