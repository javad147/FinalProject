namespace TamVaxti.Models
{
    public class Slider : BaseEntity
    {
        public int Id { get; set; }
        public string SliderTitle { get; set; }
        public string SliderName { get; set; }
        public string SliderDescription { get; set; }
        public int SliderNumber { get; set; }
        public string Image { get; set; }
    }
}
