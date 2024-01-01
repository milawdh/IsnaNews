using DataLayer.Dtos.Shared;
using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Services;

namespace IsnaNews.Controllers
{
    public class SingleController : Controller
    {
        public IActionResult Index(long id)
        {
            Core core = new Core();
            TblNews tblNews = core.News.Get(i => i.Id == id, includes: "Category,MainImage,Reporter,TblNewsKeyWordRel,TblNewsVideoRel,TblNewsComment").First();
            NewsPublicDto newsPublicDto = new()
            {
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
                    var comment = new CommentPublicDto()
                    {
                        User = new() { ProfileImage = core.Image.GetById(i.User.ProfileImageId).ImageUrl, UserName = i.User.UserName },
                        Body = i.Body,
                        DatePosted = i.DatePosted,
                        Reply = i.InverseParent.First().Body
                    };
                    return comment;
                }).ToList(),
                Videos = tblNews.TblNewsVideoRel.Select(i =>
                {
                    return core.Video.GetById(i.VideoId).VideoUrl.ToString();
                }).ToList()
            };
            return View(newsPublicDto);
        }
        public IActionResult DownloadAttach(string Name)
        {
            MemoryStream stream = new MemoryStream(System.IO.File.ReadAllBytes(@"wwwroot/Resources/" + Name));
            return File(stream, "APPLICATION/octet-stream", Name);
        }
    }
}
