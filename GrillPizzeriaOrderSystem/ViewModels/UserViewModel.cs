using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class UserViewModel
    {
        public int id { get; set; }

        public string username { get; set; } = string.Empty;

        public string email { get; set; } = string.Empty;

        public string firstName { get; set; } = string.Empty;

        public string lastName { get; set; } = string.Empty;

        public string phone { get; set; } = string.Empty;

        public DateTime creationDate { get; set; }

        public string roleName { get; set; } = string.Empty;
    }
}
