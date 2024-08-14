﻿using TamVaxti.Data;
using TamVaxti.Helpers.Extensions;
using TamVaxti.Models;
using TamVaxti.Services;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Company;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.AspNetCore.Authorization;
using TamVaxti.Helpers.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TamVaxti.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class CompanyController : Controller
    {
        // GET: CompanyController
        private readonly AppDbContext _context;
        private readonly ICompanyService _companyService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CompanyController(AppDbContext context,
                                        ICompanyService companyService,
                                        IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var company = await _companyService.GetFirstOrDefaultCompany();
            var companyViewModel = new CompanyVM();
            if (company != null)
            {
                companyViewModel.Id = company.Id;
                companyViewModel.LogoUrl = company.LogoUrl;
                companyViewModel.Name = company.Name;
                companyViewModel.AddressLineOne = company.AddressLineOne;
                companyViewModel.AddressLineTwo = company.AddressLineTwo;
                companyViewModel.Fax = company.Fax;
                companyViewModel.ContactNumber = company.ContactNumber;
                companyViewModel.AlternateNumber = company.AlternateNumber;
                companyViewModel.SupportEmailId = company.SupportEmailId;
                companyViewModel.EmailId = company.EmailId;
                companyViewModel.FacebookUrl = company.FacebookUrl;
                companyViewModel.InstagramUrl = company.InstagramUrl;
                companyViewModel.RssUrl = company.RssUrl;
                companyViewModel.XUrl = company.XUrl;
                companyViewModel.GoogleUrl = company.GoogleUrl;
                companyViewModel.ContactMapUrl = company.ContactMapUrl;
                companyViewModel.CurrencySymbol = company.CurrencySymbol;
            }

            var currency = await _companyService.GetCurrencyList();
            ViewBag.currency = currency.Select(b => new SelectListItem { Value = b.Symbol.ToString(), Text = $"{b.Code} ({b.Symbol})" }).ToList();
            return View(companyViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Index(int id, CompanyVM companyVm)
        {
            var currency = await _companyService.GetCurrencyList();
            ViewBag.currency = currency.Select(b => new SelectListItem { Value = b.Symbol.ToString(), Text = $"{b.Code} ({b.Symbol})" }).ToList();

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(companyVm);
                }
                Company company = await _companyService.ExistAsync(id);
                if (company != null)
                {
                    // If new file is uploaded and and existing url already exists then delete the existing and create the new one.
                    if (companyVm.ImageFile != null)
                    {
                        if (companyVm.LogoUrl != null)
                        {
                            RemoveFile(companyVm.LogoUrl);
                        }

                        companyVm.LogoUrl = await SaveFile(companyVm.ImageFile);

                    }
                    company = ModelMapper(company, companyVm);
                    await _companyService.UpdateAsync(company);
                }
                else
                {
                    if (companyVm.ImageFile != null)
                        companyVm.LogoUrl = await SaveFile(companyVm.ImageFile);
                    company = new Company();
                    company = ModelMapper(company, companyVm);
                    await _companyService.CreateAsync(company);
                }
                return View(companyVm);

            }
            catch
            {
                return View();
            }
        }

        private Company ModelMapper(Company company, CompanyVM companyVm)
        {
            company.Id = companyVm.Id;
            company.LogoUrl = companyVm.LogoUrl;
            company.Name = companyVm.Name;
            company.AddressLineOne = companyVm.AddressLineOne;
            company.AddressLineTwo = companyVm.AddressLineTwo;
            company.Fax = companyVm.Fax;
            company.ContactNumber = companyVm.ContactNumber;
            company.AlternateNumber = companyVm.AlternateNumber;
            company.SupportEmailId = companyVm.SupportEmailId;
            company.EmailId = companyVm.EmailId;
            company.FacebookUrl = companyVm.FacebookUrl;
            company.InstagramUrl = companyVm.InstagramUrl;
            company.RssUrl = companyVm.RssUrl;
            company.XUrl = companyVm.XUrl;
            company.GoogleUrl = companyVm.GoogleUrl;
            company.ContactMapUrl = companyVm.ContactMapUrl;
            company.CurrencySymbol = companyVm.CurrencySymbol;

            return company;
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var fileName = "";
            try
            {
                // Write Upload code over here.
                if (file != null)
                {
                    fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "img", fileName);
                    await FileExtensions.SaveFileToLocalAsync(file, filePath);
                }

            }
            catch { }
            return fileName;
        }

        private bool RemoveFile(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "img", fileName);
                FileExtensions.DeleteFileFromLocal(filePath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
