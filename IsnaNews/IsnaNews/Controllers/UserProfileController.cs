using DataLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Filters;
using ServiceLayer.Utils;
using Services.Services;
using System.Security.Claims;

namespace IsnaNews.Controllers
{
    [AuthorizePermission]
    public class UserProfileController : Controller
    {
        private readonly Core _core;

        public UserProfileController(Core core)
        {
            _core = core;
        }
        public IActionResult Index()
        {
            SetUserData();
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
            SetUserData();
            string userName = User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value;
            TblUser tblUser = _core.User.Get(_ => _.UserName == userName, includes: "TblNews").First();
            List<(string newsTitle, string newsBody, string DatePosted, long newsId, string MainImage)> model = new();
            model = tblUser.TblNews.Select(i =>
            {
                var imageUrl = _core.Image.GetById(i.MainImageId).ImageUrl;
                var result = (i.Title, i.Body, Extentions.ToPersianDate(i.DatePosted), i.Id, imageUrl);
                return result;
            }).ToList();
            return View(model);
        }
        internal void SetUserData()
        {
            TempData["AdminPermissions"] = null;
            TempData["UserProfileImage"] = null;
            string userName = User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value;
            TblUser tblUser = _core.User.Get(_ => _.UserName == userName, includes: "Role,ProfileImage").First();
            var userpermissions = new List<string>();
            var rolePermissions = _core.RolePermissionsRel.Get(i => i.RoleId == tblUser.RoleId).Select(i =>
            {
                var result = _core.Permissions.GetById(i.PermissionId);
                return result;
            }).ToList();
            rolePermissions.ForEach(i => { userpermissions.Add(i.Name); });
            TempData["AdminPermissions"] = userpermissions;
            TempData["UserProfileImage"] = tblUser.ProfileImage.ImageUrl;

        }
    }
}
