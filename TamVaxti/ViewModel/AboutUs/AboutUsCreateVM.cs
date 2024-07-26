using TamVaxti.Models;

namespace TamVaxti.ViewModels.AboutUs
{


    public class AboutUsCreateVM
    {
        public int Id { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile ImageFile { get; set; }

        public string ImageName { get; set; }
        public List<AboutUsHistoryVM> AboutHistory { get; set; }
        public List<AboutUsTeamVM> AboutUsTeam { get; set; }
    }



    //public class AboutUsCreateVM
    //{
    //    public string Title { get; set; }
    //    public string Description { get; set; }
    //    public IFormFile ImageFile { get; set; }
    //    public ICollection<AboutUsHistoryVM> AboutHistory { get; set; }
    //    public ICollection<AboutUsTeamVM> AboutUsTeam { get; set; }

    //}

    //public class AboutUsHistoryVM
    //{
    //    public string Title { get; set; }
    //    public string Description { get; set; }
    //    public IFormFile ImageFile { get; set; }

    //}
    //public class AboutUsTeamVM
    //{
    //    public string Title { get; set; }
    //    public string Role { get; set; }
    //    public IFormFile ImageFile { get; set; }

    //}
}
