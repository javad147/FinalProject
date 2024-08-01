using TamVaxti.Models;
using TamVaxti.ViewModels.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using TamVaxti.Helpers.Enums;

namespace TamVaxti.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<UserRoleVM> userRoles = new();
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Member"))
                {
                    continue;
                }
                string rolesStr = string.Join(",", roles.ToArray());

                userRoles.Add(new UserRoleVM
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = rolesStr,
                    ProfileImageUrl = user.ProfileImageUrl
                });
            }
            return View(userRoles);
        }

        private async Task<IEnumerable<SelectListItem>> GetRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name
            });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new UserCreateVM
            {
                Roles = await GetRolesAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateVM model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FullName = model.FullName,
                    PasswordHash = model.PasswordHash,
                };

                if (model.ProfileImageUrl != null)
                {
                    user.ProfileImageUrl = await SaveProfileImage(model.ProfileImageUrl);
                }

                var result = await _userManager.CreateAsync(user, model.PasswordHash);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.SelectedRole))
                    {
                        await _userManager.AddToRoleAsync(user, model.SelectedRole);
                    }
                    TempData["messageType"] = "success";
                    TempData["message"] = "User Created Successfully.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            model.Roles = await GetRolesAsync();
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            var viewModel = new UserEditVM
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                SelectedRole = userRoles.FirstOrDefault(),
                Roles = roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList(),
                ExistingProfileImageUrl = user.ProfileImageUrl
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEditVM model)
        {
            if (!ModelState.IsValid)
            {
                var roles = await _roleManager.Roles.ToListAsync();
                model.Roles = roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList();
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName;
            user.UserName = model.UserName;
            user.Email = model.Email;

            if (!string.IsNullOrEmpty(model.Password))
            {
                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded)
                {
                    foreach (var error in removePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    var roles = await _roleManager.Roles.ToListAsync();
                    model.Roles = roles.Select(r => new SelectListItem
                    {
                        Value = r.Name,
                        Text = r.Name
                    }).ToList();
                    return View(model);
                }

                var addPasswordResult = await _userManager.AddPasswordAsync(user, model.Password);
                if (!addPasswordResult.Succeeded)
                {
                    foreach (var error in addPasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    var roles = await _roleManager.Roles.ToListAsync();
                    model.Roles = roles.Select(r => new SelectListItem
                    {
                        Value = r.Name,
                        Text = r.Name
                    }).ToList();
                    return View(model);
                }
            }

            if (model.ProfileImageUrl != null && model.ProfileImageUrl.Length > 0)
            {
                if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                {
                    var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/users", Path.GetFileName(user.ProfileImageUrl));
                    if (System.IO.File.Exists(existingFilePath))
                    {
                        System.IO.File.Delete(existingFilePath);
                    }
                }

                var fileName = Path.GetFileNameWithoutExtension(model.ProfileImageUrl.FileName);
                var extension = Path.GetExtension(model.ProfileImageUrl.FileName);
                var newFileName = $"{Guid.NewGuid()}{extension}";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/users", newFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.ProfileImageUrl.CopyToAsync(stream);
                }

                user.ProfileImageUrl = newFileName;
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                var roles = await _roleManager.Roles.ToListAsync();
                model.Roles = roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList();
                return View(model);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!string.IsNullOrEmpty(model.SelectedRole))
            {
                var roleExists = await _roleManager.RoleExistsAsync(model.SelectedRole);
                if (!roleExists)
                {
                    ModelState.AddModelError(string.Empty, "Selected role does not exist.");
                    var roles = await _roleManager.Roles.ToListAsync();
                    model.Roles = roles.Select(r => new SelectListItem
                    {
                        Value = r.Name,
                        Text = r.Name
                    }).ToList();
                    return View(model);
                }

                await _userManager.AddToRoleAsync(user, model.SelectedRole);
            }

            TempData["messageType"] = "success";
            TempData["message"] = "User Updated Successfully.";
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(new UserDeleteVM { Id = user.Id, UserName = user.UserName });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["messageType"] = "error";
                TempData["message"] = "User Deleted Successfully.";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Customer()
        {
            List<UserRoleVM> userRoles = new();
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("SuperAdmin") || roles.Contains("Admin"))
                {
                    continue;
                }
                string rolesStr = string.Join(",", roles.ToArray());

                userRoles.Add(new UserRoleVM
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = rolesStr,
                    ProfileImageUrl = user.ProfileImageUrl
                });
            }
            return View(userRoles);
        }


        private async Task<string> SaveProfileImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var extension = Path.GetExtension(file.FileName);
                var newFileName = $"{Guid.NewGuid()}{extension}";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/users", newFileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return newFileName;
            }
            return null;
        }

    }
}
