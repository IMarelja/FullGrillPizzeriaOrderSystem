using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ModelConstants;

namespace ViewModels
{
    public class FoodCreateViewModel
    {
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
        public IFormFile? ImageFile { get; set; }

        [Required]
        public int FoodCategoryId { get; set; }

        public List<int> SelectedAllergenIds { get; set; } = new();
    }

    public class FoodEditViewModel : FoodCreateViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}
