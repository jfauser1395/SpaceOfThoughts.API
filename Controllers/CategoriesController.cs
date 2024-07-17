using Artblog.API.Data;
using Artblog.API.Models.Domain;
using Artblog.API.Models.DTOs;
using Artblog.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Artblog.API.Controllers
{ // https://localhost:xxxx/api/categories
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequestDto request)
        {
            // Map DTO to Domain Model
            var category = new Category { Name = request.Name, UrlHandle = request.UrlHandle };

            await categoryRepository.CreateAsync(category);

            // Domain model to DTO
            var response = new CategoryDto
            {
                Id = category.Id,
                Name = request.Name,
                UrlHandle = request.UrlHandle
            };

            return Ok(response);
        }

        // GET: https://localhost:7000/api/Categories?query=example&sortBy=example1&sortDirection=desc
        [HttpGet]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> GetAllCategories(
            [FromQuery] string? query,
            [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize
        )
        {
            var categories = await categoryRepository.GetAllAsync(
                query,
                sortBy,
                sortDirection,
                pageNumber,
                pageSize
            );

            // Map Domain modle to DTO
            var response = new List<CategoryDto>();
            foreach (var category in categories)
            {
                response.Add(
                    new CategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                        UrlHandle = category.UrlHandle
                    }
                );
            }

            return Ok(response);
        }

        // GET: https://localhost:7000/api/Categories/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> GetCategoryById([FromRoute] Guid id)
        {
            var existingcategory = await categoryRepository.GetById(id);

            if (existingcategory is null)
            {
                return NotFound();
            }
            var response = new CategoryDto
            {
                Id = existingcategory.Id,
                Name = existingcategory.Name,
                UrlHandle = existingcategory.UrlHandle
            };

            return Ok(response);
        }

        // GET: https://localhost:7000/api/Categories/count
        [HttpGet]
        [Route("count")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> GetCategoriesTatal()
        {
            var count = await categoryRepository.GetCount();

            return Ok(count);
        }

        // PUT: https://localhost:7000/api/categories/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> EditCategory(
            [FromRoute] Guid id,
            UpdateCategoryRequestDto request
        )
        {
            // Convert DTO to Domain model
            var category = new Category
            {
                Id = id,
                Name = request.Name,
                UrlHandle = request.UrlHandle
            };

            category = await categoryRepository.UpdateAsync(category);

            if (category is null)
            {
                return NotFound();
            }

            //Convert Domain model to DTO
            var response = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };

            return Ok(response);
        }

        // Delete: https://localhost:7000/api/categories/{id}
        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            var category = await categoryRepository.DeleteAsync(id);

            if (category is null)
            {
                return NotFound();
            }

            // Convert Domain model to DTO
            var response = new Category
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };

            return Ok(response);
        }
    }
}
