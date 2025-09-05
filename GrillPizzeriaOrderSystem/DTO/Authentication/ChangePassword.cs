using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelConstants;

namespace DTO.Authentication
{
    public class ChangePasswordRequest
    {
        [Required]
        [DataType(DataType.Password)]
        public string currentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationConstants.PasswordPlainMaxLenght, MinimumLength = ValidationConstants.PasswordPlainMinLenght, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [DataType(DataType.Password)]
        public string newPassword { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string confirmNewPassword { get; set; } = string.Empty;
    }
}
