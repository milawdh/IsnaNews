using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Admin.Keyword
{
    public class AdminKeywordDto
    {
        [DisplayName("کد")]
        public int Id { get; set; }
        [DisplayName("متن")]
        public string Body { get; set; }
        public AdminKeywordDto(TblKeyWord tblKeyWord) { Id = tblKeyWord.Id; Body = tblKeyWord.Body; Id = tblKeyWord.Id; }
    }
}
