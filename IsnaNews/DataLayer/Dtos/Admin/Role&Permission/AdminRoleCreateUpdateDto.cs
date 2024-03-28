using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Dtos.Admin
{
    public class AdminRoleCreateUpdateDto
    {
        [BindProperty]
        [StringLength(32, ErrorMessage = "فیلد {0} نمیتواند بیش از 32 کارکتر باشد")]
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [Display(Name = "نام")]
        public string Name { get; set; } = null!;
        public List<int>? PermissionIds { get; set; } = new List<int>();
    }
}
