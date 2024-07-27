using TamVaxti.Models;

namespace TamVaxti.Services.Interfaces
{
    public interface IDashboardService
    {
        public Task<List<Order>> GetAllProductsPurchasedInMonthAsync(DateTime date);
        public Task<List<Order>> GetLatestOrdersInMonthAsync(DateTime date, int take = 5);
    }
}
