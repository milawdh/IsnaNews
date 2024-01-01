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
        public string Title { get; set; } = null!;
        public int ReporterId { get; set; }
        public string DatePosted { get; set; }
        public string Body { get; set; } = null!;
        public string MainImageUrl { get; set; }
        public bool IsImportantNews { get; set; } = false;
        public long ViewCount { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
        public List<string> VideoUrls { get; set; } = new List<string>();
        public List<string> Keyword { get; set; } = new List<string>();
        public List<(string userName, string UserProfile, long Id, string? Reply)> Comments { get; set; } = new();
    }
}
