using Hotel.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class RoleBasedAuthorizeAttribute : TypeFilterAttribute
{
    public RoleBasedAuthorizeAttribute(string requiredRole) : base(typeof(RoleBasedAuthorizationFilter))
    {
        Arguments = new object[] { requiredRole };
    }
}
