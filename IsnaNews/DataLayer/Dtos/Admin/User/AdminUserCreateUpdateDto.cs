using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Admin.User
{
    public class AdminUserCreateUpdateDto
    {
        [BindProperty]
        [StringLength(16, ErrorMessage = "فیلد {0} نمیتواند بیش از 32 کارکتر باشد")]
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [DisplayName("نام")]

        public string Name { get; set; } = null!;

        [BindProperty]
        [StringLength(16, ErrorMessage = "فیلد {0} نمیتواند بیش از 32 کارکتر باشد")]
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [DisplayName("تلفن")]

        public string Tell { get; set; } = null!;

        [BindProperty]
        [StringLength(32, ErrorMessage = "فیلد {0} نمیتواند بیش از 32 کارکتر باشد")]
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [DisplayName("نام کاربری")]
        public string UserName { get; set; } = null!;

        [BindProperty]
        [Required(ErrorMessage = "فیلد {0} خالی است")]
        [DisplayName("رمز")]
        public string Password { get; set; } = null!;
        public int? RoleId { get; set; }
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [DisplayName("عکس کاربر")]
        public string ProfileImageUrl { get; set; }

    }
}
