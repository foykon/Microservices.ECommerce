using MassTransit;
using MassTransit.Transports;
using Shared.Events;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<Shared.Events.StockReservedEvent>
    {
        readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            // Payment process
            if (true)
            {
                // Successful payment
                Console.WriteLine("Payment Successful");

                PaymentCompletedEvent paymentCompletedEvent = new PaymentCompletedEvent
                {
                    OrderId = context.Message.OrderId
                };

                await _publishEndpoint.Publish(paymentCompletedEvent);
                

            }
            else
            {
                // Failed payment
                Console.WriteLine("Payment Failed");

            }
        }
    }
}
