using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CategoryFoodViewModel
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
    }

    public class CategoryFoodDeleteViewModel
    {
        public int id { get; set; }
    }
}
