using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TamVaxti.ViewModels.Orders;

namespace TamVaxti.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllOrdersPaginatedAsync(int page, int take = 10)
        {
            return await _context.Orders
                                //.Include(o => o.Users)
                                //.FromSqlRaw("SELECT o.*, u.FullName FROM Orders o LEFT JOIN AspNetUsers u ON o.CustomerId = u.Id")
                                .Include(o => o.Payments)
                                .Include(o => o.OrderProductDetails)
                                .ThenInclude(od => od.Product)
                                .Skip((page - 1) * take)
                                .Take(take)
                                .ToListAsync();

        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public dynamic GetOrderById(int id)
        {
            var order = _context.Orders
            .Include(o => o.Payments)
            .Include(o => o.OrderTracking)
            .Include(o => o.OrderProductDetails)
            .ThenInclude(od => od.Product)
            .Where(o => o.OrderId == id).FirstOrDefault();

            return order;
        }

        public dynamic GetDeliveryStatusItems()
        {
            var items = new List<string> {"Pending", "Processing", "Shipped", "In Transit", "Delivered", "Returned", "Cancelled" };
            return items;
        }

        public bool UpdateOrder(OrderEditVM model)
        {
            var order = _context.Orders.FirstOrDefault(e => e.OrderId == model.OrderId);
            if (order == null)
            {
                return false;
            }

            order.DeliveryStatus = model.DeliveryStatus;
            order.EstimatedDeliveryDate = model.EstimatedDeliveryDate;
            order.ActualDeliveryDate = model.ActualDeliveryDate;
            order.ExpectedDeliveryDate = model.ExpectedDeliveryDate;

            _context.Orders.Update(order);
            _context.Entry(order).Property(x => x.OrderNumber).IsModified = false;
            return _context.SaveChanges() > 0;
        }

        public bool DeleteOrder(int id)
        {
            var order = _context.Orders.FirstOrDefault(e => e.OrderId == id);
            if (order == null)
            {
                return false;
            }

            _context.Orders.Remove(order);
            return _context.SaveChanges() > 0;
        }

        public async Task<List<OrderTracking>> GetTrackingDetails(int id)
        {
            var res = await _context.OrderTracking.Where(e => e.OrderId == id).OrderByDescending(e => e.TrackingId).ToListAsync();
            return res;
        }

        public bool AddTracking(OrderTracking model)
        {
            _context.OrderTracking.Add(model);
            return _context.SaveChanges() > 0;
        }
    }
}
