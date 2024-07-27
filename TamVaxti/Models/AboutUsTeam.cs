using TamVaxti.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TamVaxti.ViewModels
{
    public class AboutUsTeam
    {
        public int Id { get; set; }

        public string Image { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(500)]
        public string Role { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        [ForeignKey("About")]
        public int? AboutId { get; set; }
        public virtual About About { get; set; }
    }
}










//namespace TamVaxti.Models
//{
//    public class AboutUsTeam
//    {

//        public int Id { get; set; }
//        public string Image { get; set; }
//        public string Title { get; set; }
//        public string Role { get; set; }
//        public DateTime? CreatedOn { get; set; }
//        public DateTime? UpdatedOn { get; set; }
//        public int AboutId { get; set; }

//        public About About { get; set; } // Navigation property
//    }
//}
