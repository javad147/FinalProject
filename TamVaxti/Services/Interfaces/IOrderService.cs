using TamVaxti.Models;
using TamVaxti.ViewModels.Orders;

namespace TamVaxti.Services.Interfaces
{
    public interface IOrderService
    {
        public Task<List<Order>> GetAllOrdersPaginatedAsync(int page, int take = 10);
        public Task<int> GetCountAsync();
        public dynamic GetOrderById(int id);
        public dynamic GetDeliveryStatusItems();
        public bool UpdateOrder(OrderEditVM model);
        public bool DeleteOrder(int id);
        public Task<List<OrderTracking>> GetTrackingDetails(int id);
        public bool AddTracking(OrderTracking model);
    }
}
