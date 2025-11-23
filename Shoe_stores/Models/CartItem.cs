using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoeStoreBackend.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }
}