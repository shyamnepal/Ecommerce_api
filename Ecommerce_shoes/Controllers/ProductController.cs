using DataAcess.Entity;
using DataAcess.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShoesRepository.GenreicRepo;
using ShoesShared.ShopesResponse;
using System.Runtime.InteropServices;

namespace Ecommerce_shoes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IGenericRepository<Product> _genericRepo;
        private readonly IConfiguration _configuration;
        public ProductController(IGenericRepository<Product> genricRepo, IConfiguration configuration)
        {
            _genericRepo = genricRepo;
            _configuration = configuration;
        }
        [HttpPost("AddProduct")]
        public IActionResult AddProduct(ProductViewModel model)
        {
            var response = new ShoesResponse();
            if (ModelState.IsValid)
            {
                //for integers
                Random r = new Random();
                int randomnum = r.Next(0, 100000000);
                var product = new Product()
                {
                   
                    CategoryId = model.CategoryId,
                    Description = model.Description,
                    Price = model.Price,
                    ProductId = randomnum*2,
                    ProductName = model.ProductName,
                    StockQuentity = model.StockQuentity


                };
                
                //Add product
                _genericRepo.Add(product);
                //response of product
                response.Data = product;
                response.Status = _configuration["Response:SuccessStatus"];
                response.ErrorCode = int.Parse(_configuration["Response:SuccessCode"]);
                response.Errormessage = "Successfully Add Product";
                
                return Ok(response);              
                
            }
            response.Status = _configuration["Response:FailedStatus"];
            response.Errormessage = "Failed to add product";
            response.ErrorCode = int.Parse(_configuration["Response:ErrorCode"]);
            return BadRequest(response);

        }

        [HttpPut("UpdateProduct")]
        public IActionResult EditProduct(ProductViewModel model)
        {
            var response= new ShoesResponse();
            if (ModelState.IsValid)
            {
                var product = _genericRepo.GetById(model.ProductId);
                if(product != null)
                {
                    product.ProductName=model.ProductName;
                    product.StockQuentity=model.StockQuentity;
                    product.Price = model.Price;
                    product.Description=model.Description;
                   _genericRepo.Update(product);

                    //response
                    response.Data = product;
                    response.Status= _configuration["Response:SuccessStatus"];
                    response.Errormessage = "Successfully update Product";
                    response.ErrorCode = int.Parse(_configuration["Response:SuccessCode"]);
                    return Ok(response);
                }
            }
            response.Status = _configuration["Response:FailedStatus"];
            response.Errormessage = "Failed to Update product";
            response.ErrorCode = int.Parse(_configuration["Response:ErrorCode"]);
            return BadRequest(response);
        }

        [HttpGet("GetProductById")]
        public IActionResult GetProductById(int id)
        {
            var response= new ShoesResponse();
            if (id!=null)
            {
                var product= _genericRepo.GetById(id);
                if (product != null)
                {
                    response.Data = product;
                    response.Status = _configuration["Response:SuccessStatus"];
                    response.Errormessage = "Successfully Get the data By Id";
                    response.ErrorCode = int.Parse(_configuration["Response:SuccessCode"]);
                    return Ok(response);
                }
                response.Status = _configuration["Response:FailedStatus"];
                response.Errormessage = "Product Id not found";
                response.ErrorCode = int.Parse(_configuration["Response:ErrorCode"]);
                return BadRequest(response);
            }
            response.Status = _configuration["Response:FailedStatus"];
            response.Errormessage = "Id is null";
            response.ErrorCode = int.Parse(_configuration["Response:ErrorCode"]);
            return BadRequest(response);
        }

        [HttpDelete("DeleteProduct")]
        public IActionResult DeleteProduct(int id)
        {
            var response= new ShoesResponse();
            if (id!=null)
            {
                var product = _genericRepo.GetById(id);
                if (product != null)
                {
                    _genericRepo.Remove(product);
                    response.Status = _configuration["Response:SuccessStatus"];
                    response.Errormessage = "Successfully Delete Product";
                    response.ErrorCode = int.Parse(_configuration["Response:SuccessCode"]);
                    return Ok(response);
                }
                response.Status = _configuration["Response:FailedStatus"];
                response.Errormessage = "Failed to delete product";
                response.ErrorCode = int.Parse(_configuration["Response:ErrorCode"]);
                return BadRequest(response);

            }
            response.Status = _configuration["Response:FailedStatus"];
            response.Errormessage = "Id is null";
            response.ErrorCode = int.Parse(_configuration["Response:ErrorCode"]);
            return BadRequest(response);

        }
        [HttpGet("AllProduct")]
        public IActionResult GetAllProduct()
        {
            return Ok( _genericRepo.GetAll());
        }

    }
}
