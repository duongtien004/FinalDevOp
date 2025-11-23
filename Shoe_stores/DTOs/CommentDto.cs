using System;

namespace ShoeStoreBackend.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; } // Lấy từ User
        public int ProductId { get; set; }

        public string? ProductName { get; set; }
    }
}