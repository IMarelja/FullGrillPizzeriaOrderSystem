using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConstants;

namespace Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(ValidationConstants.NameMaxLength)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationConstants.PasswordHashMaxLength)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(ValidationConstants.EmailMaxLength)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationConstants.NameMaxLength)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationConstants.NameMaxLength)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationConstants.PhoneMaxLength)]
        [Phone]
        public string Phone { get; set; } = string.Empty;
        public DateTime DateCreation { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }


}
