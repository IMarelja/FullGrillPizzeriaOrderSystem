using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConstants;

namespace Models
{
    public class Food
    {
        public int Id { get; set; }

        [Required]
        [StringLength(ValidationConstants.NameMaxLength)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationConstants.DescriptionMaxLength)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DecimalRangeAttribute((int)ValidationConstants.PriceDecimalInteger, (int)ValidationConstants.PriceDecimalFraction)]
        public decimal Price { get; set; }

        [StringLength(ValidationConstants.ImagePathMaxLength)]
        public string? ImagePath { get; set; }

        public int FoodCategoryId { get; set; }
        public FoodCategory FoodCategory { get; set; } = null!;

        public ICollection<FoodAllergen> FoodAllergens { get; set; } = new List<FoodAllergen>();
        public ICollection<OrderFood> OrderFoods { get; set; } = new List<OrderFood>();
    }


}
