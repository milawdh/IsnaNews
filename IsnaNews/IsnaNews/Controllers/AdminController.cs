using DataLayer.Dtos.Admin.User;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services.Api;
using ServiceLayer.Utils;

namespace IsnaNews.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }





        public IActionResult NewsList()
        {
            return PartialView("NewsDataTable");
        }
        public IActionResult UsersList()
        {
            var model = _adminService.GetUsersList();
            var dataTable = Extentions.BuildDataTable<AdminUserDto>(model.Result);
            return PartialView("UserDataTable",dataTable);
        }
        public IActionResult Settings()
        {
            return PartialView();
        }
        public IActionResult AdvertisemntsList()
        {
            return PartialView("AdvertisemntsDataTable");
        }
    }
}
