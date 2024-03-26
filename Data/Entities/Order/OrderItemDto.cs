namespace Data.Entities.Order
{
    public class OrderItemDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string TutorialImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        
        public string TutorialId { get; set; }
        public string CreatorEmail { get; set; } 
        
    }
}