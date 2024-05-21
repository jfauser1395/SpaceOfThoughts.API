using Microsoft.AspNetCore.Identity;

namespace Artblog.API.Repositories.Interface
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
