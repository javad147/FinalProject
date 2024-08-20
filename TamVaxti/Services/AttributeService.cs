using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TamVaxti.Services
{
    public class AttributeService : IAttributeService
    {
        private readonly AppDbContext _context;

        public AttributeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Attributes attribute)
        {
            // Add the attribute to the database
            await _context.Attributes.AddAsync(attribute);
            // Save changes to persist data
            await _context.SaveChangesAsync();
        }

        public async Task<List<Attributes>> GetAllAsync(string searchString)
        {
            IQueryable<Attributes> query = _context.Attributes
                                       .Where(a => !a.SoftDeleted)
                                       .OrderByDescending(a => a.Id);

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(a => a.Name.Contains(searchString));
            }

            return await query.ToListAsync();
        }


        public async Task<Attributes> GetByIdAsync(int id)
        {
            return await _context.Attributes.FindAsync(id);
        }

        public async Task DeleteAsync(Attributes attribute)
        {
            _context.Attributes.Remove(attribute);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Attributes attribute)
        {
            _context.Attributes.Update(attribute);
            await _context.SaveChangesAsync();
        }


    }
}
