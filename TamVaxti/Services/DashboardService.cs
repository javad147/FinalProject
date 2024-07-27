using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TamVaxti.Services
{
    public class DashboardService : IDashboardService
    {
        private AppDbContext _context;
        public DashboardService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Order>> GetAllProductsPurchasedInMonthAsync(DateTime date)
        {
            DateTime startDate = new DateTime(date.Year, date.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            return await _context.Orders.Include(o => o.Payments)
                                .Include(o => o.OrderProductDetails)
                                .ThenInclude(od => od.SKU)
                                .Where(o => o.DeliveryStatus == "Delivered" && o.OrderDate >= startDate && o.OrderDate < endDate)
                                .ToListAsync();

        }

        public async Task<List<Order>> GetLatestOrdersInMonthAsync(DateTime date, int take = 5)
        {
            DateTime startDate = new DateTime(date.Year, date.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            return await _context.Orders.Include(o => o.Payments)
                                .Include(o => o.OrderProductDetails)
                                .ThenInclude(od => od.SKU)
                                .Where(o => o.OrderDate >= startDate && o.OrderDate < endDate)
                                .OrderByDescending(o => o.OrderId)
                                .Take(take)
                                .ToListAsync();

        }

        /*public async Task<List<dynamic>> GetLastMonthRevenue()
        {
            var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1);
            var endDate = startDate.AddMonths(1);

            var res = await _context.Orders
                .Where(o => o.DeliveryStatus == "Delivered" && o.OrderDate >= startDate && o.OrderDate < endDate)
                .GroupBy(o => o.OrderDate.Day)
                .Select(g => new
                {
                    Day = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(r => r.Day)
                .ToListAsync();

            return res;
        }*/
    }
}
