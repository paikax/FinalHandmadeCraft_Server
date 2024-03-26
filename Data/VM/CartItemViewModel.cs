using System;

namespace Data.VM
{
    public class CartItemViewModel
    {
        public string ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public DateTime AddedAt { get; set; }
    }
}