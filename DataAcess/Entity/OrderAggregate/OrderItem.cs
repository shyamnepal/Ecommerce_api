using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Entity.OrderAggregate
{
    public class OrderItem
    {
        public OrderItem()
        {
            
        }
        public OrderItem(int id,ProductItemOrdered itemOrderd, decimal? price, int quantity)
        {
            OrderItemId = id;
            ItemOrderd = itemOrderd;
            Price = price;
            Quantity = quantity;
        }
        public int OrderItemId { get; set; }

        public ProductItemOrdered ItemOrderd { get; set; }
        public decimal? Price { get; set; }
        public int Quantity { get; set; }
    }
}
