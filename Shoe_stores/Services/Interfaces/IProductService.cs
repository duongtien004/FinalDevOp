using ShoeStoreBackend.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoeStoreBackend.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync(string? search);
        Task<ProductDto?> GetByIdAsync(int id);
        Task<ProductDto> CreateAsync(ProductDto productDto);
        Task<ProductDto?> UpdateAsync(int id, ProductDto productDto);
        Task<bool> DeleteAsync(int id);
    }
}