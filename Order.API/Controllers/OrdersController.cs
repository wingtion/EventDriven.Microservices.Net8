using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Order.API.DTOs;
using Order.API.Models; // Veritabanı modelleri için
using Shared.Events;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly OrderDbContext _context; // Veritabanı bağlantımız

        public OrdersController(IPublishEndpoint publishEndpoint, OrderDbContext context)
        {
            _publishEndpoint = publishEndpoint;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateDto orderDto)
        {
            // 1. ÖNCE VERİTABANINA KAYDET (Pending - Beklemede)
            // Gelen DTO'yu veritabanı nesnesine (Entity) çeviriyoruz.
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
                    Price = 0 // Fiyatı şimdilik 0 geçiyoruz (Normalde Product servisinden sorulur)
                }).ToList()
            };

            await _context.Orders.AddAsync(newOrder);
            await _context.SaveChangesAsync(); // SQL'e yazıldı, gerçek ID oluştu!

            // 2. EVENT OLUŞTUR (RabbitMQ için)
            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = newOrder.Id, // Artık gerçek ID kullanıyoruz
                CustomerId = orderDto.CustomerId,
                TotalPrice = orderDto.TotalPrice,
                OrderItems = orderDto.Items.Select(x => new OrderItemMessage
                {
                    ProductId = x.ProductId,
                    Count = x.Count
                }).ToList()
            };

            // 3. RABBITMQ'YA FIRLAT
            await _publishEndpoint.Publish(orderCreatedEvent);

            return Ok(new { Status = "Success", OrderId = newOrder.Id, Message = "Order saved to DB and Event published." });
        }
    }
}