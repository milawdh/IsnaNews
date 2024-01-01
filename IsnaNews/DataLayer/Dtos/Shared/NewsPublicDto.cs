using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Shared
{
    public class NewsPublicDto
    {
        public string Title { get; set; } = null!;
        public string ReporterName { get; set; } = null!;
        public DateTime DatePosted { get; set; }
        public string Body { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public string MainImageUrl { get; set; }
        public long ViewCount { get; set; }
        public List<string> Videos { get; set; } = new();
        public List<string> Images { get; set; } = new();
        public List<string> Keywords { get; set; } = new();
        public List<(string Title, string Url)> FileAttachs { get; set; } = new();
        public List<CommentPublicDto> NewsComments { get; set; } = new();
    }
}
