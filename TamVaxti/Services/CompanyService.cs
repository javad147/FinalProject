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
using Newtonsoft.Json.Linq;

namespace TamVaxti.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _accessor;
        private readonly IHttpClientFactory _httpClientFactory;
        public CompanyService(AppDbContext context, IHttpContextAccessor accessor, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _accessor = accessor;
            _httpClientFactory = httpClientFactory;
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
            return symbol ?? "₼"; 
        }

        public async Task<decimal> GetCurrencyRate()
        {
            decimal rate;
            if (_accessor.HttpContext.Request.Cookies["currencyRate"] is not null)
            {
                var currrate = _accessor.HttpContext.Request.Cookies["currencyRate"];
                rate = decimal.Parse(currrate);
            }
            else
            {
                rate = 1m;
            }
            return rate;
        }

        public async Task GetCurrencyRateFromUrl()
        {
            var url = "https://api.exchangerate-api.com/v4/latest/AZN";
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync(url);
            var exchangeRates = JObject.Parse(response);

            decimal usdRate = exchangeRates["rates"]["USD"].Value<decimal>();
            decimal eurRate = exchangeRates["rates"]["EUR"].Value<decimal>();
            decimal aznRate = exchangeRates["rates"]["AZN"].Value<decimal>();


            var currencies = new Dictionary<string, decimal>
            {
                { "$", usdRate },
                { "€", eurRate },
                { "₼", aznRate }
            };

            var jsonCurrencyInfo = JsonConvert.SerializeObject(currencies);

            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7), 
                HttpOnly = true,
                Secure = true
            };

            _accessor.HttpContext.Response.Cookies.Append("CurrencyInfo", jsonCurrencyInfo, cookieOptions);
        }

        public async Task SetCurrencyRate(string symbol)
        {
            if (_accessor.HttpContext.Request.Cookies["CurrencyInfo"] is not null)
            {
                this.SetCurrencyRateCookie(symbol);
            }
            else
            {
                await this.GetCurrencyRateFromUrl();
                this.SetCurrencyRateCookie(symbol);
            }
        }

        public void SetCurrencyRateCookie(string symbol)
        {
            if (_accessor.HttpContext.Request.Cookies["CurrencyInfo"] is not null)
            {
                var currency = _accessor.HttpContext.Request.Cookies["CurrencyInfo"];
                var currencyInfo = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(currency);
                var currencyRate = currencyInfo[symbol];

                var jsonCurrencyInfo = JsonConvert.SerializeObject(currencyRate);

                var cookieOptions = new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    Secure = true
                };

                _accessor.HttpContext.Response.Cookies.Append("CurrencyRate", jsonCurrencyInfo, cookieOptions);
            }
        }
    }
}
