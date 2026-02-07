using MassTransit;
using Microsoft.EntityFrameworkCore;
using Stock.API.Consumers;
using Stock.API.Models;

var builder = WebApplication.CreateBuilder(args);

// SQL Server Configuration for Stock Service
builder.Services.AddDbContext<StockDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("StockDbConnection"));
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- MASSTRANSIT CONFIGURATION (CONSUMER) ---
builder.Services.AddMassTransit(config =>
{
    // 1. Register the Consumer
    // We tell MassTransit that this class will handle messages.
    config.AddConsumer<OrderCreatedEventConsumer>();

    config.UsingRabbitMq((context, cfg) =>
    {
        // RabbitMQ Connection
        cfg.Host(builder.Configuration["MassTransit:Host"] ?? "localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        // 2. Configure the Receive Endpoint (QUEUE)
        // This creates a queue named "stock-order-created" in RabbitMQ.
        cfg.ReceiveEndpoint("stock-order-created", e =>
        {
            // Bind the consumer to this queue
            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
        });
    });
});
// -------------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();