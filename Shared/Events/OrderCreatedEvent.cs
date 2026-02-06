namespace Shared.Events
{
    // This event will be published when a new order is placed.
    public class OrderCreatedEvent
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalPrice { get; set; }

        // List of order items (Product Id and Quantity)
        public List<OrderItemMessage> OrderItems { get; set; } = new();
    }

    public class OrderItemMessage
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
    }
}