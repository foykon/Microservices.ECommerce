using Order.API.Models.Entitites.Enums;

namespace Order.API.Models.Entitites
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public Guid BuyerId { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
