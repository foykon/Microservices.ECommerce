using MassTransit;
using MongoDB.Driver;
using Shared;
using Shared.Events;
using Shared.Messages;
using StockAPI.Models;
using StockAPI.Services;

namespace StockAPI.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        IMongoCollection<Stock> _stockCollection;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(MongoDBService mongoDBService, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _stockCollection = mongoDBService.GetCollection<Stock>();
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            foreach(var orderItem in context.Message.OrderItems)
            {
                stockResult.Add((await _stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId && s.Count >= orderItem.Count)).Any());

            }
            if (stockResult.TrueForAll(sr => sr.Equals(true)))
            {
                foreach(OrderItemMesagge orderItem in context.Message.OrderItems)
                {
                    Stock stock = await (await _stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId)).FirstOrDefaultAsync();
                    stock.Count -= orderItem.Count;
                    await _stockCollection.FindOneAndReplaceAsync(s=> s.ProductId == orderItem.ProductId,stock);

                }

                StockReservedEvent stockReservedEvent = new()
                {
                    OrderId =  context.Message.OrderId,
                    BuyerId = context.Message.BuyerId,
                    TotalPrice = context.Message.TotalPrice
                };
                
                ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Payment_StockReservedEventQueue}"));
                await sendEndpoint.Send(stockReservedEvent);

            }
            else
            {
                
                StockNotReservedEvent stockNotReservedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    BuyerId = context.Message.BuyerId,
                    Message = "Not enough stock"
                };
                await _publishEndpoint.Publish(stockNotReservedEvent);
            }
        }
    }
}
