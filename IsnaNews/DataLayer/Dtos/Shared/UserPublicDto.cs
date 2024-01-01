using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Shared
{
    public class UserPublicDto
    {
        public string UserName { get; set; } = null!;
        public string ProfileImage { get; set; } = null!;
        public UserPublicDto(TblUser user)
        {
            UserName = user.UserName;
            ProfileImage = user.ProfileImage.ImageUrl;
        }
        public UserPublicDto()
        {

        }
    }
}
