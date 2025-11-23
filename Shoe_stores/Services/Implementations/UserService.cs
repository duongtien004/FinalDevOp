using Microsoft.EntityFrameworkCore;
using ShoeStoreBackend.Data;
using ShoeStoreBackend.DTOs;
using ShoeStoreBackend.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShoeStoreBackend.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private bool IsAdmin()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return false;

            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            return role == "Admin";
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            if (!IsAdmin())
            {
                throw new UnauthorizedAccessException("Chỉ Admin mới có quyền xem danh sách người dùng.");
            }

            return await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Role = u.Role
                }).ToListAsync();
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            if (!IsAdmin())
            {
                throw new UnauthorizedAccessException("Chỉ Admin mới có quyền xem thông tin người dùng.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new BadHttpRequestException("Người dùng không tồn tại.");
            }

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            if (!IsAdmin())
            {
                throw new UnauthorizedAccessException("Chỉ Admin mới có quyền tạo người dùng.");
            }

            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
            {
                throw new BadHttpRequestException("Email đã tồn tại.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = passwordHash,
                Role = userDto.Role ?? "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            userDto.Id = user.Id;
            return userDto;
        }

        public async Task<UserDto> UpdateUserAsync(int id, UserDto userDto)
        {
            if (!IsAdmin())
            {
                throw new UnauthorizedAccessException("Chỉ Admin mới có quyền cập nhật người dùng.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                throw new BadHttpRequestException("Người dùng không tồn tại.");
            }

            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email && u.Id != id))
            {
                throw new BadHttpRequestException("Email đã được sử dụng bởi người dùng khác.");
            }

            user.Username = userDto.Username;
            user.Email = userDto.Email;
            user.Role = userDto.Role ?? user.Role;

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            }

            await _context.SaveChangesAsync();

            return userDto;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            if (!IsAdmin())
            {
                throw new UnauthorizedAccessException("Chỉ Admin mới có quyền xóa người dùng.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}