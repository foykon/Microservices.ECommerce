using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Models;
using Order.API.Models.Entitites;
using Order.API.ViewModels;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        readonly OrderAPIDbContext _context;

        public OrderController(OrderAPIDbContext context)
        {
            _context = context;
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
             
            return Ok();
        }
    }
}
