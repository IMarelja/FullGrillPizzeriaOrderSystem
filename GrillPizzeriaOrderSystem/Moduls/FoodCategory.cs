using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConstants;

namespace Models
{
    public class FoodCategory
    {
        public int Id { get; set; }

        [Required]
        [StringLength(ValidationConstants.NameMaxLength)]
        public string Name { get; set; } = string.Empty;

        public ICollection<Food> Foods { get; set; } = new List<Food>();
    }


}
