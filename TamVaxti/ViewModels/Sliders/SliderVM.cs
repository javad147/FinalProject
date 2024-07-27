using System.ComponentModel.DataAnnotations;

namespace TamVaxti.ViewModels.Sliders
{
    public class SliderVM
    {
        public int Id { get; set; }
        public string SliderTitle { get; set; }
        public string SliderName { get; set; }
        public string SliderDescription { get; set; }
        public string Image { get; set; }
    }

}
