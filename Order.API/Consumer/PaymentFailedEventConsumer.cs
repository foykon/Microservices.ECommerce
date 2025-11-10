using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Consumer
{
    public class PaymentFailedEventConsumer : IConsumer<Shared.Events.PaymentFailedEvent>
    {
        readonly OrderAPIDbContext _context;

        public PaymentFailedEventConsumer(OrderAPIDbContext context)
        {
            _context = context;
        }
        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            Models.Entitites.Order order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == context.Message.OrderId);
            order.OrderStatus = Models.Entitites.Enums.OrderStatus.Failed;
            await _context.SaveChangesAsync();
        }
    }
}
