using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Admin.Config
{
    public class AdminContactUsDto
    {
        [DisplayName("کد")]
        public int Id { get; set; }
        [DisplayName("متن")]
        public string Body { get; set; }
        public AdminContactUsDto(string body,int id=0)
        {
            Body = body;
            Id = id;
        }
    }
}
