using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class FoodViewModel
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public decimal price { get; set; }
        public string? imagePath { get; set; }
        public CategoryFoodViewModel category { get; set; } = new();
        public List<AllergenViewModel> allergens { get; set; } = new();
    }
}
