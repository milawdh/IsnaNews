using DataLayer.Dtos.Admin.Comment;
using DataLayer.Dtos.Admin.News;
using DataLayer.Dtos.Admin.User;
using DataLayer.Dtos.Shared;
using DataLayer.Models;
using DataLayer.Types;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Filters;
using ServiceLayer.Services.Api;
using ServiceLayer.Utils;
using Services.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace IsnaNews.Controllers
{
    [TypeFilter(typeof(AuthorizePermission), Arguments = new object[] { "" })]
    [SetUserInfo]
    public class UserProfileController : Controller
    {
        private readonly Core _core;
        private readonly IAdminService _adminService;
        private readonly IConfiguration _configuration;

        public UserProfileController(Core core, IAdminService adminService, IConfiguration configuration)
        {
            _core = core;
            _adminService = adminService;
            _configuration = configuration;
        }
        public IActionResult Index()
        {

            string userName = User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value;
            TblUser tblUser = _core.User.Get(_ => _.UserName == userName, includes: "TblNewsComment").First();
            (string Name, string UserName, string Tell, List<(string newsTitle, string Body, string DatePosted)> comments) model;
            model.Name = tblUser.Name;
            model.UserName = tblUser.UserName;
            model.Tell = tblUser.Tell;
            model.comments = tblUser.TblNewsComment.Select(i =>
            {
                var result = (_core.News.GetById(i.PostId).Title, i.Body, Extentions.ToPersianDate(i.DatePosted));
                return result;
            }).ToList();
            return View(model);
        }
        public IActionResult MyNews()
        {

            string userName = User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value;
            int userId = _core.User.Get(_ => _.UserName == userName).FirstOrDefault().Id;
            List<TblNews> newsList = _core.News.Get(_ => _.ReporterId == userId).ToList();
            List<(string newsTitle, string newsBody, string DatePosted, long newsId, string MainImage)> model = new();
            model = newsList.Select(i =>
            {
                var imageUrl = _core.Image.GetById(i.MainImageId).ImageUrl;
                var result = (i.Title, i.Body, Extentions.ToPersianDate(i.DatePosted), i.Id, imageUrl);
                return result;
            }).ToList();
            return View(model);
        }

        //--News
        [HttpGet]
        public IActionResult GetAddModalNews()
        {
            return PartialView("AddNewsModal", null);
        }
        [HttpGet]
        public IActionResult GetEditModalNews([FromQuery] int Id)
        {
            var result = _adminService.GetNewsById(Id);
            if (!result.Success)
            {
                HttpContext.Response.StatusCode = 400;
                return Json(result);
            }
            return PartialView("EditNewsModal", result.Result);
        }
        [HttpPost]
        [RequestSizeLimit(1073741824)]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 1073741824)]
        public IActionResult AddNews(UserNewsCreateUpdateDto news)
        {
            string userName = User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value;

            var dto = MapNewsDto(news);
            dto.ReporterId = _core.User.Get(_ => _.UserName == userName).FirstOrDefault().Id;
            var result = _adminService.AddNewsAsync(dto).Result;
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [HttpPost]
        [RequestSizeLimit(1073741824)]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 1073741824)]
        public IActionResult UpdateNews(UserNewsCreateUpdateDto news, int Id)
        {
            string userName = User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value;

            var dto = MapNewsDto(news);
            dto.ReporterId = _core.User.Get(_ => _.UserName == userName).FirstOrDefault().Id;
            var result = _adminService.UpdateNewsAsync(dto, Id).Result;
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [HttpDelete]
        public IActionResult DeleteNews(int Id)
        {
            var result = _adminService.DeleteNews(Id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }

        [HttpDelete]
        public IActionResult DeleteNewsImageRel([FromQuery] int id)
        {
            var result = _adminService.DeleteImageRel(id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }
        [HttpDelete]
        public IActionResult DeleteNewsVideoRel([FromQuery] int id)
        {
            var result = _adminService.DeleteVideoRel(id);
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            return Json(result);
        }

        [ValidateAntiForgeryToken]
        public IActionResult UpdateUser(AdminUserCreateUpdateDto user)
        {
            string userName = User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value;
            TblUser tblUser = _core.User.Get(_ => _.UserName == userName).FirstOrDefault();
            int userId = tblUser.Id;
            user.RoleId = tblUser.RoleId;

            var result = _adminService.UpdateUserAsync(user, userId).Result;
            if (!result.Success)
                HttpContext.Response.StatusCode = 400;
            else
            {
                var token = GenerateJwtToken(user.UserName);
                HttpContext.Response.Cookies.Append("Token", token, new CookieOptions { Expires = DateTime.Now.AddMinutes(20) });
            }
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetEditModalUser()
        {
            string userName = User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value;
            int UserId = _core.User.Get(_ => _.UserName == userName).FirstOrDefault().Id;

            var result = _adminService.GetUserById(UserId);
            if (!result.Success)
            {
                HttpContext.Response.StatusCode = 400;
                return Json(result);
            }

            return PartialView("UserModal", result.Result);
        }

        public IActionResult LogOut()
        {
            HttpContext.Response.Cookies.Delete("Token");
            return RedirectToAction("Index", "Home");
        }





        private string GenerateJwtToken(string userName)
        {
            //header
            string Issuer = _configuration["Jwt:Issuer"];
            string Audience = _configuration["Jwt:Audience"];
            var expires = DateTime.Now.AddMinutes(20);
            //signiture
            byte[] key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            //payload
            var subject = new ClaimsIdentity
            (new[]
                {
                new Claim(ClaimTypes.NameIdentifier,userName),
                }
            );
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                //Headers
                Issuer = Issuer,
                Audience = Audience,
                Expires = expires,
                //payload
                Subject = subject,
                //signiture
                SigningCredentials = signingCredentials,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string JwtToken = tokenHandler.WriteToken(token);
            return JwtToken;
        }


        /// <summary>
        /// Maps UserNewsCreateUpdateDto to AdminCreateUpdateNewsDto
        /// </summary>
        /// <returns></returns>
        private AdminNewsCreateUpdateDto MapNewsDto(UserNewsCreateUpdateDto userDto)
        {
            var Result = new AdminNewsCreateUpdateDto()
            {
                Body = userDto.Body,
                CategoryId = userDto.CategoryId,
                ImageUrls = userDto.ImageUrls,
                IsImportantNews = userDto.IsImportantNews,
                Keyword = userDto.Keyword,
                MainImage = userDto.MainImage,
                Title = userDto.Title,
                VideoUrls = userDto.VideoUrls,
            };
            return Result;
        }
    }
}
