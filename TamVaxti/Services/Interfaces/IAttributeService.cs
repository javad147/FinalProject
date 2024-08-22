using TamVaxti.Models;

namespace TamVaxti.Services.Interfaces
{
    public interface IAttributeService
    {
        Task CreateAsync(Attributes attribute);

        Task<List<Attributes>> GetAllAsync(string searchString);

        Task<Attributes> GetByIdAsync(int id);

        Task DeleteAsync(Attributes attribute);

        Task UpdateAsync(Attributes attribute);


        Task<Attributes> FindByNameAsync(string name);

    }
}
