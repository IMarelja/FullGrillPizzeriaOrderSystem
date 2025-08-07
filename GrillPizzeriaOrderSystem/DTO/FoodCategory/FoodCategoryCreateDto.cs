using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConstants;

namespace DTO.FoodCategory
{
    public class FoodCategoryCreateDto
    {
        [Required]
        [StringLength(ValidationConstants.NameMaxLength)]
        public string Name { get; set; } = string.Empty;
    }
}
