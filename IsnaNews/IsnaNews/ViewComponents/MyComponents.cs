using DataLayer.Dtos.Shared;
using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using System.Data;

namespace IsnaNews.ViewComponents
{
    [ViewComponent(Name = "Footer")]
    public class Footer : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("Footer");
        }
    }

    [ViewComponent(Name = "MostViewedNews")]
    public class MostViewedNews : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Core core = new Core();
            List<TblNews> tblNews = core.News.Get().OrderByDescending(_ => _.ViewCount).Take(6).ToList();
            return View("MostViewedNews", tblNews);
        }
    }
    [ViewComponent(Name = "Ads")]
    public class Ads : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Core core = new Core();
            List<TblAdvertisement> ads = core.Advertisement.Get(includes: "MainBaner").ToList();
            return View("Ads", ads);
        }
    }
    [ViewComponent(Name = "LatestNews")]
    public class LatestNews : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Core core = new Core();
            List<TblNews> tblNews = core.News.Get(includes:"MainImage").OrderByDescending(_ => _.DatePosted).Take(5).ToList();
            return View("LatestNews", tblNews);
        }
    }
    [ViewComponent(Name = "ImportantNews")]
    public class ImportantNews : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Core core = new Core();
            List<TblNews> tblNews = core.News.Get(includes: "MainImage").Where(_ => _.IsImportantNews).ToList();
            return View("ImportantNews", tblNews);
        }
    }
    [ViewComponent(Name = "VideoBox")]
    public class VideoBox : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<string> videos)
        {
            return View("VideoBox", videos);
        }
    }
    [ViewComponent(Name = "Keywords")]
    public class Keywords : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<string> keywords)
        {
            return View("Keywords", keywords);
        }
    }
    [ViewComponent(Name = "FileAttach")]
    public class FileAttach : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<(string Title, string Url)> fileNames)
        {
            return View("FileAttach", fileNames);
        }
    }
    [ViewComponent(Name = "CommentsBar")]
    public class CommentsBar : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<CommentPublicDto> comments)
        {
            return View("CommentsBar", comments);
        }
    }
    [ViewComponent(Name = "ValidationModal")]
    public class ValidationModal : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync((List<string> validationErrors, string Title) titles)
        {
            return View("ValidationModal", titles);
        }
    }
    [ViewComponent(Name = "MyComments")]
    public class MyComments : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<(string newsTitle, string Body, string DatePosted)> model)
        {
            return View("MyComments", model);
        }
    }
    [ViewComponent(Name = "AdminList")]
    public class AdminList : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<string> permissions)
        {
            return View("AdminList", permissions);
        }
    }
    [ViewComponent(Name = "AdminAdvertisementModal")]
    public class AdminAdvertisementModal : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("AdminAdvertisementModal");
        }
    }

    [ViewComponent(Name = "OneBodyModal")]
    public class OneBodyModal : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("OneBodyModal");
        }
    }

    [ViewComponent(Name = "PermissionSelectModal")]
    public class PermissionSelectModal : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Core core = new Core();
            var permissions = core.Permissions.Get().ToList();
            return View("PermissionSelectModal", permissions);
        }
    }
    [ViewComponent(Name = "AdminDataTable")]
    public class AdminDataTable : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Tuple<DataTable, int, int, bool, bool> data)
        {
            (DataTable data, int pageCount, int AllPageCount, bool HasUpdate, bool HasDelete) model =
                (data.Item1, data.Item2, data.Item3, data.Item4, data.Item5);
            return View("AdminDataTable", model);
        }
    }
    [ViewComponent(Name = "RolesOptions")]
    public class RolesOptions : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string RoleName = "NormalUser")
        {
            Core core = new Core();
            var roles = core.Role.Get().ToList();
            return View("RolesOptions", (roles, RoleName));
        }
    }
    [ViewComponent(Name = "UsersOptions")]
    public class UsersOptions : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int selectedUser = 0)
        {
            Core core = new Core();
            var users = core.User.Get().ToList();
            return View("UsersOptions", (users, selectedUser));
        }
    }
    [ViewComponent(Name = "CategoryOptions")]
    public class CategoryOptions : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int selectedCategory = 0)
        {
            Core core = new Core();
            var categories = core.Category.Get().ToList();
            return View("CategoryOptions", (categories, selectedCategory));
        }
    }
    [ViewComponent(Name = "AllKeywords")]
    public class AllKeywords : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<string>? selectedKeywords = null)
        {
            selectedKeywords = selectedKeywords ?? new List<string>();
            Core core = new Core();
            var keywords = core.Keyword.Get().ToList();
            return View("AllKeywords", (keywords, selectedKeywords));
        }
    }
    [ViewComponent(Name = "ParentCategoryOptions")]
    public class ParentCategoryOptions : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Tuple<int, int> Ids)
        {
            int selectedCategory = Ids.Item1;
            int currentCategoryId = Ids.Item2;
            Core core = new Core();
            var categories = core.Category.Get().ToList();
            return View("ParentCategoryOptions", (categories, selectedCategory, currentCategoryId));
        }
    }
}
