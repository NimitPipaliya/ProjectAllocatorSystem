using System.IdentityModel.Tokens.Jwt;

namespace ProjectAllocatorSystemMVC.Infrastructure
{
    public interface IJwtTokenHandler
    {
        JwtSecurityToken ReadJwtToken(string token);

    }
}
