using System.ComponentModel.DataAnnotations;

namespace ShoeStoreBackend.DTOs
{
    public class CommentCreateDto
    {
        [Required]
        public string? Content { get; set; }
        public int ProductId { get; set; }
    }
}