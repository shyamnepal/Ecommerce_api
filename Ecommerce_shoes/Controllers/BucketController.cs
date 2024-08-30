using DataAcess.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoesRepository.GenreicRepo;
using ShoesShared.ShopesResponse;
using System.Drawing;
using System.IO;
using System.Reflection.Metadata.Ecma335;

namespace Ecommerce_shoes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BucketController : ControllerBase
    {
        private readonly IGenericRepository<Product> _genericRepo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<ProductImage> _contextImage;
        public BucketController(IWebHostEnvironment webHostEnvironment,IGenericRepository<Product> genericRepository, IConfiguration configuration,IGenericRepository<ProductImage> contextImage)
        {

            _webHostEnvironment = webHostEnvironment;   
            _genericRepo = genericRepository; 
            _configuration = configuration;
            _contextImage = contextImage;

        }
        [HttpPost]
        public async Task<IActionResult> UplaodImage(string ProductName, string Brand, int productId, List<IFormFile> files,string imageAltText)
        {
            var response = new ShoesResponse();

            if (string.IsNullOrWhiteSpace(ProductName) && string.IsNullOrWhiteSpace(Brand))
            {
                response.Status = _configuration["Response:FailedStatus"];
                response.Errormessage = "Product name and brand is not null";
                response.ErrorCode = int.Parse(_configuration["Response:ErrorCode"]);
                return NotFound(response);
            }
            List<ProductImage> image = new List<ProductImage>();
              foreach(var file in files)
            {

                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var extension = Path.GetExtension(file.FileName);
                var newFileName= $"{fileName}_{DateTime.Now.Ticks}{extension}";

                var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Image",ProductName,Brand);
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                var fileNameWithPath = Path.Combine(filePath, newFileName);
                using (var stream =new FileStream (fileNameWithPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var img = new ProductImage
                {
                    ImageUrl = $"/Image/{newFileName}",
                    createdAt = DateTime.Now,
                    ProductId = productId,
                    //ImageId = randomnum,
                    ImageAltText = imageAltText
                   
                   

                };
                image.Add(img);
                
                
            
              }

            if (image.Count < 0)
            {
                response.Status = _configuration["Response:FailedStatus"];
                response.Errormessage = "the list is empty";
                response.ErrorCode = int.Parse(_configuration["Response:ErrorCode"]);
                return Ok(response);
            }

            _contextImage.AddRangeAsync(image);
            response.Status = _configuration["Response:SuccessStatus"];
            response.Errormessage = "successfully upload the image";
            response.ErrorCode = int.Parse(_configuration["Response:SuccessCode"]);
            return Ok(response);

        }
    }
}
