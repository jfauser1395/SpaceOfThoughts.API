using Artblog.API.Models.Domain;

namespace Artblog.API.Repositories.Interface
{
    public interface IImageRepository
    {
        Task<BlogImage> Upload(IFormFile file, BlogImage blogImage);

        Task<IEnumerable<BlogImage>> GetAll(
            string? sortBy,
            string? sortDirection
            );

        Task<BlogImage?> DeleteAsync(Guid id);
    }
}
