using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Admin.News
{
    public class AdminNewsDto
    {
        [DisplayName("کد")]
        public long Id { get; set; }
        [DisplayName("تیتر خبر")]
        public string Title { get; set; } = null!;
        [DisplayName("کد خبرنگار")]
        public int ReporterId { get; set; }
        [DisplayName("نام خبرنگار")]
        public string ReporterName { get; set; }
        [DisplayName("کد دسته بندی")]
        public int CategoryId { get; set; }
        [DisplayName("نام دسته بندی")]
        public string CategoryName { get; set; }
        [DisplayName("تاریخ انتشار")]
        public string DatePosted { get; set; }
        [DisplayName("متن خبر")]
        public string Body { get; set; } = null!;
        public string MainImageUrl { get; set; }
        [DisplayName("خبر مهم")]
        public bool IsImportantNews { get; set; } = false;
        [DisplayName("تعداد بازدید")]
        public long ViewCount { get; set; }
        public virtual List<(string Url , long id)> ImageUrls { get; set; } = new List<(string Url, long id)>();
        public virtual List<(string Url, long id)> VideoUrls { get; set; } = new List<(string Url, long id)>();
        public virtual List<string> Keyword { get; set; } = new List<string>();
        public virtual List<(string userName, string UserProfile, long Id, string? Reply,string Body,string DatePosted,long? ReplyId, bool IsReply)> Comments { get; set; } = new();
    }
}
