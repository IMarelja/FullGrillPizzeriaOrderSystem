using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class FoodAllergen
    {
        public int FoodId { get; set; }
        public Food Food { get; set; } = null!;

        public int AllergenId { get; set; }
        public Allergen Allergen { get; set; } = null!;
    }


}
