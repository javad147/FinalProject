using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.ViewModels.Sliders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TamVaxti.ViewComponents
{
    public class SliderViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;
        public SliderViewComponent(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var sliderData  = await _context.Sliders
                .Where(s => !s.SoftDeleted)
                .OrderBy(s => s.SliderNumber)
                .Select(s => new SliderVM
                {
                    Id = s.Id,
                    SliderTitle = s.SliderTitle,
                    SliderName = s.SliderName,
                    SliderDescription = s.SliderDescription,
                    Image = s.Image
                }).ToListAsync();

            return await Task.FromResult(View(sliderData)); 
        }
    }
}
