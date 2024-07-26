namespace TamVaxti.ViewModels.AboutUs
{
    public class AboutUsTeamVM
    {

        public int Id { get; set; }


        public string Title { get; set; }
        public string Role { get; set; }
        public IFormFile ImageFile { get; set; }

        public string ImageName { get; set; }

    }
}
