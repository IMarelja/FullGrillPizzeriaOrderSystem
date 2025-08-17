using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class UserViewNormalModel
    {
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        [Phone]
        public string Phone { get; set; } = string.Empty;
    }
}
