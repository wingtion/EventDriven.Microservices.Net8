using MassTransit;
using Microsoft.EntityFrameworkCore; 
using Shared.Events;
using Stock.API.Models; 

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly ILogger<OrderCreatedEventConsumer> _logger;
        private readonly StockDbContext _context; 

        public OrderCreatedEventConsumer(ILogger<OrderCreatedEventConsumer> logger, StockDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation($"[Stock Service] Order Received! OrderId: {message.OrderId}");

            foreach (var item in message.OrderItems)
            {
                // 1. find the product in the database
                var stock = await _context.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                if (stock != null)
                {
                    // 2. Checking if the stock is enough
                    if (stock.Quantity >= item.Count)
                    {
                        stock.Quantity -= item.Count; 
                        _logger.LogInformation($"-- ProductId: {item.ProductId} updated. New Stock: {stock.Quantity}");
                    }
                    else
                    {
                        _logger.LogWarning($"-- Not enough stock for ProductId: {item.ProductId}!");
                    }
                }
                else
                {
                    _logger.LogWarning($"-- ProductId: {item.ProductId} not found in database!");
                }
            }

            // 3. save the changes
            await _context.SaveChangesAsync();
            _logger.LogInformation("[Stock Service] Database updated successfully.");
        }
    }
}