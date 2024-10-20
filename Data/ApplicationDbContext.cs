using Microsoft.EntityFrameworkCore;
using SpaceOfThoughts.API.Models.Domain;

namespace SpaceOfThoughts.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<BlogPost> BlogPosts { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<BlogImage> BlogImages { get; set; }
    }
}
