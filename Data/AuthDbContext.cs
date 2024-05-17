using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Artblog.API.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var readerRoleId = "0839b6ac-c835-4402-b477-dff84f98f9d1";
            var writerRoleId = "775bed88-eb72-4a1f-93ee-bdf869707bdc";

            // Create Reader and Writer Role
            var role = new List<IdentityRole>
            {
                new IdentityRole()
                {
                    Id = readerRoleId,
                    Name = "Reader",
                    NormalizedName = "Reader".ToUpper(),
                    ConcurrencyStamp = readerRoleId
                },
                new IdentityRole()
                {
                    Id = writerRoleId,
                    Name = "Writer",
                    NormalizedName = "Writer".ToUpper(),
                    ConcurrencyStamp = writerRoleId
                },
            };

            // Seed the roles
            builder.Entity<IdentityRole>().HasData(role);

            // Create an Admin User
            var adminUserId = "6542a5f0-8e4f-44df-a7e8-afbee3ad97cf";
            var admin = new IdentityUser()
            {
                Id = adminUserId,
                UserName = "admin@artblog.com",
                Email = "admin@artblog.com",
                NormalizedEmail = "admin@artblog.com".ToUpper(),
                NormalizedUserName = "admin@artblog.com".ToUpper()
            };

            // Generate Password
            admin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(
                admin,
                "Admin@123"
            );

            builder.Entity<IdentityUser>().HasData(admin);

            // Give Roles To Admin
            var adminRoles = new List<IdentityUserRole<string>>()
            {
                new() { UserId = adminUserId, RoleId = readerRoleId },
                new() { UserId = adminUserId, RoleId = writerRoleId }
            };

            builder.Entity<IdentityUserRole<string>>().HasData(adminRoles);
        }
    }
}
