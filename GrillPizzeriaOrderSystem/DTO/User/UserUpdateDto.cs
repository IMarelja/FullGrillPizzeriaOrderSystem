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
        public string email { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationConstants.NameMaxLength)]
        public string firstName { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationConstants.NameMaxLength)]
        public string lastName { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationConstants.PhoneMaxLength)]
        [Phone]
        public string phone { get; set; } = string.Empty;
    }

}
