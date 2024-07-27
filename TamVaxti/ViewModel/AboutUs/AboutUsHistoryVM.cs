namespace TamVaxti.ViewModels.AboutUs
{
    public class AboutUsHistoryVM
    {
        public int Id { get; set; }
   
        public string Title { get; set; }
        public string Description { get; set; }
        public IFormFile ImageFile { get; set; }

        public string ImageName { get; set; }
    }
}
