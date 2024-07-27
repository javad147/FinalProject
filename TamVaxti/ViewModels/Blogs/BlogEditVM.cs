namespace TamVaxti.ViewModels.Blogs
{
    public class BlogEditVM
    {
        public int Id { get; set; }
        public string BlogTitle { get; set; }
        public string BlogDescription { get; set; }
        public string Image {  get; set; }
        public IFormFile ImageFile { get; set; }
        public DateTime Date { get; set; }
    }
}
