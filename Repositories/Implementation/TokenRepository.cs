using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SpaceOfThoughts.API.Repositories.Interface;

namespace SpaceOfThoughts.API.Repositories.Implementation
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration configuration;

        public TokenRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string CreateJWTToken(IdentityUser user, List<string> roles)
        {
            // Retrieve JWT configuration values
            var jwtKey =
                configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key is missing");
            var jwtIssuer =
                configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException("JWT Issuer is missing");
            var jwtAudience =
                configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException("JWT Audience is missing");

            // Create Claims
            var claims = new List<Claim>();

            // Add email claim if it's not null or empty
            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            // Add role claims
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // JWT Security Token Parameters
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
            );

            // Return Token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
