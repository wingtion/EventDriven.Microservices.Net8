using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;

var builder = WebApplication.CreateBuilder(args);

// SQL Server Configuration for Order Service
builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrderDbConnection"));
});

// --- 1. Service Registration ---

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- 2. MassTransit & RabbitMQ Configuration ---
builder.Services.AddMassTransit(config =>
{
    // Configure RabbitMQ Connection
    config.UsingRabbitMq((context, cfg) =>
    {
        // Connection string to RabbitMQ running on Docker
        // Host: localhost (because Docker maps ports to localhost)
        // VirtualHost: "/" (Default)
        cfg.Host(builder.Configuration["MassTransit:Host"] ?? "localhost", h =>
        {
            h.Username("guest"); // Default RabbitMQ user
            h.Password("guest"); // Default RabbitMQ password
        });
    });
});
// ----------------------------------------------

var app = builder.Build();

// --- 3. HTTP Request Pipeline ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();