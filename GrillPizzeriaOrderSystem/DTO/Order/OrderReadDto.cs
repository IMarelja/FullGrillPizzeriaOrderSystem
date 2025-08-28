using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Food;

namespace DTO.Order
{
    public class OrderItemReadDto
    {
        public FoodReadDto Food { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; } // Quantity * UnitPrice
    }

    public class OrderReadDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public IEnumerable<OrderItemReadDto> Items { get; set; } = Enumerable.Empty<OrderItemReadDto>();
        public decimal OrderTotal { get; set; }
    }
}
