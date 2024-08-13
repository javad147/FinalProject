﻿using TamVaxti.Helpers.Enums;
using TamVaxti.Models;
using TamVaxti.ViewModels.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TamVaxti.Helpers;
using TamVaxti.Services.Interfaces;
using System.Net.Mail;

namespace TamVaxti.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICompanyService _companyService;

        public AccountController(UserManager<AppUser> userManager, 
                                 SignInManager<AppUser> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 ICompanyService companyService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _companyService = companyService;
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(RegisterVM request)
        {
            if(!ModelState.IsValid) return View(request);

            AppUser user = new()
            {
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
            };

            var result = await _userManager.CreateAsync(user,request.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors) 
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
                return View(request);
            }

            await _userManager.AddToRoleAsync(user, nameof(Roles.Member));

            return RedirectToAction("Index","Home");   
        }


        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(LoginVM request)
        {
            if (!ModelState.IsValid) return View();

            var existUser = await _userManager.FindByEmailAsync(request.EmailOrUsername);

            if (existUser is null)
            {
                existUser = await _userManager.FindByNameAsync(request.EmailOrUsername);
               // return RedirectToAction("Index", "Home");
            }

            if(existUser is null)
            {
                ModelState.AddModelError(string.Empty, "Login failed");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(existUser, request.Password, false,false); 

            if(!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Login failed");
                return View();
            }

            return RedirectToAction("Index", "Home");
        } 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");
        }


        [HttpGet]
        public async Task<IActionResult> CreateRoles()
        {
            foreach (var role in Enum.GetValues(typeof(Roles)))
            {
                if(!await _roleManager.RoleExistsAsync(nameof(role)))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = role.ToString() });
                } 
            }
            return Ok();
        }

        [HttpGet]
        public ActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ResetPassword model)
        {
            if (ModelState.IsValid)
            {
                var existUser = await _userManager.FindByEmailAsync(model.Email);

                if (existUser == null)
                {
                    TempData["ErrorMessage"] = "User does not exist with this email";
                    return View();
                }

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(existUser);
                var resetLink = Url.Action("ResetPassword", "Account",
                    new { token = resetToken, email = model.Email }, Request.Scheme);

                var company = await _companyService.GetFirstOrDefaultCompany();

                var forgotPassword = new ForgotPasswordEmail
                {
                    Name = existUser.UserName,
                    Link = resetLink,
                    Company = company
                };

                string emailBody = GetEmailBodyFromFile($"{Directory.GetCurrentDirectory()}/wwwroot/emailtemplate/forgotpassword.html", forgotPassword);

                await EmailHelper.SendEmailAsync(model.Email, existUser.UserName, "Password Reset Request", emailBody);

                TempData["SuccessMessage"] = "Password Reset Email Sent";
                return View();
            }

            return View(model);
        }

        public string GetEmailBodyFromFile(string filePath, dynamic forgotPassword)
        {
            string emailBody = System.IO.File.ReadAllText(filePath);
            emailBody = emailBody.Replace("{{Name}}", forgotPassword.Name);
            emailBody = emailBody.Replace("{{Link}}", forgotPassword.Link);
            emailBody = emailBody.Replace("{{fbLink}}", forgotPassword.Company.FacebookUrl);
            emailBody = emailBody.Replace("{{insLink}}", forgotPassword.Company.InstagramUrl);
            emailBody = emailBody.Replace("{{twLink}}", forgotPassword.Company.XUrl);
            emailBody = emailBody.Replace("{{gpLink}}", forgotPassword.Company.GoogleUrl);
            return emailBody;
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid password reset token.");
            }

            var model = new ConfirmResetPassword { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ConfirmResetPassword model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    return View(model);
                }

                var isValidToken = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword", model.Token);

                if (!isValidToken)
                {
                    ModelState.AddModelError(string.Empty, "Invalid token.");
                    return View(model);
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Password Reset Successfully";
                    return View();
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);

        }

    }
}
