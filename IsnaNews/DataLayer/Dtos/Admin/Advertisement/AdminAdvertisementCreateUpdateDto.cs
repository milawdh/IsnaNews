using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Admin.Advertisement
{
    public class AdminAdvertisementCreateUpdateDto
    {
        [BindProperty]
        [Required(ErrorMessage = "فیلد {0} خالی است")]
        [DisplayName("بنر")]
        public string MainBaner { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "فیلد {0} خالی است")]
        [DisplayName("لینک تبلیغ")]
        public string Link { get; set; }
    }
}
