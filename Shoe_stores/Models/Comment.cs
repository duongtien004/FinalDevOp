using System;
using System.ComponentModel.DataAnnotations;

namespace ShoeStoreBackend.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        public int UserId { get; set; }
        public User? User { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}