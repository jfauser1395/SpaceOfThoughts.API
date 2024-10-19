using Microsoft.AspNetCore.Identity;

namespace SpaceOfThoughts.API.Repositories.Interface
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
