using TamVaxti.Helpers.Enums;
using TamVaxti.Models;
using TamVaxti.ViewModels.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TamVaxti.Helpers;
using TamVaxti.Services.Interfaces;
using System.Net.Mail;
using TamVaxti.Helpers.Extensions;
using TamVaxti.Data;
namespace TamVaxti.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICompanyService _companyService;
        private readonly AppDbContext _context;

        public AccountController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 ICompanyService companyService,
                                 AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _companyService = companyService;
            _context = context;
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

            

            var existingUserByUsername = await _userManager.FindByNameAsync(request.UserName);
            var existingUserByEmail = await _userManager.FindByEmailAsync(request.Email);
            var existingUserByPhoneNumber = await _userManager.FindByPhoneNumberAsync(request.PhoneNumber);

            if (existingUserByUsername != null)
            {
                ModelState.AddModelError(nameof(request.UserName), "User Name already exists.");
            }

            if (existingUserByEmail != null)
            {
                ModelState.AddModelError(nameof(request.Email), "Email Already exists.");
            }

            if (existingUserByPhoneNumber != null)
            {
                ModelState.AddModelError(nameof(request.PhoneNumber), "Phone Number already exists.");
            }
            if (!ModelState.IsValid)
            {
                return View(request);
            }
            AppUser user = new()
            {
                Email = request.Email,
                UserName = request.UserName,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    if(item.Description.Contains("Password"))
                    {
                        ModelState.AddModelError(nameof(request.Password), item.Description);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, item.Description);
                    }
                  
                }
                return View(request);
            }

            await _userManager.AddToRoleAsync(user, nameof(Roles.Member));
            // Add shipping address.
            UserShippingAddress shippingAddress = new()
            {
                UserId = user.Id,
                Flat = request.Flat ?? "",
                Address = request.Address,
                ZipCode = request.ZipCode,
                Country = request.Country,
                City = request.City,
                Region = request.Region
            };
            _context.UserShippingAddress.Add(shippingAddress);
            await _context.SaveChangesAsync();

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

            var company = await _companyService.GetFirstOrDefaultCompany();

            var verifyemail = new
            {
                Name = request.UserName,
                Link = confirmationLink,
                Company = company
            };
            string emailBody = GetEmailBodyFromFile($"{Directory.GetCurrentDirectory()}/wwwroot/emailtemplate/emailverify.html", verifyemail);

            await EmailHelper.SendEmailAsync(request.Email, request.UserName, "Confirm your email", emailBody, "verify.jpg");
            TempData["SuccessMessage"] = "Registration successful. Please check your email to confirm your account.";

            return View();
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

            if (existUser is null)
            {
                ModelState.AddModelError(string.Empty, "Login failed");
                return View();
            }

            if (!await _userManager.IsEmailConfirmedAsync(existUser))
            {
                ModelState.AddModelError(string.Empty, "You need to confirm your email to log in.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(existUser, request.Password, false,false); 

            if (!result.Succeeded)
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
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> CreateRoles()
        {
            foreach (var role in Enum.GetValues(typeof(Roles)))
            {
                if (!await _roleManager.RoleExistsAsync(nameof(role)))
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

                await EmailHelper.SendEmailAsync(model.Email, existUser.UserName, "Password Reset Request", emailBody, "resetpasswords.jpg");

                TempData["SuccessMessage"] = "Password Reset Email Sent";
                return RedirectToAction("SignIn", "Account");
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
                    return RedirectToAction("SignIn", "Account");

                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);

        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                TempData["ErrorMessage"] = "Invalid token or user ID.";
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Invalid user ID.";
                return RedirectToAction("Index", "Home");
            }

            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                TempData["ErrorMessage"] = "Your email is already confirmed. You can now log in.";
                return RedirectToAction("SignIn", "Account");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                var company = await _companyService.GetFirstOrDefaultCompany();

                var verifyemail = new
                {
                    Name = user.UserName,
                    Link = "",
                    Company = company
                };
                string emailBody = GetEmailBodyFromFile($"{Directory.GetCurrentDirectory()}/wwwroot/emailtemplate/welcome.html", verifyemail);

                await EmailHelper.SendEmailAsync(user.Email, user.UserName, "Email Confirmed", emailBody, "welcome.jpg");

                TempData["SuccessMessage"] = "Email confirmed successfully. You can now log in.";
                return RedirectToAction("SignIn", "Account");
            }

            TempData["ErrorMessage"] = "Error confirming your email. Please contact support.";
            return RedirectToAction("Index", "Home");
        }

    }
}
