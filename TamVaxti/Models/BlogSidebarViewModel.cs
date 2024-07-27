using System.ComponentModel.DataAnnotations.Schema;
namespace TamVaxti.Models
{
    public class BlogSidebarViewModel
    {
        public List<Blog> Blogs { get; set; }  // Ensure this property exists
        public List<Blog> RecentBlogs { get; set; }  // Example additional property
        public List<Blog> PopularBlogs { get; set; }
    }
}