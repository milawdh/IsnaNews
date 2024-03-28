using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Admin.Category
{
    public class AdminCategoryCreateUpdateDto
    {
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [StringLength(32, ErrorMessage = "فیلد {0} نمیتواند بیش از 32 کاراکتر باشد")]
        [Display(Name = "نام دسته بندی")]
        public string Name { get; set; } = null!;

        public int? ParentId { get; set; } = null;
    }
}
