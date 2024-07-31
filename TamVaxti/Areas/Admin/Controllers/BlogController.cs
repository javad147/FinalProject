using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Blogs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using TamVaxti.Helpers.Enums;

namespace TamVaxti.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public BlogController(IBlogService blogService,
                              AppDbContext context, UserManager<AppUser> userManager)
        {
            _blogService = blogService;
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var blogs = await _blogService.GetAllOrderByAsync();
            return View(blogs);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogCreateVM blog)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            bool existblog = await _blogService.ExistAsync(blog.Title);

            if (existblog)
            {
                ModelState.AddModelError("Title", "This blog already exists");
                return View();
            }

            if (blog.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Blog");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + blog.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await blog.ImageFile.CopyToAsync(fileStream);
                }
                blog.Image = uniqueFileName;
            }
            AppUser user = new();
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            };
            blog.Uname= user.UserName;
            blog.Date = DateTime.Now;
            await _blogService.CreateAsync(blog);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id is null) return BadRequest();

            var blog = await _blogService.GetByIdAsync((int)id);

            if (blog is null) return NotFound();
            blog.Date = DateTime.Now;
            return View(new BlogDetailVM { Title = blog.Title, Description = blog.Description, CreateDate = blog.Date, Image = blog.Image });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();

            await _blogService.DeleteAsync((int)id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
            var blog = await _blogService.GetByIdAsync((int)id);

            if (blog is null)
            {
                return NotFound();
            }

            return View(new BlogEditVM { Id = blog.Id, BlogTitle = blog.Title, BlogDescription = blog.Description, Image = blog.Image }) ;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, BlogEditVM blogEdit)
        {
            if (!ModelState.IsValid) return View(blogEdit);

            if (id is null) return BadRequest();

            var blog = await _blogService.GetByIdAsync((int)id);

            if (blog is null) return NotFound();

            if (blogEdit.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/Blog");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + blogEdit.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await blogEdit.ImageFile.CopyToAsync(fileStream);
                }
                blogEdit.Image = uniqueFileName;
            }

            await _blogService.EditAsync(blog, blogEdit);
            return RedirectToAction(nameof(Index));
        }
    }
}
