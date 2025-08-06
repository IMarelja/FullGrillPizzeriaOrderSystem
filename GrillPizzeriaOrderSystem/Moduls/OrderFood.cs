using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class OrderFood
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int FoodId { get; set; }
        public Food Food { get; set; } = null!;
    }


}
