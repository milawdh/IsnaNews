using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Admin.User
{
    public class AdminUserDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Tell { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string RoleName { get; set; }

        public string DateSigned { get; set; }

        public string ProfileImageUrl { get; set; }

        public string LastLoginDate { get; set; }
        public List<string> permissions { get; set; } = new List<string>();
    }
}
