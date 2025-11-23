using Microsoft.EntityFrameworkCore;
using ShoeStoreBackend.Models;
using BCrypt.Net;

namespace ShoeStoreBackend.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(AppDbContext context)
        {
            // Đảm bảo cơ sở dữ liệu được tạo
            await context.Database.EnsureCreatedAsync();

            // Kiểm tra xem đã có admin chưa
            if (!await context.Users.AnyAsync(u => u.Role == "Admin"))
            {
                var admin = new User
                {
                    Username = "admin",
                    Email = "admin@shoestore.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = "Admin"
                };

                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}