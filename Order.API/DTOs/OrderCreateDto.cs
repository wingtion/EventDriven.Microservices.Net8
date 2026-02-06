namespace Order.API.DTOs
{
    // Data Transfer Object for creating a new order.
    // This is what the client (Frontend/Swagger) sends to us.
    public class OrderCreateDto
    {
        public int CustomerId { get; set; }
        public decimal TotalPrice { get; set; }

        // List of products in the order
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
    }
}