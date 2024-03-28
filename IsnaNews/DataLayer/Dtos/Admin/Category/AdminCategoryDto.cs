using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Admin.Category
{
    public class AdminCategoryDto
    {
        public AdminCategoryDto(TblCategory tblCategory)
        {
            Id = tblCategory.Id;
            Name = tblCategory.Name;
            if (tblCategory.Parent is not null)
            {
                ParentId = tblCategory.ParentId;
                ParentName = tblCategory.Parent.Name;
            }
        }
        [DisplayName("کد")]
        public int Id { get; set; }
        [DisplayName("نام دسته بندی")]
        public string Name { get; set; }
        [DisplayName("نام دسته بندی مادر")]
        public string ParentName { get; set; } = null;
        public int? ParentId { get; set; } = null;

    }
}
