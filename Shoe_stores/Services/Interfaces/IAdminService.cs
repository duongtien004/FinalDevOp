using ShoeStoreBackend.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShoeStoreBackend.Services.Interfaces
{
    public interface IAdminService
    {
        Task<List<RevenueDto>> GetRevenueAsync(string period, DateTime? startDate, DateTime? endDate);
        Task<object> GetRevenueTrendAsync(string period, DateTime? startDate, DateTime? endDate);
        Task<List<OrderStatusDto>> GetOrdersByStatusAsync();
        Task<object> GetOrderDistributionAsync(string period, DateTime? startDate, DateTime? endDate);
        Task<List<TopProductDto>> GetTopSellingProductsAsync(int limit);
        Task<List<InventoryProductDto>> GetInventoryProductsAsync(string type, int limit);
        Task<DashboardDto> GetDashboardAsync();
        Task<byte[]> ExportExcelReportAsync(DateTime? startDate, DateTime? endDate);
        Task<byte[]> ExportPdfReportAsync(DateTime? startDate, DateTime? endDate);
    }
}