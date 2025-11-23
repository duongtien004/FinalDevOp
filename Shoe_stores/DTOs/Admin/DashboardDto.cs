namespace ShoeStoreBackend.DTOs.Admin
{
    public class DashboardDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public List<TopProductDto> TopProducts { get; set; } = new List<TopProductDto>();
    }
}