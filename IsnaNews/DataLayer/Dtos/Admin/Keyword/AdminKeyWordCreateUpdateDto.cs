using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Admin.Keyword
{
    public class AdminKeyWordCreateUpdateDto
    {
        [BindProperty]
        [StringLength(16, ErrorMessage = "فیلد {0} نمیتواند بیش از 16 کارکتر باشد")]
        [Required(ErrorMessage = "فیلد {0} خالی است",AllowEmptyStrings =false)]
        [Display(Name = "نام")]
        public string Body { get; set; }
    }
}
