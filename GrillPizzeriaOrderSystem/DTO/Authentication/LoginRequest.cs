using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConstants;

namespace DTO.Authentication
{
    public class LoginRequest
    {
        [Required]
        [StringLength(ValidationConstants.NameMaxLength, ErrorMessage = "Username must not exceed 100 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
