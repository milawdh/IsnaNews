using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Admin.Config
{
    public class AdminAboutUsDto
    {
        public string Body { get; set; }
        public AdminAboutUsDto(string body)
        {
            Body = body;
        }
    }
}
