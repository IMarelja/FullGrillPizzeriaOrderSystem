using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConstants;

namespace Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        [Required]
        [DecimalRangeAttribute((int)ValidationConstants.PriceDecimalInteger, (int)ValidationConstants.PriceDecimalFraction)]
        public decimal OrderTotalPrice { get; set; }

        public ICollection<OrderFood> OrderFoods { get; set; } = new List<OrderFood>();
    }


}
