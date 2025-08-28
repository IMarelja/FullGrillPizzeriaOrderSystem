using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.MultiViewModels
{
    public class FoodCreationMultiView
    {
        public FoodCreateViewModel NewFood { get; set; } = new();
        public List<AllergenViewModel> Allergens { get; set; } = new();
        public List<CategoryFoodCreateViewModel> FoodCategories { get; set; } = new();
    }

    public class FoodEditMultiView
    {
        public FoodEditViewModel NewFood { get; set; } = new();
        public List<AllergenViewModel> Allergens { get; set; } = new();
        public List<CategoryFoodCreateViewModel> FoodCategories { get; set; } = new();
    }
}
