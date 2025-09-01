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
        public string name { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationConstants.DescriptionMaxLength)]
        public string description { get; set; } = string.Empty;

        [Required]
        [DecimalRange((int)ValidationConstants.PriceDecimalInteger, (int)ValidationConstants.PriceDecimalFraction)]
        public decimal price { get; set; }
        [StringLength(ValidationConstants.ImagePathMaxLength)]
        public string? imagePath { get; set; }
        public IFormFile? imageFile { get; set; }

        [Required]
        public int foodCategoryId { get; set; }

        public List<int> allergenIds { get; set; } = new();
    }

    public class FoodEditViewModel : FoodCreateViewModel
    {
        [Required]
        public int id { get; set; }
    }
}
