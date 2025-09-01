using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConstants;

namespace ViewModels
{
    public class CategoryFoodCreateViewModel
    {
        [Required]
        [StringLength(ValidationConstants.NameMaxLength)]
        public string name { get; set; } = string.Empty;
    }

    public class CategoryFoodEditViewModel : CategoryFoodCreateViewModel
    {
        [Required]
        public int id { get; set; }
    }
}
