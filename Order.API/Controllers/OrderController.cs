using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Models;
using Order.API.Models.Entitites;
using Order.API.ViewModels;
using Shared.Events;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        readonly OrderAPIDbContext _context;

        readonly IPublishEndpoint _publishEndpoint;
        public OrderController(OrderAPIDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderVM createOrder)
        {
            Models.Entitites.Order order = new Models.Entitites.Order
            {
                OrderId = Guid.NewGuid(),
                BuyerId = createOrder.BuyerId,
                CreateDate = DateTime.Now,
                OrderStatus = Models.Entitites.Enums.OrderStatus.Completed,

                
            };
            order.OrderItems = createOrder.CreateOrderItems.Select(oi => new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.OrderId,
                ProductId = oi.ProductId,
                Count = oi.Count,
                Price = oi.Price
            }).ToList();
            order.TotalPrice = order.OrderItems.Sum(oi => oi.Count * oi.Price);

            await _context.AddAsync(order);
            await _context.SaveChangesAsync();
             
            OrderCreatedEvent orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = order.OrderId,
                BuyerId = order.BuyerId,
                OrderItems = order.OrderItems.Select(oi => new Shared.Messages.OrderItemMesagge
                {
                    ProductId = oi.ProductId,
                    Count = oi.Count
                }).ToList()
            };

            await _publishEndpoint.Publish(orderCreatedEvent); 

            return Ok();
        }
    }
}
