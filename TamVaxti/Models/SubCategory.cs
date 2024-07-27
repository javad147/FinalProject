using System.ComponentModel.DataAnnotations;

namespace TamVaxti.Models
{
    public class SubCategory :BaseEntity
    {
        [Required]
        public string Name { get; set; }

        public string SubcategoryImage { get; set; }

        [Required]
        public int CategoryId { get; set; }


        public Category Category { get; set; }

        public bool IsPublished { get; set; }

    }
}
