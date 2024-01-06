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

namespace DataLayer.Dtos.Admin.News
{
    public class AdminNewsCreateUpdateDto
    {
        [StringLength(128, ErrorMessage = "فیلد {0} نمیتواند بیش از 128 کارکتر باشد")]
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [DisplayName("تیتر")]
        public string Title { get; set; } = null!;
        [StringLength(128, ErrorMessage = "فیلد {0} نمیتواند بیش از 128 کارکتر باشد")]
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [DisplayName("خبرنگار")]
        public int ReporterId { get; set; }

        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [DisplayName("بدنه خبر")]
        public string Body { get; set; } = null!;
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [DisplayName("دسته بندی خبر")]
        public int CategoryId { get; set; }
        public IFormFile MainImage { get; set; }

        [Description("It's Used to check if Main image changed in Update")]
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [DisplayName("عکس اصلی خبر")]
        public string PerviuosMainImage { get; set; }
        public bool IsImportantNews { get; set; } = false;
        public List<IFormFile> VideoUrls { get; set; } = new List<IFormFile>();
        public List<IFormFile> ImageUrls { get; set; } = new List<IFormFile>();
        public List<string> Keyword { get; set; } = new List<string>();

    }
}
