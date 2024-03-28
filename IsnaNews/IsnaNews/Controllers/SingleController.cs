using DataLayer.Dtos.Shared;
using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Filters;
using Services.Services;
using System.Net;

namespace IsnaNews.Controllers
{
    [SetUserInfo]
    public class SingleController : Controller
    {
        private readonly Core core;

        public SingleController(Core core)
        {
            this.core = core;
        }
        public IActionResult Index(long id)
        {
            if (!core.News.Any(x => x.Id == id))
            {
                return RedirectToAction("NotFound","Home");
            }
            TblNews tblNews = core.News.Get(i => i.Id == id, includes: "Category,MainImage,Reporter,TblNewsKeyWordRel,TblNewsVideoRel,TblNewsComment,TblNewsImageRel").First();
            NewsPublicDto newsPublicDto = new()
            {
                Id = id,
                CategoryId = tblNews.CategoryId,
                Body = tblNews.Body,
                CategoryName = core.Category.GetById(tblNews.CategoryId).Name,
                DatePosted = tblNews.DatePosted,
                Title = tblNews.Title,
                ViewCount = tblNews.ViewCount,
                ReporterName = tblNews.Reporter.Name,
                MainImageUrl = tblNews.MainImage.ImageUrl,
                Images = tblNews.TblNewsImageRel.Select(i =>
                {
                    return core.Image.GetById(i.ImageId).ImageUrl.ToString();
                }).ToList(),
                Keywords = tblNews.TblNewsKeyWordRel.Select(i =>
                {
                    var body = core.Keyword.GetById(i.KeyWordId).Body;
                    return body.ToString();
                }).ToList(),
                NewsComments = tblNews.TblNewsComment.Where(i => i.Parent == null).Select(i =>
                {
                    UserPublicDto user = new UserPublicDto(core.User.Get(_=>_.Id==i.UserId,includes: "ProfileImage").FirstOrDefault());
                    var comment = new CommentPublicDto()
                    {
                        User = user,
                        Body = i.Body,
                        DatePosted = i.DatePosted,
                    };
                    if(i.InverseParent.Count>0)
                    {
                        comment.Reply = i.InverseParent.First().Body;
                    }
                    return comment;
                }).ToList(),
                Videos = tblNews.TblNewsVideoRel.Select(i =>
                {
                    return core.Video.GetById(i.VideoId).VideoUrl.ToString();
                }).ToList()
            };
            return View(newsPublicDto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddComment(string Body, long PostId)
        {
            if (Body.IsNullOrEmpty())
                return BadRequest("متن دیدگاه نمیتواند خالی باشد");
            if (Body.Trim().Length <= 0)
                return BadRequest("متن دیدگاه نمیتواند خالی باشد");
            if (!core.News.Any(i => i.Id == PostId))
                return BadRequest("خبر مورد نظر یافت نشد");
            if (TempData["UserId"] == null)
                return BadRequest("لطفا وارد حساب کاربری خود شوید");
            try
            {
                int userId = (int)TempData["UserId"];
                string HostName = Dns.GetHostName();
                IPAddress[] IPAddress = Dns.GetHostEntry(HostName).AddressList;
                string Ip = IPAddress[1].ToString();
                TblNewsComment comment = new()
                {
                    Body = Body,
                    DatePosted = DateTime.Now,
                    UserId = userId,
                    Ip = Ip,
                    PostId = PostId
                };
                core.Comment.Add(comment);
                core.Save();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }







    }
}
