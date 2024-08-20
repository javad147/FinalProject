using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Accounts;
using TamVaxti.ViewModels.Blogs;
using TamVaxti.ViewModels.Company;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace TamVaxti.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _accessor;
        public CompanyService(AppDbContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _accessor = accessor;
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
        public async Task<List<CurrencyMaster>> GetCurrencyList()
        {
            return await _context.CurrencyMaster.ToListAsync();
        }
        public string GetCurrencySymbol()
        {
            dynamic symbol;
            if (_accessor.HttpContext.Request.Cookies["currency"] is not null)
            {
                symbol = _accessor.HttpContext.Request.Cookies["currency"];
            }
            else
            {
                var company = _context.Company.FirstOrDefault();
                symbol = company?.CurrencySymbol;
            }
            return symbol ?? "$"; 
        }
    }
}
