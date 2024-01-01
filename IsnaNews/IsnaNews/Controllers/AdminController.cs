using Microsoft.AspNetCore.Mvc;

namespace IsnaNews.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult NewsList()
        {
            return PartialView();
        }
        public IActionResult UsersList()
        {
            return PartialView();
        }
        public IActionResult Settings()
        {
            return PartialView();
        }
        public IActionResult AdvertisemntsList()
        {
            return PartialView();
        }
    }
}
