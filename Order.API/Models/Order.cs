namespace Order.API.Models
{
    // Represents a customer order in the database.
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Order Status: Pending, Completed, Failed
        public string Status { get; set; }

        public List<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }

        public int OrderId { get; set; } // Foreign Key
        public Order Order { get; set; } // Navigation Property
    }
}