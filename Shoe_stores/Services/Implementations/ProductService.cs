using Microsoft.EntityFrameworkCore;
using ShoeStoreBackend.Data;
using ShoeStoreBackend.DTOs;
using ShoeStoreBackend.Models;
using System.Security.Claims;

namespace ShoeStoreBackend.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Kiểm tra xem người dùng hiện tại có phải là Admin không
        private bool IsAdmin()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return false;

            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            return role == "Admin";
        }

        // READ: Cả User và Admin đều có thể truy cập
            public async Task<IEnumerable<ProductDto>> GetAllAsync(string? search)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower(); // chuẩn hóa chữ thường

                query = query.Where(p =>
                    p.Name.ToLower().Contains(search) ||
                    p.Description.ToLower().Contains(search) ||
                    p.Category.ToLower().Contains(search)
                );
            }

            return await query.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Category = p.Category
            }).ToListAsync();
        }

        // READ: Cả User và Admin đều có thể truy cập
        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Category = product.Category
            };
        }

        // CREATE: Chỉ Admin được phép
        public async Task<ProductDto> CreateAsync(ProductDto productDto)
        {
            if (!IsAdmin())
            {
                throw new UnauthorizedAccessException("Only Admin can create products.");
            }

            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                ImageUrl = productDto.ImageUrl,
                Category = productDto.Category
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            productDto.Id = product.Id;
            return productDto;
        }

        // UPDATE: Chỉ Admin được phép
        public async Task<ProductDto?> UpdateAsync(int id, ProductDto productDto)
        {
            if (!IsAdmin())
            {
                throw new UnauthorizedAccessException("Only Admin can update products.");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null) return null;

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.ImageUrl = productDto.ImageUrl;
            product.Category = productDto.Category;

            await _context.SaveChangesAsync();

            return productDto;
        }

        // DELETE: Chỉ Admin được phép
        public async Task<bool> DeleteAsync(int id)
        {
            if (!IsAdmin())
            {
                throw new UnauthorizedAccessException("Only Admin can delete products.");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}