using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Order
{
    public class OrderReadDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }

        public string Username { get; set; } = string.Empty;
        public List<string> FoodNames { get; set; } = new();
    }
}
