using System.ComponentModel.DataAnnotations;

namespace ShoeStoreBackend.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;
    }
}
