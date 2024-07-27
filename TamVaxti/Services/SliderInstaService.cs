using TamVaxti.Data;
using TamVaxti.Models;
using TamVaxti.Services.Interfaces;
using TamVaxti.ViewModels.Sliders;
using Microsoft.EntityFrameworkCore;

namespace TamVaxti.Services
{
    public class SliderInstaService : ISliderInstaService
    {
        private readonly AppDbContext _context;

        public SliderInstaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SliderVM>> GetAllOrderByDescendingAsync()
        {
            List<Slider> sliders = await _context.Sliders
                                                 .Where(s => !s.SoftDeleted)
                                                 .OrderByDescending(s => s.Id)
                                                 .ToListAsync();

            return sliders.Select(s => new SliderVM
            {
                Id = s.Id,
                SliderTitle = s.SliderTitle,
                SliderName = s.SliderName,
                SliderDescription = s.SliderDescription,
                Image = s.Image
            }).ToList();
        }

        public async Task<List<InstaSlider>> GetAllAsync()
        {
            return await _context.InstaSliders.ToListAsync();
        }
    }
}
