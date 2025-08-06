namespace GrillPizzeriaOrderMiddleware.Services
{
    public interface IAuthenticationRepository
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<int> CreateUserAsync(User user);
        Task<bool> AuthenticateUserAsync(string username, string password);
    }
}
