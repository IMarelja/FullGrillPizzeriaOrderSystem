using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class OrderItemViewModel
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
    }

    public class OrderViewModel
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Username { get; set; } = string.Empty;

        public List<OrderItemViewModel> Items { get; set; } = new();
        public decimal OrderTotalPrice { get; set; }
    }

    public class OrderDeleteViewModel
    {
        public int id { get; set; }
    }
}
