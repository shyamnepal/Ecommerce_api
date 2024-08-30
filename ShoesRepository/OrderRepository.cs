using DataAcess.Entity;
using DataAcess.Entity.OrderAggregate;
using ShoesRepository.GenreicRepo;
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

        public OrderRepository(IBasketRepository basketRepo, IGenericRepository<Product> ProductRepo, IGenericRepository<DeliveryMethod> deliveryRepo)
        {
            _basketRepo = basketRepo;
            _productRepo = ProductRepo;
            _deliveryRepo = deliveryRepo;
        }
        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
        {
            //get basket from the repo 
            var basket = await _basketRepo.GetBasketAsync(basketId);

            //get item from the product repo
            var items = new List<OrderItem>();
            
            foreach(var item in basket.Items)
            {
                var productItem = _productRepo.GetById(id: item.BasketItemId);
                var itemOrder = new ProductItemOrdered(productItem.ProductId, productItem.ProductName, productItem.ProductImages);
                var orderItem = new OrderItem(item.BasketItemId,itemOrder, productItem.Price, item.Quantity);
                items.Add(orderItem);

                
            }

            //get delivery method for repo 
            var deliveryMethod = _deliveryRepo.GetById(deliveryMethodId);

            //get delivery method from repo
            var subtotal = items.Sum(item => item.Price * item.Quantity);

            //create oreder 
            var order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subtotal);
            return order;
        }

        public Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Order>> GetOrderForUserAsync(string buyerEmail)
        {
            throw new NotImplementedException();
        }
    }
}
