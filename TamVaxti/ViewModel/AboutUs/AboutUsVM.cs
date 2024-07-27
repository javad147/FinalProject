using TamVaxti.Models;

namespace TamVaxti.ViewModels.AboutUs
{
    public class AboutUsVM
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public About About { get; set; }
        public List<AboutUsHistory> History { get; set; }
        public List<AboutUsTeam> Team { get; set; }

    }
}
