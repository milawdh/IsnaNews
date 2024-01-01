using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Admin
{
    public class AdminUsersPermissionsDto
    {
        public AdminUsersPermissionsDto(List<string> permissions) 
        {
            Permissions = permissions;
        }
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
