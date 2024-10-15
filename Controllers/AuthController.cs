using Artblog.API.Models.DTOs;
using Artblog.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Artblog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly ITokenRepository tokenRepository;

        public AuthController(
            UserManager<IdentityUser> userManager,
            ITokenRepository tokenRepository
        )
        {
            this.userManager = userManager;
            this.tokenRepository = tokenRepository;
        }

        // POST: {apiBaseUrl}/api/auth/login
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var identityUser = await userManager.FindByEmailAsync(request.Email);

            if (identityUser is not null)
            {
                // Check Password
                var checkPasswordResult = await userManager.CheckPasswordAsync(
                    identityUser,
                    request.Password
                );

                if (checkPasswordResult)
                {
                    var roles = await userManager.GetRolesAsync(identityUser);

                    // Create a Token and Response
                    var jwtToken = tokenRepository.CreateJWTToken(identityUser, roles.ToList());

                    var response = new LoginResponseDto
                    {
                        Id = identityUser.Id,
                        UserName = identityUser.UserName,
                        Email = request.Email,
                        Roles = roles.ToList(),
                        Token = jwtToken
                    };

                    return Ok(response);
                }
            }
            ModelState.AddModelError("", "Email or Password is incorrect");

            return ValidationProblem(ModelState);
        }

        // POST: {apiBaseUrl}/api/auth/register
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            // Check if the email entry is formatted correctly
            try
            {
                var addr = new System.Net.Mail.MailAddress(request.Email);
                if (addr.Address != request.Email)
                {
                    ModelState.AddModelError("", "Invalid email format");
                    return ValidationProblem(ModelState);
                }
            }
            catch (FormatException)
            {
                ModelState.AddModelError("emailFormat", "Invalid email format");
                return ValidationProblem(ModelState);
            }

            // Check if the email is already taken
            var existingUserByEmail = await userManager.FindByEmailAsync(request.Email);
            if (existingUserByEmail is not null)
            {
                ModelState.AddModelError("email", "Email is already taken");
                return ValidationProblem(ModelState);
            }

            // Check if the username is already taken
            var existingUserByUsername = await userManager.FindByNameAsync(request.UserName);
            if (existingUserByUsername is not null)
            {
                ModelState.AddModelError("userName", "Username is already taken");
                return ValidationProblem(ModelState);
            }

            // Create IdentityUser object
            var user = new IdentityUser
            {
                UserName = request.UserName?.Trim(),
                Email = request.Email?.Trim()
            };

            // Create user
            var identityResult = await userManager.CreateAsync(user, request.Password);

            if (identityResult.Succeeded)
            {
                // Add role to user (Reader)
                identityResult = await userManager.AddToRoleAsync(user, "Reader");

                if (identityResult.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    int errorIndex = 1;

                    if (identityResult.Errors.Any())
                    {
                        foreach (var error in identityResult.Errors)
                        {
                            ModelState.AddModelError($"{errorIndex}", error.Description);
                            errorIndex++;
                        }
                    }
                }
            }
            else
            {
                int errorIndex = 1;

                if (identityResult.Errors.Any())
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError($"{errorIndex}", error.Description);
                        errorIndex++;
                    }
                }
            }

            return ValidationProblem(ModelState);
        }

        // GET: {apiBaseUrl}/api/auth/users
        [HttpGet]
        [Route("users")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] string? query,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize
        )
        {
            // Filter out admin user by name to not be displayed on the client
            var usersQuery = userManager.Users.AsQueryable().Where(u => u.UserName != "Admin");

            // Query
            if (string.IsNullOrEmpty(query) == false)
            {
                usersQuery = usersQuery.Where(u => u.UserName.Contains(query));
            }

            // Sort
            if (string.IsNullOrEmpty(sortBy) == false)
            {
                if (string.Equals(sortBy, "userName", StringComparison.OrdinalIgnoreCase))
                {
                    var isAsc = string.Equals(
                        sortDirection,
                        "asc",
                        StringComparison.OrdinalIgnoreCase
                    )
                        ? true
                        : false;

                    usersQuery = isAsc
                        ? usersQuery.OrderBy(u => u.UserName)
                        : usersQuery.OrderByDescending(u => u.UserName);
                }
            }

            // Pagination
            // Pag number 1 page size 5- skip 0, take 5 (and so on)
            var skipResults = (pageNumber - 1) * pageSize;
            usersQuery = usersQuery.Skip(skipResults ?? 0).Take(pageSize ?? 100);

            // Convert to DTO
            var users = await usersQuery.ToListAsync();
            var response = new List<UserResponseDto>();
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                var userResponse = new UserResponseDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles
                };
                response.Add(userResponse);
            }

            return Ok(response);
        }

        // GET: {apiBaseUrl}/api/auth/users/{id}
        [HttpGet]
        [Route("users/{id}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> GetUserById([FromRoute] string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            var roles = await userManager.GetRolesAsync(user);
            var response = new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles
            };

            return Ok(response);
        }

        // GET: {apiBaseUrl}/api/auth/count
        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> GetUsersTotal()
        {
            var count = await userManager.Users.CountAsync();

            return Ok(count - 1); // -1 because we do not count the admin
        }

        // Delete: {apiBaseUrl}/api/auth/users/{id}
        [HttpDelete]
        [Route("users/{id}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user is null)
            {
                return NotFound();
            }

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok();
        }
    }
}
