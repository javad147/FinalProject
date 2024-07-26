using TamVaxti.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace TamVaxti.Models
{
    public class About
    {
        public int Id { get; set; }

        public string Image { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public virtual ICollection<AboutUsHistory> AboutUsHistories { get; set; }
        public virtual ICollection<AboutUsTeam> AboutUsTeams { get; set; }
    }
}


//namespace FiorelloFrontBackDB.Models
//{
//    public class About
//    {

//        public int Id { get; set; }
//        public string Image { get; set; }
//        public string Title { get; set; }
//        public string Description { get; set; }
//        public DateTime? CreatedOn { get; set; }
//        public DateTime? UpdatedOn { get; set; }

//        public ICollection<AboutUsHistory> AboutUsHistory { get; set; }
//        public ICollection<AboutUsTeam> AboutUsTeam { get; set; }
//    }
//}

