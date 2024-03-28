using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Admin.Comment
{
    public class AdminCommentReplyDto
    {
        [BindProperty]
        [Required(ErrorMessage = "فیلد {0} خالی است")]
        [DisplayName("کد کامنت مورد نظر")]
        public long ReplyingCommentId { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "فیلد {0} خالی است")]
        [DisplayName("متن")]
        [StringLength(512, ErrorMessage = "فیلد {0} نمیتواند بیش از 512 کارکتر باشد")]
        public string Body { get; set; }
        public string? IP { get; set; }
    }
}
