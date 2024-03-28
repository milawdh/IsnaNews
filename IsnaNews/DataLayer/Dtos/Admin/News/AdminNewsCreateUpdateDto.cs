using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using TestMultipart.ModelBinding;

namespace DataLayer.Dtos.Admin.News
{
    public class AdminNewsCreateUpdateDto
    {
        [BindProperty]
        [StringLength(128, ErrorMessage = "فیلد {0} نمیتواند بیش از 128 کارکتر باشد")]
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [Display(Name = "تیتر")]
        public string Title { get; set; } = null!;
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [Display(Name = "خبرنگار")]
        [BindProperty]
        public int ReporterId { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [Display(Name = "بدنه خبر")]
        public string Body { get; set; } = null!;
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [Display(Name = "دسته بندی خبر")]
        [BindProperty]
        public int CategoryId { get; set; }
        [BindProperty]
        public IFormFile MainImage { get; set; }
        public bool IsImportantNews { get; set; } = false;
        public List<IFormFile> VideoUrls { get; set; } = new List<IFormFile>();
        public List<IFormFile> ImageUrls { get; set; } = new List<IFormFile>();
        public List<string> Keyword { get; set; } = new List<string>();

    }
}
