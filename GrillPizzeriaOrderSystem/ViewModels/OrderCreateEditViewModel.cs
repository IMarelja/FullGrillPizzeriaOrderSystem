using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConstants;

namespace ViewModels
{

    public class OrderItemCreateViewModel
    {
        [Required]
        public int foodId { get; set; }

        [Required]
        [MinLength(ValidationConstants.QuantityOfFoodMin, ErrorMessage = "Quantity must be 1 or more")]
        [MaxLength(ValidationConstants.QuantityOfFoodMax, ErrorMessage = "Quantity must be less then 100")]
        public int quantity { get; set; }
    }

    public class OrderCreateViewModel
    {

        [Required]
        [MinLength(ValidationConstants.ItemsRequestMin, ErrorMessage = "Order must have at least one item.")]
        public List<OrderItemCreateViewModel> items { get; set; } = new();
    }

    public class OrderEditViewModel : OrderCreateViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}
