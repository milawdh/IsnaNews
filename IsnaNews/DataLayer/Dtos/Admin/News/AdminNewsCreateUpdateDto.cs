using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

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

        [Column(TypeName = "datetime")]
        public DateTime DatePosted { get; set; }
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [DisplayName("بدنه خبر")]
        public string Body { get; set; } = null!;
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [DisplayName("دسته بندی خبر")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "فیلد {0} خالی است", AllowEmptyStrings = false)]
        [DisplayName("عکس اصلی خبر")]
        public string MainImageUrl { get; set; }
        public bool IsImportantNews { get; set; } = false;
        public List<string> VideoUrls { get; set; } = new List<string>();
        public List<string> ImageUrls { get; set; } = new List<string>();
        public List<string> Keyword { get; set; } = new List<string>();

    }
}
