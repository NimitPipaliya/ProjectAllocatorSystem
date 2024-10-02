using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ProjectAllocatorSystemMVC.Implementation;
using ProjectAllocatorSystemMVC.Infrastructure;

namespace ProjectAllocatorSystemMVC.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ManagerAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            HttpContext context1 = context.HttpContext;
            IJwtTokenHandler tokenHandler = new JwtTokenHandler();

            string token = context1.Request.Cookies["jwtToken"];

            if (token != null)
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var userRole = jwtToken.Claims.First(claim => claim.Type == "UserRole").Value;
                if (userRole == "2")
                {
                    return;
                }
                else
                {
                    context1.Response.Cookies.Delete("jwtToken");
                }
            }
            else
            {
                context1.Response.Cookies.Delete("jwtToken");
            }

            // Unauthorized access, redirect or return unauthorized result
            context.Result = new UnauthorizedResult();
        }
    }
}
