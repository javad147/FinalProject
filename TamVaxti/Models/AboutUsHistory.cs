using TamVaxti.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TamVaxti.ViewModels
{
    public class AboutUsHistory
    {
        public int Id { get; set; }

        public string Image { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        [ForeignKey("About")]
        public int? AboutId { get; set; }
        public virtual About About { get; set; }
    }
}



//namespace FiorelloFrontBackDB.Models
//{
//    public class AboutUsHistory
//    {
//        public int Id { get; set; }
//        public string Image { get; set; }
//        public string Title { get; set; }
//        public string Description { get; set; }
//        public DateTime? CreatedOn { get; set; }
//        public DateTime? UpdatedOn { get; set; }
//        public int AboutId { get; set; }
//        public About About { get; set; } // Navigation property

//    }

//}
