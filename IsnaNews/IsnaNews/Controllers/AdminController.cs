using DataLayer.Dtos.Admin;
using DataLayer.Dtos.Admin.Advertisement;
using DataLayer.Dtos.Admin.Config;
using DataLayer.Dtos.Admin.Keyword;
using DataLayer.Dtos.Admin.News;
using DataLayer.Dtos.Admin.User;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services.Api;
using ServiceLayer.Utils;
using System.Data;

namespace IsnaNews.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public IActionResult NewsList(int? p, int? s)
        {
            p = p ?? 1;
            s = s ?? 0;
            ViewBag.skipCount = s <= 0 ? 0 : s;
            ViewBag.pageCount = p <= 1 ? 1 : p;
            var newsList = _adminService.GetNewsList();
            foreach (var item in newsList.Result)
            {
                item.Body = item.Body.ToString().ToShortBody(15);
            }
            var dataTable = Extentions.BuildDataTable<AdminNewsDto>(newsList.Result, "لیست کاربران");

            return View("NewsDataTable", dataTable);
        }
        public IActionResult UsersList()
        {
            var users = _adminService.GetUsersList();
            var dataTable = Extentions.BuildDataTable<AdminUserDto>(users.Result, "لیست کاربران");
            return PartialView("UserDataTable", dataTable);
        }
        public IActionResult Contracts()
        {
            var aboutUsList = _adminService.GetAboutUsList();
            var contactUsList = _adminService.GetContactUsList();
            (DataTable aboutUsDT, DataTable contactUsDT) data;
            data.aboutUsDT = Extentions.BuildDataTable<AdminAboutUsDto>(aboutUsList.Result, "لیست درباره ما");
            data.contactUsDT = Extentions.BuildDataTable<AdminContactUsDto>(contactUsList.Result, "لیست ارتباط با ما");
            return View("ContractsDataTable", data);
        }

        public IActionResult Keywords()
        {
            var keywordList = _adminService.GetKewordList();
            var dataTable = Extentions.BuildDataTable<AdminKeywordDto>(keywordList.Result, "لیست کلمات کلیدی");
            return View("KeywordsDataTable", dataTable);
        }

        public IActionResult Roles()
        {
            var Roles = _adminService.GetRoleList();
            var dataTable = Extentions.BuildDataTable<AdminRoleDto>(Roles.Result, "لیست نقش ها");
            return View("RolesDataTable", dataTable);
        }

        public IActionResult AdvertisemntsList()
        {
            var advertisemntsList = _adminService.GetAdminAdvertisementList();
            var dataTable = Extentions.BuildDataTable<AdminAdvertisementDto>(advertisemntsList.Result, "لیست تبلیغات");
            return View("AdvertisemntsDataTable", dataTable);
        }
    }
}
