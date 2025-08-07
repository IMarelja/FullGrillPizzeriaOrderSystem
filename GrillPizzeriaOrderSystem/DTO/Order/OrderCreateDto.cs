using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Order
{
    public class OrderCreateDto
    {
        public int UserId { get; set; }
        public List<int> FoodIds { get; set; } = new();
    }
}
