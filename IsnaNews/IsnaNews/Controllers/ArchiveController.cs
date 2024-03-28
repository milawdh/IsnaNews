using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Filters;
using Services.Services;

namespace IsnaNews.Controllers
{
    [SetUserInfo]
    public class ArchiveController : Controller
    {
        private readonly Core _core;

        public ArchiveController(Core core)
        {
            _core = core;
        }
        public IActionResult Index(int? p, [FromQuery] string? s, int? c)
        {
            p = p ?? 1;
            ViewBag.pageCount = p <= 1 ? 1 : p;
            var newsList = _core.News.Get(includes: "Reporter,MainImage").ToList();
            if (!s.IsNullOrEmpty())
            {
                if (s.Length > 128)
                    return RedirectToAction("NotFound", "Home");

                newsList = newsList.Where(_ => _.Body.Contains(s) ||
                _.Title.Contains(s) || _.Reporter.Name.Contains(s)).ToList();
            }

            if (c.HasValue)
            {
                newsList = newsList.Where(_ => _.CategoryId == c).ToList();
            }

            var model = newsList.Skip((p.Value - 1) * 6).Take(6).ToList();
            ViewBag.AllPageCount = newsList.Count == 0 || newsList.Count == 6 ? 1 :
                Convert.ToInt32(Math.Ceiling(a: newsList.Count / 10)) + 1;
            return View(model);
        }
    }
}
