using System.ComponentModel.DataAnnotations.Schema;

namespace TamVaxti.Models
{
    public class Blog : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Date {  get; set; } 
        public string Image {  get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public int Hits { get; set; }
        public int CommentCount { get; set; } = 0; // Add this property for comment count
        public string Uname { get; set; }
        public ICollection<Comment> Comments { get; set; } // Navigation property
    }
}
