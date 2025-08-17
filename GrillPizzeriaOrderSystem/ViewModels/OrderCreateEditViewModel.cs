using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{

    public class OrderCreateViewModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public List<int> SelectedFoodIds { get; set; } = new();
    }

    public class OrderEditViewModel : OrderCreateViewModel
    {
        [Required]
        public int Id { get; set; }
    }
}
