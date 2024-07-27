using TamVaxti.ViewModels.AboutUs;

namespace TamVaxti.Services.Interfaces
{
    public interface IAboutUsService 
    {
        List<AboutUsVM> GetAllAboutUs();

        Task CreateAsync(AboutUsCreateVM model);

        Task<bool> UpdateAsync(int id, AboutUsCreateVM model);

        Task<AboutUsCreateVM> GetAboutUsByIdAsync(int id);

        //AboutUsVM GetAboutUsById(int id);
        //void AddAboutUs(AboutUsVM model);
        //void UpdateAboutUs(AboutUsVM model);
        //void DeleteAboutUs(int id);
    }
}
