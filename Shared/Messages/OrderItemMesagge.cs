using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Messages
{
    public class OrderItemMesagge
    {
        public Guid ProductId { get; set; }
        public int Count { get; set; }
    }
}
