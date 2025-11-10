using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumer;
using Order.API.Models;
using Shared;
using Shared.Events;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<OrderAPIDbContext>(options=>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"));
});

builder.Services.AddMassTransit(configure =>
{
    configure.AddConsumer<PaymentCompletedEventConsumer>();
    configure.AddConsumer<StockNotReservedEventConsumer>();
    configure.AddConsumer<PaymentFailedEventConsumer>();
    configure.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ"]);
        cfg.ReceiveEndpoint(RabbitMQSettings.Order_PaymentCompletedEventQueue, e =>
        {
            e.ConfigureConsumer<PaymentCompletedEventConsumer>(context);
        });
        cfg.ReceiveEndpoint(RabbitMQSettings.Order_StockNotReservedEventQueue, e =>
        {
            e.ConfigureConsumer<StockNotReservedEventConsumer>(context);
        });
        cfg.ReceiveEndpoint(RabbitMQSettings.Order_PaymentFailedEventQueue, e =>
        {
            e.ConfigureConsumer<PaymentFailedEventConsumer>(context);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
