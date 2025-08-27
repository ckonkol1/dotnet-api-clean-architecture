using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PlantTracker.WebApi.Middleware.Identity
{
    public class RequiresClaimAttribute(string claimName, string claimValue) : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.HasClaim(claimName, claimValue))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}