using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Food
{
    public class FoodReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
        public int FoodCategoryId { get; set; }
        public string FoodCategoryName { get; set; } = string.Empty;
        public List<string> AllergenNames { get; set; } = new();
    }
}
