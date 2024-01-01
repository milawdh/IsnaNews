using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Dtos.Shared
{
    public class CommentPublicDto
    {
        public UserPublicDto User { get; set; }
        public string Body { get; set; } = null!;
        public DateTime DatePosted { get; set; }
        public string Reply { get; set; } = null;
        public CommentPublicDto(TblNewsComment comment)
        {
            Body = comment.Body;
            DatePosted = comment.DatePosted;
            User = new UserPublicDto(comment.User);
        }
        public CommentPublicDto()
        {

        }
    }
}
