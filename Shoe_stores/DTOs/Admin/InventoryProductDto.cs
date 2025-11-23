namespace ShoeStoreBackend.DTOs.Admin
{
    public class InventoryProductDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int TotalSold { get; set; } // Ước tính tồn kho dựa trên số lượng đã bán
    }
}