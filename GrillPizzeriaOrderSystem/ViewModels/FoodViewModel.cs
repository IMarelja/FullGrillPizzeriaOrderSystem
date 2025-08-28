using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class FoodViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
        public int FoodCategoryId { get; set; }
        public CategoryFoodViewModel Category { get; set; } = new();
        public List<AllergenViewModel> Allergens { get; set; } = new();
    }
}
