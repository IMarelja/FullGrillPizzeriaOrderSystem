using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConstants;

namespace DTO.Food
{
    public class FoodUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(ValidationConstants.NameMaxLength)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationConstants.DescriptionMaxLength)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DecimalRange((int)ValidationConstants.PriceDecimalInteger, (int)ValidationConstants.PriceDecimalFraction)]
        public decimal Price { get; set; }

        [StringLength(ValidationConstants.ImagePathMaxLength)]
        public string? ImagePath { get; set; }

        [Required]
        public int FoodCategoryId { get; set; }

        [Required]
        public List<int> AllergenIds { get; set; } = new();
    }
}
