using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class FoodSearchViewModel
    {
        public int total { get; set; }
        public int currentPage { get; set; }
        public int currentPageSize { get; set; }
        public int pageSize { get; set; }
        public int totalPages { get; set; }
        public IEnumerable<FoodViewModel> data { get; set; } = Enumerable.Empty<FoodViewModel>();
    }
}
