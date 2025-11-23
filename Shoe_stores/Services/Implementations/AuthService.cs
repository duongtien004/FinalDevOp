using Microsoft.EntityFrameworkCore;
using ShoeStoreBackend.Data;
using ShoeStoreBackend.Dtos;
using ShoeStoreBackend.DTOs;
using ShoeStoreBackend.Models;
using System.Threading.Tasks;

namespace ShoeStoreBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public AuthService(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService; // Gán giá trị cho trường private
        }

        public async Task<object> RegisterAsync(RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                throw new BadHttpRequestException("Email đã tồn tại");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new
            {
                message = "Đăng ký thành công",
                user = new
                {
                    user.Id,
                    user.Username,
                    user.Email,
                    user.Role
                }
            };
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Sai email hoặc mật khẩu");
            }

            return _tokenService.GenerateToken(user);
        }

        public async Task<object> GetCurrentUserAsync(string userId)
{
    if (string.IsNullOrEmpty(userId))
    {
        throw new UnauthorizedAccessException("Không tìm thấy userId trong token.");
    }

    var user = await _context.Users.FindAsync(int.Parse(userId));
    if (user == null)
    {
        throw new BadHttpRequestException("Người dùng không tồn tại.");
    }

    return new
    {
        user.Id,
        user.Username,
        user.Email,
        user.Role
    };
}
    }
}