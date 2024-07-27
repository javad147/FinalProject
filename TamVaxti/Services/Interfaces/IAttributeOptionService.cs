using TamVaxti.Models;

namespace TamVaxti.Services.Interfaces
{
    public interface IAttributeOptionService 
    {
        Task CreateAsync(AttributeOption attributeOption);

        Task<AttributeOption> GetByIdAsync(long id);

        Task DeleteAsync(AttributeOption attribute);


    }
}
