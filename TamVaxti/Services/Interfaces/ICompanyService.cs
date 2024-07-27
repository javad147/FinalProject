using TamVaxti.Models;
using TamVaxti.ViewModels.Accounts;
using TamVaxti.ViewModels.Blogs;

namespace TamVaxti.Services.Interfaces
{
    public interface ICompanyService
    {
        Task<Company> GetFirstOrDefaultCompany();
        Task<Company> ExistAsync(int id);
        Task CreateAsync(Company companyCreateVM);
        Task UpdateAsync(Company company);
    }
}
