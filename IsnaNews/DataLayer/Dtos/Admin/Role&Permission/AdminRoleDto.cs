using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Admin
{
    public class AdminRoleDto
    {
        public string Name { get; set; } = null!;
        public List<(int Id,string Name)> Permissions { get; set; } = new List<(int Id, string Name)>();
    }
}
