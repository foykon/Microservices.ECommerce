using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Shared.Events;

namespace Order.API.Consumer
{
    public class StockNotReservedEventConsumer : IConsumer<Shared.Events.StockNotReservedEvent>
    {
        readonly OrderAPIDbContext _context;

        public StockNotReservedEventConsumer(OrderAPIDbContext context)
        {
            _context = context;
        }
        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            Models.Entitites.Order order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == context.Message.OrderId);
            order.OrderStatus = Models.Entitites.Enums.OrderStatus.Failed;
            await _context.SaveChangesAsync();
        }
    }
}
