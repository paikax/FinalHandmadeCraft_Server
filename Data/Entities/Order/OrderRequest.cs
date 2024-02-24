using System.Collections.Generic;

namespace Data.Entities.Order
{
    public class OrderRequest
    {
        public string UserId { get; set; }
        
        public List<OrderItem> Items { get; set; }
        
        public decimal TotalPrice { get; set; }
        
        public string Address { get; set; }
    }
}