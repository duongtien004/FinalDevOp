namespace ShoeStoreBackend.DTOs.Admin
{
    public class TopProductDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}