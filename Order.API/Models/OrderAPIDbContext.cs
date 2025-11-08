using Microsoft.EntityFrameworkCore;
using Order.API.Models.Entitites;

namespace Order.API.Models
{
    public class OrderAPIDbContext : DbContext
    {
        public OrderAPIDbContext(DbContextOptions options) : base(options)
        {


        }

        public DbSet<Entitites.Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        

    }
}
