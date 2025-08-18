using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConstants;

namespace DTO.User
{
    public class UserUpdateDto
    {
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
    }

}
