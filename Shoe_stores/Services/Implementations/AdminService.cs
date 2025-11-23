using ClosedXML.Excel;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.EntityFrameworkCore;
using ShoeStoreBackend.Data;
using ShoeStoreBackend.DTOs.Admin;
using ShoeStoreBackend.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShoeStoreBackend.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;

        public AdminService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<RevenueDto>> GetRevenueAsync(string period, DateTime? startDate, DateTime? endDate)
        {
            period = (period ?? "day").ToLower();
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var query = _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate && o.Status == "Completed");

            IQueryable<RevenueDto> groupedQuery;
            switch (period)
            {
                case "month":
                    groupedQuery = query
                        .GroupBy(o => new { Year = o.OrderDate.Year, Month = o.OrderDate.Month })
                        .Select(g => new RevenueDto
                        {
                            Period = new DateTime(g.Key.Year, g.Key.Month, 1),
                            TotalRevenue = g.Sum(o => o.TotalAmount)
                        });
                    break;
                case "year":
                    groupedQuery = query
                        .GroupBy(o => o.OrderDate.Year)
                        .Select(g => new RevenueDto
                        {
                            Period = new DateTime(g.Key, 1, 1),
                            TotalRevenue = g.Sum(o => o.TotalAmount)
                        });
                    break;
                default: // day
                    groupedQuery = query
                        .GroupBy(o => o.OrderDate.Date)
                        .Select(g => new RevenueDto
                        {
                            Period = g.Key,
                            TotalRevenue = g.Sum(o => o.TotalAmount)
                        });
                    break;
            }

            return await groupedQuery.OrderBy(g => g.Period).ToListAsync();
        }

        public async Task<object> GetRevenueTrendAsync(string period, DateTime? startDate, DateTime? endDate)
        {
            period = (period ?? "day").ToLower();
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var data = await GetRevenueAsync(period, startDate, endDate);
            var labels = data.Select(d =>
            {
                return period switch
                {
                    "month" => d.Period.ToString("yyyy-MM"),
                    "year" => d.Period.ToString("yyyy"),
                    _ => d.Period.ToString("yyyy-MM-dd")
                };
            }).ToList();
            var revenues = data.Select(d => d.TotalRevenue).ToList();

            return new
            {
                chart = new
                {
                    type = "line",
                    data = new
                    {
                        labels,
                        datasets = new[]
                        {
                            new
                            {
                                label = "Doanh thu",
                                data = revenues,
                                borderColor = "#4A90E2",
                                backgroundColor = "rgba(74, 144, 226, 0.2)",
                                fill = true
                            }
                        }
                    },
                    options = new
                    {
                        scales = new
                        {
                            x = new { title = new { display = true, text = period == "day" ? "Ngày" : period == "month" ? "Tháng" : "Năm" } },
                            y = new { title = new { display = true, text = "Doanh thu (VND)" } }
                        }
                    }
                }
            };
        }

        public async Task<List<OrderStatusDto>> GetOrdersByStatusAsync()
        {
            return await _context.Orders
                .GroupBy(o => o.Status ?? "Unknown")
                .Select(g => new OrderStatusDto
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
        }

        public async Task<object> GetOrderDistributionAsync(string period, DateTime? startDate, DateTime? endDate)
        {
            period = (period ?? "day").ToLower();
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var query = _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate);

            IQueryable<OrderDistributionDto> groupedQuery;
            switch (period)
            {
                case "month":
                    groupedQuery = query
                        .GroupBy(o => new DateTime(o.OrderDate.Year, o.OrderDate.Month, 1))
                        .Select(g => new OrderDistributionDto
                        {
                            Period = g.Key,
                            OrderCount = g.Count()
                        });
                    break;
                case "year":
                    groupedQuery = query
                        .GroupBy(o => new DateTime(o.OrderDate.Year, 1, 1))
                        .Select(g => new OrderDistributionDto
                        {
                            Period = g.Key,
                            OrderCount = g.Count()
                        });
                    break;
                default: // day
                    groupedQuery = query
                        .GroupBy(o => o.OrderDate.Date)
                        .Select(g => new OrderDistributionDto
                        {
                            Period = g.Key,
                            OrderCount = g.Count()
                        });
                    break;
            }

            var data = await groupedQuery.OrderBy(g => g.Period).ToListAsync();
            var labels = data.Select(d =>
            {
                return period switch
                {
                    "month" => d.Period.ToString("yyyy-MM"),
                    "year" => d.Period.ToString("yyyy"),
                    _ => d.Period.ToString("yyyy-MM-dd")
                };
            }).ToList();
            var counts = data.Select(d => d.OrderCount).ToList();

            return new
            {
                chart = new
                {
                    type = "line",
                    data = new
                    {
                        labels,
                        datasets = new[]
                        {
                            new
                            {
                                label = "Số đơn hàng",
                                data = counts,
                                borderColor = "#50C878",
                                backgroundColor = "rgba(80, 200, 120, 0.2)",
                                fill = true
                            }
                        }
                    },
                    options = new
                    {
                        scales = new
                        {
                            x = new { title = new { display = true, text = period == "day" ? "Ngày" : period == "month" ? "Tháng" : "Năm" } },
                            y = new { title = new { display = true, text = "Số đơn hàng" } }
                        }
                    }
                }
            };
        }

        public async Task<List<TopProductDto>> GetTopSellingProductsAsync(int limit)
        {
            return await _context.OrderItems
                .Include(oi => oi.Product)
                .GroupBy(oi => new { oi.ProductId, oi.Product!.Name })
                .Select(g => new TopProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    TotalSold = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.Quantity * oi.Price)
                })
                .OrderByDescending(g => g.TotalSold)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<InventoryProductDto>> GetInventoryProductsAsync(string type, int limit)
        {
            var soldQuantities = await _context.OrderItems
                .GroupBy(oi => oi.ProductId)
                .Select(g => new { ProductId = g.Key, TotalSold = g.Sum(oi => oi.Quantity) })
                .ToDictionaryAsync(g => g.ProductId, g => g.TotalSold);

            return await _context.Products
                .Select(p => new InventoryProductDto
                {
                    ProductId = p.Id,
                    ProductName = p.Name,
                    TotalSold = soldQuantities.ContainsKey(p.Id) ? soldQuantities[p.Id] : 0
                })
                .OrderBy(p => type.ToLower() == "most" ? p.TotalSold : -p.TotalSold)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var totalRevenue = await _context.Orders
                .Where(o => o.Status == "Completed")
                .SumAsync(o => o.TotalAmount);

            var totalOrders = await _context.Orders.CountAsync();
            var completedOrders = await _context.Orders.CountAsync(o => o.Status == "Completed");
            var topProducts = await GetTopSellingProductsAsync(5);

            return new DashboardDto
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                CompletedOrders = completedOrders,
                TopProducts = topProducts
            };
        }

        public async Task<byte[]> ExportExcelReportAsync(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Báo cáo đơn hàng");

            worksheet.Cell(1, 1).Value = "Mã đơn hàng";
            worksheet.Cell(1, 2).Value = "Ngày đặt";
            worksheet.Cell(1, 3).Value = "Tổng tiền";
            worksheet.Cell(1, 4).Value = "Trạng thái";
            worksheet.Cell(1, 5).Value = "Sản phẩm";
            worksheet.Cell(1, 6).Value = "Số lượng";
            worksheet.Cell(1, 7).Value = "Giá";

            int row = 2;
            foreach (var order in orders)
            {
                if (order.OrderItems != null)
                {
                    foreach (var item in order.OrderItems)
                    {
                        worksheet.Cell(row, 1).Value = order.Id;
                        worksheet.Cell(row, 2).Value = order.OrderDate.ToString("yyyy-MM-dd");
                        worksheet.Cell(row, 3).Value = order.TotalAmount;
                        worksheet.Cell(row, 4).Value = order.Status ?? "Unknown";
                        worksheet.Cell(row, 5).Value = item.Product?.Name ?? "Unknown";
                        worksheet.Cell(row, 6).Value = item.Quantity;
                        worksheet.Cell(row, 7).Value = item.Price;
                        row++;
                    }
                }
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> ExportPdfReportAsync(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToListAsync();

            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            var boldFont = PdfFontFactory.CreateFont("Helvetica-Bold");
            document.Add(new Paragraph("Báo cáo đơn hàng")
                .SetFont(boldFont)
                .SetFontSize(20));
            document.Add(new Paragraph($"Từ {startDate:yyyy-MM-dd} đến {endDate:yyyy-MM-dd}")
                .SetFontSize(12));

            var table = new Table(7);
            table.AddHeaderCell("Mã đơn hàng");
            table.AddHeaderCell("Ngày đặt");
            table.AddHeaderCell("Tổng tiền");
            table.AddHeaderCell("Trạng thái");
            table.AddHeaderCell("Sản phẩm");
            table.AddHeaderCell("Số lượng");
            table.AddHeaderCell("Giá");

            foreach (var order in orders)
            {
                if (order.OrderItems != null)
                {
                    foreach (var item in order.OrderItems)
                    {
                        table.AddCell(order.Id.ToString());
                        table.AddCell(order.OrderDate.ToString("yyyy-MM-dd"));
                        table.AddCell(order.TotalAmount.ToString());
                        table.AddCell(order.Status ?? "Unknown");
                        table.AddCell(item.Product?.Name ?? "Unknown");
                        table.AddCell(item.Quantity.ToString());
                        table.AddCell(item.Price.ToString());
                    }
                }
            }

            document.Add(table);
            document.Close();
            return stream.ToArray();
        }
    }
}