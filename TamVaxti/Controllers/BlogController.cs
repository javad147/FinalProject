using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TamVaxti.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpPost]
        public async Task<IActionResult> PostComment(Comment comment)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userName = User.FindFirst(ClaimTypes.Name)?.Value;
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                comment.Name = userName;
                comment.Email = userEmail;
            }

            if (ModelState.IsValid)
            {
                comment.Date = DateTime.Now;
                await _blogService.AddCommentAsync(comment);
                TempData["messageType"] = "success";
                TempData["message"] = "Comment Posted Successfully.";
                return RedirectToAction("Details", new { id = comment.BlogId });
            }

            // Handle validation errors or return to the same view
            return View("Details", await _blogService.GetByIdAsync(comment.BlogId));
        }





        public async Task<IActionResult> Index()
        {
            // Fetch the count of all blogs
            ViewBag.Count = await _blogService.BlogCountAsync();

            // Fetch the main blog entries (e.g., for pagination or general display)
            

            // Create a view model to pass the data to the view
            var model = new BlogSidebarViewModel
            {
                Blogs = await _blogService.TakeAsync() ?? new List<Blog>(),  // Initialize with an empty list if null
                RecentBlogs = await _blogService.GetRecentBlogsAsync(5) ?? new List<Blog>(),
                PopularBlogs = await _blogService.GetPopularBlogsAsync(5) ?? new List<Blog>()
            };

            return View(model);
            
        }


        public async Task<IActionResult> Details(int id)
        {
            var blog = await _blogService.GetByIdAsync(id);

            if (blog == null)
            {
                return NotFound(); // Handle the case where the blog post is not found
            }

            // Update the view count (Hits) for the blog post
            await _blogService.UpdateViewCountAsync(id);

            // Pass the blog post to the view
            
            blog.Comments = (ICollection<Comment>)await _blogService.GetCommentsByBlogIdAsync(id);
            return View(blog);
        }

        [HttpGet]
        public async Task<IActionResult> ShowMore(int skip)
        {

            List<Blog> blogs = await _blogService.SkipTakeAsync(skip);
            return PartialView("_BlogsPartial", blogs);
        }
    }
}
