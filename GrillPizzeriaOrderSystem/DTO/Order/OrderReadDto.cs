using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Order
{
    public class OrderItemReadDto
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; } = string.Empty;
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
