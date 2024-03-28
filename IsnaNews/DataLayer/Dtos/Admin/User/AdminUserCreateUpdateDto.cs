using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DataLayer.Dtos.Admin.User
{
    public class AdminUserCreateUpdateDto
    {
        [BindProperty]
        [StringLength(16, ErrorMessage = "فیلد {0} نمیتواند بیش از 32 کارکتر باشد")]
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [Display(Name = "نام")]
        public string Name { get; set; } = null!;

        [BindProperty]
        [StringLength(16, ErrorMessage = "فیلد {0} نمیتواند بیش از 32 کارکتر باشد")]
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [Display(Name ="تلفن")]

        public string Tell { get; set; } = null!;

        [BindProperty]
        [StringLength(32, ErrorMessage = "فیلد {0} نمیتواند بیش از 32 کارکتر باشد")]
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [Display(Name ="نام کاربری")]
        public string UserName { get; set; } = null!;

        [BindProperty]
        [Display(Name ="رمز")]
        public string? Password { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [Display(Name ="نقش کاربر")]
        public int RoleId { get; set; }
        public IFormFile ProfileImage { get; set; }
        [Description("It's Used to check if profile image changed in Update")]
        [Display(Name ="عکس کاربر")]
        public string PerviuosProfileImage { get; set; }
    }
}
