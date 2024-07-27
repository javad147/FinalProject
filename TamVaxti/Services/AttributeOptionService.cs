using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;

namespace TamVaxti.Services
{
    public class AttributeOptionService : IAttributeOptionService
    {

        private readonly AppDbContext _context;

        public AttributeOptionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(AttributeOption attributeOption)
        {
            // Add the new attribute option to the database context
            _context.AttributeOptions.Add(attributeOption);

            // Save changes to the database
            await _context.SaveChangesAsync();
        }


        public async Task<AttributeOption> GetByIdAsync(long id)
        {
            return await _context.AttributeOptions.FindAsync(id);
        }

        public async Task DeleteAsync(AttributeOption attribute)
        {
            _context.AttributeOptions.Remove(attribute);
            await _context.SaveChangesAsync();
        }
    }
}
