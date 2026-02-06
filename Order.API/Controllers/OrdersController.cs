using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Order.API.DTOs;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly OrderDbContext _context;

        public OrdersController(IPublishEndpoint publishEndpoint, OrderDbContext context)
        {
            _publishEndpoint = publishEndpoint;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateDto orderDto)
        {
            // 1. Save Order to Database (Status: Pending)
            // We map the incoming DTO to the database entity.
            var newOrder = new Models.Order
            {
                CustomerId = orderDto.CustomerId,
                Status = "Pending",
                TotalPrice = orderDto.TotalPrice,
                CreatedDate = DateTime.Now,
                Items = orderDto.Items.Select(x => new OrderItem
                {
                    ProductId = x.ProductId,
                    Count = x.Count,
                    Price = 0 // Price logic should come from Product Service in a real scenario.
                }).ToList()
            };

            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync(); // Order ID is generated here.

            // 2. Create Integration Event
            // This event will be consumed by the Stock Service.
            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = newOrder.Id,
                CustomerId = orderDto.CustomerId,
                TotalPrice = orderDto.TotalPrice,
                OrderItems = orderDto.Items.Select(x => new OrderItemMessage
                {
                    ProductId = x.ProductId,
                    Count = x.Count
                }).ToList()
            };

            // 3. Publish to Message Broker (RabbitMQ)
            await _publishEndpoint.Publish(orderCreatedEvent);

            return Ok(new { Status = "Success", OrderId = newOrder.Id, Message = "Order saved to DB and Event published." });
        }
    }
}