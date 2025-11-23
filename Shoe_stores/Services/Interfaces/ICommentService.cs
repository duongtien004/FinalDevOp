using ShoeStoreBackend.DTOs;
using System.Threading.Tasks;

namespace ShoeStoreBackend.Services.Interfaces
{
    public interface ICommentService
    {
        Task<CommentDto> CreateCommentAsync(CommentCreateDto commentDto, int userId);
        Task<List<CommentDto>> GetCommentsByProductAsync(int productId);
        Task<CommentDto> GetCommentByIdAsync(int id);
        Task<CommentDto> UpdateCommentAsync(int id, CommentCreateDto dto, int userId); // Chỉ 3 tham số
        Task<bool> DeleteCommentAsync(int id, int userId);
    }
}