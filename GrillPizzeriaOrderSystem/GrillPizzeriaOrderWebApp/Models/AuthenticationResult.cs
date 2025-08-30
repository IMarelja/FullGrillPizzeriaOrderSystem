namespace GrillPizzeriaOrderWebApp.Models
{
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
        public int? StatusCode { get; set; }
        public UserProfileAuthenticated? UserProfile { get; set; }
    }

    public class UserProfileAuthenticated
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}
