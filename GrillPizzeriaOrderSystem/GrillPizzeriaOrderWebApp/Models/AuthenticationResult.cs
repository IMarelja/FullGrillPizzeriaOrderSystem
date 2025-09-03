using ViewModels;

namespace GrillPizzeriaOrderWebApp.Models
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
        public int? StatusCode { get; set; }
        public UserViewModel? UserProfile { get; set; }
    }
}
