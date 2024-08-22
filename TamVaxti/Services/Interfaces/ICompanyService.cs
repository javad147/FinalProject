﻿using TamVaxti.Models;
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
        Task<List<CurrencyMaster>> GetCurrencyList();
        string GetCurrencySymbol();
        Task<decimal> GetCurrencyRate();
        Task GetCurrencyRateFromUrl();
        Task SetCurrencyRate(string symbol);
        void SetCurrencyRateCookie(string symbol);
    }
}
