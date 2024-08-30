using DataAcess.Entity.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesRepository
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethod, string basketId,
            Address shippingAddress);
        Task<IReadOnlyList<Order>> GetOrderForUserAsync(string buyerEmail);
        Task<Order> GetOrderByIdAsync(int id, string buyerEmail);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
    }
}
