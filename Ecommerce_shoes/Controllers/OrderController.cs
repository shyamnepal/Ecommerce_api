using AutoMapper;
using DataAcess.Entity;
using DataAcess.Entity.OrderAggregate;
using Ecommerce_shoes.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoesRepository;
using ShoesShared.ShopesResponse;
using System.Security.Claims;

namespace Ecommerce_shoes.Controllers
{
    //[Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        public OrderController(IOrderRepository orderRepository, IMapper mapper, IConfiguration config)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _config = config;   

        }
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder(OrderDto orderDto)
        {
            var response = new ShoesResponse();
            var email = ClaimsPrincipalExtensions.RetrieveEmailFromPrincipal(User);
            var address = _mapper.Map<Address>(orderDto.ShipToAddress);
            var order = await _orderRepository.CreateOrderAsync(email, orderDto.DeliverMethodId, orderDto.BasketId, address);
           //failed status 
            response.Message="Problem creating order";
            response.Status = _config["Response:FailedStatus"];
            response.ErrorCode = int.Parse(_config["Response:ErrorCode"]);
            if (order == null) return BadRequest(response);
            //response ordder
            response.Data = order;
            response.Status = _config["Response:SuccessStatus"];
            response.ErrorCode = int.Parse(_config["Response:SuccessCode"]);
            response.Message = "Successfully Order the prodcut";
            return Ok(order);
        }
    }
}
