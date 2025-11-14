using MassTransit;
using MongoDB.Driver;
using Shared;
using StockAPI.Consumers;
using StockAPI.Models;
using StockAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ"]);
        cfg.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedEventQueue, e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
        }); 
    });
});

builder.Services.AddSingleton<MongoDBService>();

using IServiceScope scope = builder.Services.BuildServiceProvider().CreateScope();

MongoDBService mongoDBService = scope.ServiceProvider.GetService<MongoDBService>();

var collection = mongoDBService.GetCollection<Stock>();

#region set initial data
if (!collection.FindSync<Stock>(Builders<Stock>.Filter.Empty).Any())
{
    await collection.InsertManyAsync(new List<Stock>
    {
        new Stock
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.Parse("7854B46A-92C5-4AF5-BE07-E92EC6538933"),
            Count = 100
        },
        new Stock
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.Parse("BF757F7C-A6CF-4C99-A0C3-A2B412FF3168"),
            Count = 200
        },
        new Stock
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.Parse("9495C5F8-E56B-428B-8A9E-B17988BA0EEB"),
            Count = 300
        }
    });
}
#endregion


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
app.MapGet("/ready", () =>
{
    Console.WriteLine("Stcok.API is ready.");
    return false;
});
app.MapGet("/commit", () =>
{
    Console.WriteLine("Stcok.API is commited.");
    return true;
});
app.MapGet("/rollback", () =>
{
    Console.WriteLine("Stcok.API is rollbacked.");
});
app.Run();
