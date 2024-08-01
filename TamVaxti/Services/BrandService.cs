using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels;
using TamVaxti.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bogus.DataSets;

namespace TamVaxti.Services
{

    public class BrandService : IBrandService
    {
        private readonly AppDbContext _context;

        public BrandService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Brand brand)
        {
            _context.Brand.Add(brand);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Brand brand)
        {
            _context.Brand.Remove(brand);
            await _context.SaveChangesAsync();
        }

        public async Task<Brand> GetByIdAsync(int id)
        {
            return await _context.Brand.SingleOrDefaultAsync(m => m.Id == id);
        }
        public async Task<Brand> GetByNameAsync(string name)
        {
            return await _context.Brand.SingleOrDefaultAsync(m => m.Name == name);
        }
        public async Task<List<Brand>> GetAllActiveAsync()
        {
            return await _context.Brand.OrderByDescending(b=>b.Id).Where(m => !m.SoftDeleted).ToListAsync();
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            return await _context.Brand.OrderByDescending(b => b.Id).ToListAsync();
        }

        public async Task UpdateAsync(Brand brand)
        {
            _context.Brand.Update(brand);
           await _context.SaveChangesAsync();
        }
    }
}
