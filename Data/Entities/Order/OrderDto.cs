using System.Collections.Generic;

namespace Data.Entities.Order
{
    public class OrderDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public List<OrderItemDto> Items { get; set; }
        public decimal TotalPrice { get; set; }
        public string Address { get; set; }
    }
}