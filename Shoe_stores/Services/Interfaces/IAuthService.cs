using ShoeStoreBackend.Dtos;
using System.Threading.Tasks;

namespace ShoeStoreBackend.Services
{
    public interface IAuthService
    {
        Task<object> RegisterAsync(RegisterDto registerDto);
        Task<string> LoginAsync(LoginDto loginDto);
        Task<object> GetCurrentUserAsync(string userId);
    }
}