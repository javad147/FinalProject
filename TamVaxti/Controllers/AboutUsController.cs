using TamVaxti.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TamVaxti.ViewModels.AboutUs;

namespace TamVaxti.Controllers
{
    public class AboutUsController : Controller
    {
        private readonly AppDbContext _context;

        public AboutUsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            var about = await _context.About.FirstOrDefaultAsync();
            var history = await _context.AboutUsHistory.Where(h => h.AboutId == about.Id).ToListAsync();
            var team = await _context.AboutUsTeam.Where(t => t.AboutId == about.Id).ToListAsync();

            var viewModel = new AboutUsVM
            {
                About = about,
                History = history,
                Team = team
            };

            return View(viewModel);
        }
    }
}
