using ShoeStoreBackend.Data;
using ShoeStoreBackend.DTOs;
using ShoeStoreBackend.Models;
using ShoeStoreBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoeStoreBackend.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly AppDbContext _context;

        public CommentService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<CommentDto> CreateCommentAsync(CommentCreateDto commentDto, int userId)
        {
            if (!await _context.Products.AnyAsync(p => p.Id == commentDto.ProductId))
                throw new BadHttpRequestException("Product not found");

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var comment = new Comment
                    {
                        Content = commentDto.Content ?? throw new ArgumentNullException(nameof(commentDto.Content)),
                        UserId = userId,
                        ProductId = commentDto.ProductId,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Comments.Add(comment);
                    await _context.SaveChangesAsync();

                    var user = await _context.Users
                        .Where(u => u.Id == userId)
                        .Select(u => new { u.Username })
                        .FirstOrDefaultAsync();
                    var product = await _context.Products
                        .Where(p => p.Id == commentDto.ProductId)
                        .Select(p => new { p.Name })
                        .FirstOrDefaultAsync();

                    var result = new CommentDto
                    {
                        Id = comment.Id,
                        Content = comment.Content,
                        Username = user?.Username ?? $"User_{userId}",
                        ProductName = product?.Name ?? $"Product_{commentDto.ProductId}",
                        CreatedAt = comment.CreatedAt
                    };

                    await transaction.CommitAsync();
                    return result;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<List<CommentDto>> GetCommentsByProductAsync(int productId)
        {
            var comments = await _context.Comments
                .Where(c => c.ProductId == productId)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    Username = _context.Users
                        .Where(u => u.Id == c.UserId)
                        .Select(u => u.Username)
                        .FirstOrDefault() ?? $"User_{c.UserId}",
                    ProductName = _context.Products
                        .Where(p => p.Id == c.ProductId)
                        .Select(p => p.Name)
                        .FirstOrDefault() ?? $"Product_{c.ProductId}",
                    CreatedAt = c.CreatedAt
                }).ToListAsync();

            return comments;
        }

        public async Task<CommentDto> GetCommentByIdAsync(int id)
        {
            _context.ChangeTracker.Clear();
            var comment = await _context.Comments
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            Console.WriteLine($"GetCommentByIdAsync: Checking id {id}, Found: {comment != null}, UserId: {comment?.UserId}");

            if (comment == null) throw new BadHttpRequestException("Comment not found");

            var user = await _context.Users
                .Where(u => u.Id == comment.UserId)
                .Select(u => new { u.Username })
                .FirstOrDefaultAsync();
            var product = await _context.Products
                .Where(p => p.Id == comment.ProductId)
                .Select(p => new { p.Name })
                .FirstOrDefaultAsync();

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                Username = user?.Username ?? $"User_{comment.UserId}",
                ProductName = product?.Name ?? $"Product_{comment.ProductId}",
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task<CommentDto> UpdateCommentAsync(int id, CommentCreateDto dto, int userId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.ChangeTracker.Clear();
                    var comment = await _context.Comments
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == id);
                    Console.WriteLine($"UpdateCommentAsync: Checking id {id}, Found: {comment != null}, UserId: {comment?.UserId}");

                    if (comment == null) throw new BadHttpRequestException("Comment not found");
                    // Kiểm tra quyền: Chỉ người tạo hoặc Admin được cập nhật
                    if (comment.UserId != userId)
                    {
                        // Giả định Admin có quyền, cần kiểm tra role từ context hoặc truyền từ controller
                        throw new UnauthorizedAccessException("Not authorized to update this comment.");
                    }

                    comment.Content = dto.Content ?? throw new ArgumentNullException(nameof(dto.Content));
                    _context.Comments.Update(comment);
                    await _context.SaveChangesAsync();

                    var user = await _context.Users
                        .Where(u => u.Id == userId)
                        .Select(u => new { u.Username })
                        .FirstOrDefaultAsync();
                    var product = await _context.Products
                        .Where(p => p.Id == comment.ProductId)
                        .Select(p => new { p.Name })
                        .FirstOrDefaultAsync();

                    var result = new CommentDto
                    {
                        Id = comment.Id,
                        Content = comment.Content,
                        Username = user?.Username ?? $"User_{userId}",
                        ProductName = product?.Name ?? $"Product_{comment.ProductId}",
                        CreatedAt = comment.CreatedAt
                    };

                    await transaction.CommitAsync();
                    return result;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<bool> DeleteCommentAsync(int id, int userId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.ChangeTracker.Clear();
                    var comment = await _context.Comments
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == id);
                    Console.WriteLine($"DeleteCommentAsync: Checking id {id}, Found: {comment != null}, UserId: {comment?.UserId}, Requested UserId: {userId}");

                    if (comment == null)
                    {
                        Console.WriteLine($"DeleteCommentAsync: Comment {id} not found in database.");
                        return false;
                    }
                    // Kiểm tra quyền: Chỉ người tạo hoặc Admin được xóa
                    if (comment.UserId != userId)
                    {
                        // Giả định Admin có quyền, cần kiểm tra role từ context hoặc truyền từ controller
                        throw new UnauthorizedAccessException("Not authorized to delete this comment.");
                    }

                    _context.Comments.Remove(comment);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    Console.WriteLine($"DeleteCommentAsync: Successfully deleted comment {id}");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DeleteCommentAsync: Error deleting comment {id}: {ex.Message}");
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<List<CommentDto>> GetAllCommentsAsync()
        {
            var comments = await _context.Comments
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    Username = _context.Users
                        .Where(u => u.Id == c.UserId)
                        .Select(u => u.Username)
                        .FirstOrDefault() ?? $"User_{c.UserId}",
                    ProductName = _context.Products
                        .Where(p => p.Id == c.ProductId)
                        .Select(p => p.Name)
                        .FirstOrDefault() ?? $"Product_{c.ProductId}",
                    CreatedAt = c.CreatedAt
                }).ToListAsync();

            return comments;
        }
    }
}