using TamVaxti.Helpers.Enums;
using TamVaxti.Models;
using TamVaxti.ViewModels.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace TamVaxti.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser> userManager, 
                                 SignInManager<AppUser> signInManager,
                                 RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
    }
}
