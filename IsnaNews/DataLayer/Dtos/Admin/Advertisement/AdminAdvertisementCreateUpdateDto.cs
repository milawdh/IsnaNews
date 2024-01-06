using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace DataLayer.Dtos.Admin.Advertisement
{
    public class AdminAdvertisementCreateUpdateDto
    {
        [BindProperty]
        [DisplayName("بنر")]
        public IFormFile MainBaner { get; set; }
        
        [BindProperty]
        [Description("It's Used to check if MainBaner image changed in Update")]
        [Required(ErrorMessage = "فیلد {0} خالی است")]
        [DisplayName("عکس اصلی")]
        public string? PerviuosMainBaner { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "فیلد {0} خالی است")]
        [DisplayName("لینک تبلیغ")]
        public string Link { get; set; }
    }
}
