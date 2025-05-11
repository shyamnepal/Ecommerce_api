using DataAcess.Entity;
using DataAcess.Entity.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using ShoesRepository.GenreicRepo;
using ShoesShared.ModelDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShoesRepository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IGenericRepository<Product> _productRepo;
        private readonly IGenericRepository<DeliveryMethod> _deliveryRepo;
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<Address> _addressRepo;
        private readonly IPaymentRepository _paymentService;

        public OrderRepository(
            IBasketRepository basketRepo, 
            IGenericRepository<Product> ProductRepo, 
            IGenericRepository<DeliveryMethod> deliveryRepo, 
            IGenericRepository<Order> orderRepo, 
            IPaymentRepository paymentService,
            IGenericRepository<Address> addressRepo
            )
        {
            _basketRepo = basketRepo;
            _productRepo = ProductRepo;
            _deliveryRepo = deliveryRepo;
            _orderRepo = orderRepo;
            _paymentService = paymentService;
            _addressRepo = addressRepo;
           

        }

        public async Task<OrderResponseDto> CreateOrderAsync(
            string buyerEmail,
            int deliveryMethodId,
            string basketId,
            Address shippingAddress)
        {
            var orderResponseDto = new OrderResponseDto();
            // 1. Load basket
            var basket = await _basketRepo.GetBasketAsync(basketId)
                ?? throw new ArgumentException($"Basket '{basketId}' not found.");

            // 2. Map to OrderItems
            var items = new List<OrderItem>();
            foreach (var bi in basket.Items)
            {
                var p = await _productRepo.GetByIdAsync(bi.BasketItemId)
                    ?? throw new KeyNotFoundException($"Product {bi.BasketItemId} not found.");
                items.Add(new OrderItem(
                    new ProductItemOrdered(p.ProductId, p.ProductName, p.ProductImages),
                    p.Price,
                    bi.Quantity));
            }

            // 3. Lookup delivery
            var deliveryMethod = await _deliveryRepo.GetByIdAsync(deliveryMethodId)
                ?? throw new KeyNotFoundException($"Delivery method {deliveryMethodId} not found.");

            // 4. Calculate subtotal
            var subtotal = items.Sum(x => x.Price * x.Quantity);

            // ✅ Ensure the address is added to the database
            shippingAddress.AddressId = 0; // ✅ Reset ID to trigger auto-generation
            await _addressRepo.Add(shippingAddress);


            // ✅ Step: create payment intent via your payment service
            var paymentIntentDto = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            // Create the order with the intent ID
            var order = new Order(items, "test@gmail.com", shippingAddress, deliveryMethod, subtotal)
            {
                PaymentIntentId = paymentIntentDto.Id,
                Status = OrderStatus.Pending
            };

            await _orderRepo.Add(order);
            orderResponseDto.OrderId = order.OrderId;
            orderResponseDto.ClientSecret = paymentIntentDto.ClientSecret;
            orderResponseDto.PaymentIntentId = order.PaymentIntentId;
            return orderResponseDto;
        }

        public Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            throw new NotImplementedException();
        }

        public async Task<Order> GetOrderByPaymentIntentId(string paymentIntentId)
        {
            // Fix CS1002: Add a semicolon at the end of the statement.
            // Fix CS4016: Await the asynchronous method to ensure the return type matches 'Order'.
            return await _orderRepo.GetByIdAsync(paymentIntentId, o => o.DeliveryMethod, o => o.OrderItems);
        }

        public Task<IReadOnlyList<Order>> GetOrderForUserAsync(string buyerEmail)
        {
            throw new NotImplementedException();
        }

        public async Task<Order> UpdateOrder(Order order)
        {
            _orderRepo.Update(order); // Update method in IGenericRepository is void, so it cannot be awaited.
            return await Task.FromResult(order); // Wrap the order in a Task to match the return type.
        }
    }
}
