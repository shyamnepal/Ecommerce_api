using DataAcess.Entity.OrderAggregate;

namespace Ecommerce_shoes.Dtos
{
    public class OrderDto
    {
        public string BasketId { get; set; }
        public int DeliverMethodId { get; set; }
        public Address ShipToAddress{ get; set; }
    }
}
