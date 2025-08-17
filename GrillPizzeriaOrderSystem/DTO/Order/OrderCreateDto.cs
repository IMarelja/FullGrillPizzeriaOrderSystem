using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConstants;

namespace DTO.Order
{
    public class OrderItemCreateDto
    {
        [Required]
        public int FoodId { get; set; }

        [Range(ValidationConstants.QuantityOfFoodMin, ValidationConstants.QuantityOfFoodMax)]
        public int Quantity { get; set; }
    }

    public class OrderCreateDto
    {
        [Required]
        [MinLength(ValidationConstants.ItemsRequestMin)]
        public List<OrderItemCreateDto> Items { get; set; } = new();
    }
}
