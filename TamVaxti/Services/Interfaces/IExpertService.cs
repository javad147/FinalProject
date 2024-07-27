using TamVaxti.Models;

namespace TamVaxti.Services.Interfaces
{
    public interface IExpertService
    {
        Task<List<Expert>> GetAllAsync();
    }
}
