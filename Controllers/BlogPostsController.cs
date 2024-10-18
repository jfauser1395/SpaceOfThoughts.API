using Artblog.API.Models.Domain;
using Artblog.API.Models.DTOs;
using Artblog.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Artblog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly ICategoryRepository categoryRepository;

        public BlogPostsController(
            IBlogPostRepository blogPostRepository,
            ICategoryRepository categoryRepository
        )
        {
            this.blogPostRepository = blogPostRepository;
            this.categoryRepository = categoryRepository;
        }

        // POST: {apiBaseUrl}/api/blogposts
        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateBlogPost([FromBody] CreateBlogPostRequestDto request)
        {
            //Convert DTO to Domain
            var blogPost = new BlogPost
            {
                Author = request.Author,
                Content = request.Content,
                FeaturedImageUrl = request.FeaturedImageUrl,
                IsVisible = request.IsVisible,
                PublishedDate = request.PublishedDate,
                ShortDescription = request.ShortDescription,
                Title = request.Title,
                UrlHandle = request.UrlHandle,
                Categories = new List<Category>()
            };

            foreach (var categoryGuid in request.Categories)
            {
                var existingCategory = await categoryRepository.GetById(categoryGuid);
                if (existingCategory != null)
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }

            blogPost = await blogPostRepository.CreateAsync(blogPost);

            // Convert Domain Model back to DTO
            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Author = blogPost.Author,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                IsVisible = blogPost.IsVisible,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                Title = blogPost.Title,
                UrlHandle = blogPost.UrlHandle,
                Categories = blogPost
                    .Categories.Select(x => new CategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        UrlHandle = x.UrlHandle
                    })
                    .ToList()
            };

            return Ok(response);
        }

        // GET: {apiBaseUrl}/api/blogposts?query=example@sortBy=title@sortDirection=desc
        [HttpGet]
        public async Task<IActionResult> GetAllBlogPosts(
            [FromQuery] string? query,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize
        )
        {
            var blogPosts = await blogPostRepository.GetAllAsync(
                query,
                sortBy,
                sortDirection,
                pageNumber,
                pageSize
            );

            // Convert Domain model to DTO
            var response = new List<BlogPostDto>();
            foreach (var blogPost in blogPosts)
            {
                response.Add(
                    new BlogPostDto
                    {
                        Id = blogPost.Id,
                        Author = blogPost.Author,
                        Content = blogPost.Content,
                        FeaturedImageUrl = blogPost.FeaturedImageUrl,
                        IsVisible = blogPost.IsVisible,
                        PublishedDate = blogPost.PublishedDate,
                        ShortDescription = blogPost.ShortDescription,
                        Title = blogPost.Title,
                        UrlHandle = blogPost.UrlHandle,
                        Categories = blogPost
                            .Categories.Select(x => new CategoryDto
                            {
                                Id = x.Id,
                                Name = x.Name,
                                UrlHandle = x.UrlHandle
                            })
                            .ToList()
                    }
                );
            }

            return Ok(response);
        }

        // GET: {apiBaseUrl}/api/blogposts/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetBlogPostById([FromRoute] Guid id)
        {
            // Get the BlogPost from Repository
            var blogPost = await blogPostRepository.GetByIdAsync(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            // Convert Domain Model to DTO
            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Author = blogPost.Author,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                IsVisible = blogPost.IsVisible,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                Title = blogPost.Title,
                UrlHandle = blogPost.UrlHandle,
                Categories = blogPost
                    .Categories.Select(x => new CategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        UrlHandle = x.UrlHandle
                    })
                    .ToList()
            };

            return Ok(response);
        }

        // GET: {apiBaseUrl}/api/blogposts/count
        [HttpGet]
        [Route("count")]
        public async Task<IActionResult> GetBlogPostTotal()
        {
            var count = await blogPostRepository.GetCount();

            return Ok(count);
        }

        // GET: {apiBaseUrl}/api/blogPosts/{urlHandle}
        [HttpGet]
        [Route("{urlHandle}")]
        public async Task<IActionResult> GetBlogPostByUrl([FromRoute] string urlHandle)
        {
            // Get Blog Post details from repository
            var blogPost = await blogPostRepository.GetByUrlHandleAsync(urlHandle);

            if (blogPost == null)
            {
                return NotFound();
            }

            // Convert Domain Model to DTO
            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Author = blogPost.Author,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                IsVisible = blogPost.IsVisible,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                Title = blogPost.Title,
                UrlHandle = blogPost.UrlHandle,
                Categories = blogPost
                    .Categories.Select(x => new CategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        UrlHandle = x.UrlHandle
                    })
                    .ToList()
            };

            return Ok(response);
        }

        // PUT: {apiBaseUrl}/api/blogposts/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> UpdateBlogPostById(
            [FromRoute] Guid id,
            UpdateBlogpostRequestDto request
        )
        {
            // Convert DTO to Domain Model
            var blogPost = new BlogPost
            {
                Id = id,
                Author = request.Author,
                Content = request.Content,
                FeaturedImageUrl = request.FeaturedImageUrl,
                IsVisible = request.IsVisible,
                PublishedDate = request.PublishedDate,
                ShortDescription = request.ShortDescription,
                Title = request.Title,
                UrlHandle = request.UrlHandle,
                Categories = new List<Category>()
            };

            foreach (var categoryGuid in request.Categories)
            {
                var existingCategory = await categoryRepository.GetById(categoryGuid);

                if (existingCategory != null)
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }

            // Call repository to update blogpost Domain Model
            var updatedBlogPost = await blogPostRepository.UpdateAsync(blogPost);

            if (updatedBlogPost == null)
            {
                return NotFound();
            }

            // Convert Domain Model back to DTO
            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Author = blogPost.Author,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                IsVisible = blogPost.IsVisible,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                Title = blogPost.Title,
                UrlHandle = blogPost.UrlHandle,
                Categories = blogPost
                    .Categories.Select(x => new CategoryDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        UrlHandle = x.UrlHandle
                    })
                    .ToList()
            };

            return Ok(response);
        }

        // Delete:{apiBaseUrl}/api/blogposts/{id}
        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteBlogPost([FromRoute] Guid id)
        {
            var deletedBlogPost = await blogPostRepository.DeleteAsync(id);

            if (deletedBlogPost == null)
            {
                return NotFound();
            }

            // Convert Domain Model to DTO
            var response = new BlogPostDto
            {
                Id = deletedBlogPost.Id,
                Author = deletedBlogPost.Author,
                Content = deletedBlogPost.Content,
                FeaturedImageUrl = deletedBlogPost.FeaturedImageUrl,
                IsVisible = deletedBlogPost.IsVisible,
                PublishedDate = deletedBlogPost.PublishedDate,
                ShortDescription = deletedBlogPost.ShortDescription,
                Title = deletedBlogPost.Title,
                UrlHandle = deletedBlogPost.UrlHandle,
                Categories = deletedBlogPost
                    .Categories.Select(category => new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                        UrlHandle = category.UrlHandle
                    })
                    .ToList()
            };

            return Ok(response);
        }
    }
}
