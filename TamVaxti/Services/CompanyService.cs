using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TamVaxti.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly AppDbContext _context;
        public CompanyService(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Company company)
        {
            await _context.Company.AddAsync(company);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Company company)
        {
            _context.Company.Update(company);
            await _context.SaveChangesAsync();
        }

        public async Task<Company> ExistAsync(int id)
        {
            return await _context.Company.FindAsync(id);
        }

        public async Task<Company> GetFirstOrDefaultCompany()
        {
            return await _context.Company.FirstOrDefaultAsync();
        }
    }
}
