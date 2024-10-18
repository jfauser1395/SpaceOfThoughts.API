using Artblog.API.Data;
using Artblog.API.Models.Domain;
using Artblog.API.Repositories.Interface;
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

        public async Task<IEnumerable<BlogImage>> GetAll(
            string? sortBy = null,
            string? sortDirection = null
        )
        {
            // Query
            var blogImages = dbContext.BlogImages.AsQueryable();

            // Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (string.Equals(sortBy, "DateCreated", StringComparison.OrdinalIgnoreCase))
                {
                    var isAsc = string.Equals(
                        sortDirection,
                        "desc",
                        StringComparison.OrdinalIgnoreCase
                    )
                        ? true
                        : false;

                    blogImages = isAsc
                        ? blogImages.OrderBy(x => x.DateCreated)
                        : blogImages.OrderByDescending(x => x.DateCreated);
                }
            }
            return await blogImages.ToListAsync();
        }

        public async Task<BlogImage> Upload(IFormFile file, BlogImage blogImage)
        {
            // Upload the Image to API/Image folder
            var localPath = Path.Combine(
                webHostEnvironment.ContentRootPath,
                "Images",
                $"{blogImage.FileName}{blogImage.FileExtension}"
            );

            using var stream = new FileStream(localPath, FileMode.Create);
            await file.CopyToAsync(stream);

            // Update the Database
            //https://spaceofthoughts.com/images/somefilename.jpg
            var httpRequestImage = httpContextAccessor?.HttpContext?.Request;
            var urlPath =
                $"https://{httpRequestImage?.Host}{httpRequestImage?.PathBase}/Images/{blogImage.FileName}{blogImage.FileExtension}";

            blogImage.Url = urlPath;

            await dbContext.BlogImages.AddAsync(blogImage);
            await dbContext.SaveChangesAsync();

            return blogImage;
        }

        public async Task<BlogImage?> DeleteAsync(Guid id)
        {
            var existingImage = await dbContext.BlogImages.FirstOrDefaultAsync(x => x.Id == id);

            if (existingImage is null)
            {
                return null;
            }

            // Delete the image from the Image folder

            // Get the file path of the image
            var filePath = Path.Combine(
                webHostEnvironment.ContentRootPath,
                "Images",
                $"{existingImage.FileName}{existingImage.FileExtension}"
            );

            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Delete the file
                File.Delete(filePath);
            }

            // Delete the image from the Database
            dbContext.BlogImages.Remove(existingImage);
            await dbContext.SaveChangesAsync();
            return existingImage;
        }
    }
}
