using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoeStoreBackend.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace ShoeStoreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue([FromQuery] string? period = "day", [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var result = await _adminService.GetRevenueAsync(period ?? "day", startDate, endDate);
            return Ok(result);
        }

        [HttpGet("revenue/trend")]
        public async Task<IActionResult> GetRevenueTrend([FromQuery] string? period = "day", [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var result = await _adminService.GetRevenueTrendAsync(period ?? "day", startDate, endDate);
            return Ok(result);
        }

        [HttpGet("orders/status")]
        public async Task<IActionResult> GetOrdersByStatus()
        {
            var result = await _adminService.GetOrdersByStatusAsync();
            return Ok(result);
        }

        [HttpGet("orders/distribution")]
        public async Task<IActionResult> GetOrderDistribution([FromQuery] string? period = "day", [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var result = await _adminService.GetOrderDistributionAsync(period ?? "day", startDate, endDate);
            return Ok(result);
        }

        [HttpGet("products/top-selling")]
        public async Task<IActionResult> GetTopSellingProducts([FromQuery] int limit = 10)
        {
            var result = await _adminService.GetTopSellingProductsAsync(limit);
            return Ok(result);
        }

        [HttpGet("products/inventory")]
        public async Task<IActionResult> GetInventoryProducts([FromQuery] string type = "most", [FromQuery] int limit = 10)
        {
            var result = await _adminService.GetInventoryProductsAsync(type, limit);
            return Ok(result);
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _adminService.GetDashboardAsync();
            return Ok(result);
        }

        [HttpGet("report/excel")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var fileContent = await _adminService.ExportExcelReportAsync(startDate, endDate);
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BaoCaoDonHang.xlsx");
        }

        [HttpGet("report/pdf")]
        public async Task<IActionResult> ExportPdf([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            var fileContent = await _adminService.ExportPdfReportAsync(startDate, endDate);
            return File(fileContent, "application/pdf", "BaoCaoDonHang.pdf");
        }
    }
}