using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Shared
{
    public class UserLoginDto
    {
        [BindProperty]
        [StringLength(32, ErrorMessage = "فیلد {0} نمیتواند بیش از 32 کارکتر باشد")]
        [Required(ErrorMessage = "فیلد {0} خالی است")]
        [DisplayName("نام")]
        public string UserName { get; set; } = "";
        [BindProperty]
        [Required(ErrorMessage = "فیلد {0} خالی است")]
        [DisplayName("رمز")]
        public string Password { get; set; } = "";
    }
}
