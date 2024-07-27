namespace TamVaxti.ViewModels.SubCategory
{
    public class SubCategoryVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SubcategoryImage { get; set; } 
        public int CategoryId { get; set; }

        public bool IsPublished { get; set; }

        public string CategoryName { get; set; }
    }
}
