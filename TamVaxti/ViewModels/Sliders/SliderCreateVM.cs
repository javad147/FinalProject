using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TamVaxti.ViewModels.Sliders
{
    public class SliderCreateVM
    {
        [Required(ErrorMessage = "Slider Title is required.")]
        public string SliderTitle { get; set; }

        //[Required(ErrorMessage = "Slider Name is required.")]
        public string SliderName { get; set; }

        [Required(ErrorMessage = "Slider Description is required.")]
        public string SliderDescription { get; set; }

        //[Required(ErrorMessage = "Slider Number is required.")]
        public int SliderNumber { get; set; }

        [Required(ErrorMessage = "Image is required.")]
        public IFormFile Image { get; set; }
    }
}
