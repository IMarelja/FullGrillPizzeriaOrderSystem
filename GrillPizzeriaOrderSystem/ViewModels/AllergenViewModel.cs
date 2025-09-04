using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class AllergenViewModel
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
    }

    public class AllergenDeleteViewModel
    {
        public int id { get; set; }
    }
}
