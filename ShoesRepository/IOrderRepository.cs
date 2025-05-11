using DataAcess.Entity.OrderAggregate;
using ShoesShared.ModelDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoesRepository
{
    public interface IOrderRepository
    {
        Task<OrderResponseDto> CreateOrderAsync(string buyerEmail, int deliveryMethod, string basketId,
            Address shippingAddress);
        Task<IReadOnlyList<Order>> GetOrderForUserAsync(string buyerEmail);
        Task<Order> GetOrderByIdAsync(int id, string buyerEmail);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
        Task<Order> GetOrderByPaymentIntentId(string paymentIntentId);
        Task<Order> UpdateOrder(Order order);

    }
}
