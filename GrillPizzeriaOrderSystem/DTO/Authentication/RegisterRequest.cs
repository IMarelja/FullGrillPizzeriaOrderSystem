using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConstants;

namespace DTO.Authentication
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(ValidationConstants.NameMaxLength, ErrorMessage = "Username must not exceed 100 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(ValidationConstants.EmailMaxLength, ErrorMessage = "Email must not exceed 200 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationConstants.PasswordPlainMaxLenght, MinimumLength = ValidationConstants.PasswordPlainMinLenght, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        [StringLength(ValidationConstants.NameMaxLength, ErrorMessage = "First name must not exceed 100 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationConstants.NameMaxLength, ErrorMessage = "Last name must not exceed 100 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(ValidationConstants.PhoneMaxLength, ErrorMessage = "Phone number must not exceed 50 characters.")]
        public string Phone { get; set; } = string.Empty;
    }
}
