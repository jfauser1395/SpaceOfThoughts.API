using Artblog.API.Data;
using Artblog.API.Models.Domain;
using Artblog.API.Repositories.Interface;

namespace Artblog.API.Repositories.Implementation
{
    public class BlogPOstRepository : IBlogPostRepository
    {
        private readonly ApplicationDbContext dbContext;
        public BlogPOstRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<BlogPost> CreateAsync(BlogPost blogPost)
        {
           await dbContext.BlogPosts.AddAsync(blogPost);
           await dbContext.SaveChangesAsync();
           return blogPost;
        }
    }
}
