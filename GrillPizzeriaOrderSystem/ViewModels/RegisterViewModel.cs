using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConstants;

namespace ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Username")]
        [MaxLength(ValidationConstants.NameMaxLength, ErrorMessage = "Username must be at most 100 characters long.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [MaxLength(ValidationConstants.EmailMaxLength, ErrorMessage = "Email must be at most 255 characters long.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "First Name")]
        [MaxLength(ValidationConstants.NameMaxLength, ErrorMessage = "First name must be at most 100 characters long.")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        [MaxLength(ValidationConstants.NameMaxLength, ErrorMessage = "Last name must be at most 100 characters long.")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        [MaxLength(ValidationConstants.PhoneMaxLength, ErrorMessage = "Phone number must be at most 40 characters long.")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [MinLength(ValidationConstants.PasswordPlainMinLenght, ErrorMessage = "Password must be at least 8 characters long.")]
        [MaxLength(ValidationConstants.PasswordPlainMaxLenght, ErrorMessage = "Password must be at most 100 characters long.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
