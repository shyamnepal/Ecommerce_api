using DataAcess.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAcess.Entity;
using ShoesRepository.GenreicRepo;
using ShoesShared.ShopesResponse;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ecommerce_shoes.Attribute;

namespace Ecommerce_shoes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[ApiKeyAuthorizationFilter("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoryController : ControllerBase
    {
        private readonly IGenericRepository<Category> _category;
        private readonly IConfiguration _configuration;
        public CategoryController(IGenericRepository<Category> category, IConfiguration configuration)
        {
            _category = category;
            _configuration = configuration; 
        }
        [HttpPost("AddCategory")]
        public IActionResult AddCaegory(CategoryViewModel model) {
            var response = new ShoesResponse();
            if (ModelState.IsValid)
            {
                //for integers
                Random r = new Random();
                int randomnum = r.Next(0, 100000000);
                var productCategory = new DataAcess.Entity.Category()
                {
                    CategoryName = model.CategoryName,
                    CategoryId = randomnum * 2,
                    Description = model.Description

                };
                //Add product
                

                _category.Add(productCategory);
                response.Data = productCategory;
                response.Status = _configuration["Response:SuccessStatus"];
                response.ErrorCode = int.Parse(_configuration["Response:SuccessCode"]);
                response.Message = "Successfully Add Category";
                return Ok(response);
            }
             response.Status= _configuration["Response:FailedStatus"];
            response.Message = "Failed to add Category";
            response.ErrorCode = int.Parse(_configuration["Response:ErrorCode"]);
            return BadRequest(response);
           


        }
        [HttpPut("EditCategory")]
        public IActionResult EditCategory(CategoryViewModel model)
        {
            var response = new ShoesResponse();
            Category category = _category.GetById(model.CategoryId);
            
            if (category != null)
            {
                category.CategoryName= model.CategoryName;
                category.Description= model.Description;
                _category.Update(category);
                response.Data = category;
                response.Status = _configuration["Response:SuccessStatus"];
                response.ErrorCode = int.Parse(_configuration["Response:SuccessCode"]);
                response.Message = "Successfully Add Category";
                return Ok(response);
               
            }
            response.Status = _configuration["Response:FailedStatus"];
            response.Message = "Failed to Update Category";
            response.ErrorCode = int.Parse(_configuration["Response:ErrorCode"]);
            return BadRequest(response);

        }

        [HttpDelete("DeleteCategory")]
        public IActionResult DeleteCategory(int id)
        {
            var response = new ShoesResponse();

            if (id != null)
            {
                Category category= _category.GetById(id);
                if(category != null)
                {
                    _category.Remove(category);
                    response.Status = _configuration["Response:SuccessStatus"];
                    response.ErrorCode = int.Parse(_configuration["Response:SuccessCode"]);
                    response.Message = "Category deleted";
                    return Ok(response);    
                }
            }
            response.Status = _configuration["Response:FailedStatus"];
            response.Message = "Failed to Delete Category";
            response.ErrorCode = int.Parse(_configuration["Response:ErrorCode"]);
            return BadRequest(response);
        }

        [HttpGet("AllCategory")]
        public IActionResult GetCategory()
        {
            return Ok(_category.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult GetCategoryById(int id)
        {
            var response = new ShoesResponse();
            var category = _category.GetById(id); // or your logic to fetch it

            if (category == null)
            {
                response.Status = _configuration["Response:FailedStatus"];
                response.Message = "Category not found.";
                response.ErrorCode = int.Parse(_configuration["Response:ErrorCode"]);
                return BadRequest(response);
            }
            response.Status = _configuration["Response:SuccessStatus"];
            response.ErrorCode = int.Parse(_configuration["Response:SuccessCode"]);
            response.Data = category;
            response.Message = "Successfully get the category";
            return Ok(response);
            
        }
    }
}
