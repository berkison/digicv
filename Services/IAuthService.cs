using Simple.DTOs;

namespace Simple.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(UserDto request);
        Task<bool> RegisterAsync(UserDto request);

        //Task<bool> DeleteAsync(UserDto request);
    }
}
