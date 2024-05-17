using Artblog.API.Data;
using Artblog.API.Models.Domain;
using Artblog.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Artblog.API.Repositories.Implementation
{
    public class ImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ApplicationDbContext dbContext;

        public ImageRepository(
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext dbContext
        )
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<BlogImage>> GetAll()
        {
            return await dbContext.BlogImages.ToListAsync();
        }

        public async Task<BlogImage> Upload(IFormFile file, BlogImage blogImage)
        {
            // Upload the Image to API/Image
            var localPath = Path.Combine(
                webHostEnvironment.ContentRootPath,
                "Images",
                $"{blogImage.FileName}{blogImage.FileExtension}"
            );

            using var stream = new FileStream(localPath, FileMode.Create);
            await file.CopyToAsync(stream);

            // Update the Database
            //https://artblog.com/images/somefilename.jpg
            var httpRequestImage = httpContextAccessor.HttpContext.Request;
            var urlPath =
                $"{httpRequestImage.HttpContext.Request.Scheme}://{httpRequestImage.Host}{httpRequestImage.PathBase}/Images/{blogImage.FileName}{blogImage.FileExtension}";

            blogImage.Url = urlPath;

            await dbContext.BlogImages.AddAsync(blogImage);
            await dbContext.SaveChangesAsync();

            return blogImage;
        }
    }
}
