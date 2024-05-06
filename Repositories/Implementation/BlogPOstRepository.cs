using Artblog.API.Data;
using Artblog.API.Models.Domain;
using Artblog.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace Artblog.API.Repositories.Implementation
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly ApplicationDbContext dbContext;
        public BlogPostRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<BlogPost> CreateAsync(BlogPost blogPost)
        {
           await dbContext.BlogPosts.AddAsync(blogPost);
           await dbContext.SaveChangesAsync();
           return blogPost;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await dbContext.BlogPosts.ToListAsync();
        }
    }
}
