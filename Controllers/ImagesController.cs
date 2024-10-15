﻿using Artblog.API.Models.Domain;
using Artblog.API.Models.DTOs;
using Artblog.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Artblog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        // GET: {apiBaseUrl}/api/Images?sortBy=example1&sortDirection=desc
        [HttpGet]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> GetAllImages(
            [FromQuery] string? sortBy,
            [FromQuery] string? sortDirection
        )
        {
            // Call image repository to get all images
            var images = await imageRepository.GetAll(
                sortBy,
                sortDirection
            );

            // Convert Domain model to DTO
            var response = new List<BlogImageDto>();
            foreach (var image in images)
            {
                response.Add(
                    new BlogImageDto
                    {
                        Id = image.Id,
                        Title = image.Title,
                        DateCreated = image.DateCreated,
                        FileExtension = image.FileExtension,
                        FileName = image.FileName,
                        Url = image.Url
                    }
                );
            }

            return Ok(response);
        }

        // Post: {apiBaseUrl}/api/images
        [HttpPost]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> UploadImage(
            IFormFile file,
            [FromForm] string fileName,
            [FromForm] string title
        )
        {
            // Call ValidateFileUpdate method to validate the file
            ValidateFileUpdate(file);

            // Check if the ModelState is valid
            if (ModelState.IsValid)
            {
                // Create a new BlogImage object
                var blogImage = new BlogImage
                {
                    FileExtension = Path.GetExtension(file.FileName).ToLower(),
                    FileName = fileName,
                    Title = title,
                    DateCreated = DateTime.Now,
                };

                // Upload the file and get the updated BlogImage object
                blogImage = await imageRepository.Upload(file, blogImage);

                // Convert Domain Model to DTO
                var response = new BlogImageDto
                {
                    Id = blogImage.Id,
                    Title = blogImage.Title,
                    DateCreated = blogImage.DateCreated,
                    FileExtension = blogImage.FileExtension,
                    FileName = blogImage.FileName,
                    Url = blogImage.Url
                };

                return Ok(response);
            }

            return BadRequest(ModelState);
        }

        private void ValidateFileUpdate(IFormFile file)
        {
            var allowedExtension = new string[] { ".jpg", ".jpeg", ".png" };

            if (!allowedExtension.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                ModelState.AddModelError("file", "Unsupported file format");
            }

            if (file.Length > 10485760)
            {
                ModelState.AddModelError("file", "File size cannot be more than 10MB");
            }
        }


        // Delete:{apiBaseUrl}/api/categories/{id}
        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id)
        {
            var deletedImage = await imageRepository.DeleteAsync(id);

            if (deletedImage is null)
            {
                return NotFound();
            }

            // Convert Domain model to DTO
            var response = new BlogImageDto
            {
                Id = deletedImage.Id,
                Title = deletedImage.Title,
                DateCreated = deletedImage.DateCreated,
                FileExtension = deletedImage.FileExtension,
                FileName = deletedImage.FileName,
                Url = deletedImage.Url
            };

            return Ok(response);
        }
    }
}
