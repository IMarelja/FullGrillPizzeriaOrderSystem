using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required, DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string currentPassword { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [MinLength(ModelConstants.ValidationConstants.PasswordPlainMinLenght,
            ErrorMessage = "Password must be at least 8 characters long.")]
        [MaxLength(ModelConstants.ValidationConstants.PasswordPlainMaxLenght,
            ErrorMessage = "Password must be at most 100 characters long.")]
        public string newPassword { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare(nameof(newPassword), ErrorMessage = "Passwords do not match.")] // << fix
        public string confirmNewPassword { get; set; } = string.Empty;
    }

}
