using Microsoft.AspNetCore.Mvc;

namespace IsnaNews.Controllers
{
    public class ArchiveController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
