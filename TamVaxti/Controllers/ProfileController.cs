using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TamVaxti.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            AppUser user = new();

            user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userProfile = new UserProfileVM
            {
                UserId = user.Id,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email
            };

            var shippingAddress = await _context.UserShippingAddress
                .FirstOrDefaultAsync(sa => sa.UserId == user.Id);

            if (shippingAddress != null)
            {
                userProfile.Flat = shippingAddress.Flat;
                userProfile.Address = shippingAddress.Address;
                userProfile.ZipCode = shippingAddress.ZipCode;
                userProfile.Country = shippingAddress.Country;
                userProfile.City = shippingAddress.City;
                userProfile.Region = shippingAddress.Region;
            }

            return View(userProfile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserProfileVM model)
        {

            AppUser user = new();
            user = await _userManager.FindByNameAsync(User.Identity.Name);

            var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email, user.Id);
            var existingUserByPhoneNumber = await _userManager.FindByPhoneNumberAsync(model.PhoneNumber, user.Id);

            if (existingUserByEmail != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Email Already exists.");
            }

            if (existingUserByPhoneNumber != null)
            {
                ModelState.AddModelError(nameof(model.PhoneNumber), "Phone Number already exists.");
            }
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }


            // Update user details
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            model.UserId = user.Id;
            await _userManager.UpdateAsync(user);

            var shippingAddress = await _context.UserShippingAddress
                .FirstOrDefaultAsync(sa => sa.UserId == model.UserId);

            if (shippingAddress == null)
            {
                shippingAddress = new UserShippingAddress
                {
                    UserId = model.UserId,
                    Flat = model.Flat,
                    Address = model.Address,
                    ZipCode = model.ZipCode,
                    Country = model.Country,
                    City = model.City,
                    Region = model.Region
                };
                _context.UserShippingAddress.Add(shippingAddress);
            }
            else
            {
                shippingAddress.Flat = model.Flat;
                shippingAddress.Address = model.Address;
                shippingAddress.ZipCode = model.ZipCode;
                shippingAddress.Country = model.Country;
                shippingAddress.City = model.City;
                shippingAddress.Region = model.Region;
                _context.UserShippingAddress.Update(shippingAddress);
            }

            await _context.SaveChangesAsync();

            TempData["messageType"] = "success";
            TempData["message"] = "User Profile Updated Successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
