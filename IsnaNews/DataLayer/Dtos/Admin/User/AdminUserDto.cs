using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace DataLayer.Dtos.Admin.User
{
    public class AdminUserDto
    {
        [DisplayName("کد کاربر")]
        public int Id { get; set; }
        [DisplayName("نام کاربر")]
        public string Name { get; set; } = null!;
        [DisplayName("تلفن کاربر")]
        public string Tell { get; set; } = null!;
        [DisplayName("نام کاربری کاربر")]
        public string UserName { get; set; } = null!;
        [DisplayName("نقش کاربر")]
        public string RoleName { get; set; }
        [DisplayName("تاریخ ثبت نام")]
        public string DateSigned { get; set; }
        public string ProfileImageUrl { get; set; }
        [DisplayName("اخرین بار ورود")]
        public string LastLoginDate { get; set; }
        public virtual List<string> permissions { get; set; } = new List<string>();
    }
}
