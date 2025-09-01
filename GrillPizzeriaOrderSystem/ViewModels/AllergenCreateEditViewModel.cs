using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConstants;

namespace ViewModels
{
    public class AllergenCreateViewModel
    {
        [Required]
        [StringLength(ValidationConstants.NameMaxLength)]
        public string name { get; set; } = string.Empty;
    }

    public class AllergenEditViewModel : AllergenCreateViewModel
    {
        [Required]
        public int id { get; set; }
    }
}
