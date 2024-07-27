using TamVaxti.Models;
using TamVaxti.ViewModels.Categories;
using TamVaxti.ViewModels.Sliders;

namespace TamVaxti.Services.Interfaces
{
    public interface ISliderInstaService
    {

        Task<List<SliderVM>> GetAllOrderByDescendingAsync();

        Task<List<InstaSlider>> GetAllAsync();
    }
}
