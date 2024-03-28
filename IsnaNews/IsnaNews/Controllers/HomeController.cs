using IsnaNews.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Filters;
using Services.Services;
using System.Diagnostics;

namespace IsnaNews.Controllers
{
    [SetUserInfo]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Core core;

        public HomeController(ILogger<HomeController> logger,Core core)
        {
            _logger = logger;
            this.core = core;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            ViewBag.Title = "درباره ما";
            var model = core.AboutUs.Get().Select(_=>_.Value).ToList();
            return View("Config",model);
        }
        public IActionResult ContactUs()
        {
            ViewBag.Title = "ارتباط با ما";
            var model = core.ContactUs.Get().Select(_ => _.Value).ToList();
            return View("Config",model);
        }
        [HttpGet]
        public IActionResult GetCategories()
        {
            var result = core.Category.Get(includes: "InverseParent").Select(_ =>
            {
                var category = _;
                category.Parent = null;
                return category;
            }).ToList();
            return Json(result);
        }

        public IActionResult NotFound()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
