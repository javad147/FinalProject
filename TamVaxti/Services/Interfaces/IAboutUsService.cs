using TamVaxti.ViewModels.AboutUs;

namespace TamVaxti.Services.Interfaces
{
    public interface IAboutUsService 
    {
        List<AboutUsVM> GetAllAboutUs();

        Task CreateAsync(AboutUsCreateVM model);

        Task UpdateAsync(int id, AboutUsCreateVM model);

        Task<AboutUsCreateVM> GetAboutUsByIdAsync(int id);
    }
}
