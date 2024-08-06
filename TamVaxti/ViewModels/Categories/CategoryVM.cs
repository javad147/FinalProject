namespace TamVaxti.ViewModels.Categories
{
    public class CategoryVM
    {
        public int Id { get; set; } 
        public string Name { get; set; }

        public string CategoryImage { get; set; }

        public bool IsPublished { get; set; }

        public bool ShowInMenu { get; set; }
        public bool ShowOnCategoryHomePage { get; set; }

        public bool ShowOnTrendingHomePage { get; set; }

    }
}
