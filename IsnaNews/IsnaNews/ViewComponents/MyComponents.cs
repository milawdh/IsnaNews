using DataLayer.Dtos.Shared;
using Microsoft.AspNetCore.Mvc;

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
            return View("MostViewedNews");
        }
    }
    [ViewComponent(Name = "LatestNews")]
    public class LatestNews : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("LatestNews");
        }
    }
    [ViewComponent(Name = "ImportantNews")]
    public class ImportantNews : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View("ImportantNews");
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
    [ViewComponent(Name ="FileAttach")]
    public class FileAttach : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<(string Title, string Url)> fileNames)
        {
            return View("FileAttach", fileNames);
        }
    }
    [ViewComponent(Name ="CommentsBar")]
    public class CommentsBar : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<CommentPublicDto> comments)
        {
            return View("CommentsBar", comments);
        }
    }
    [ViewComponent(Name ="ValidationModal")]
    public class ValidationModal : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync((List<string> validationErrors,string Title) titles)
        {
            return View("ValidationModal",titles);
        }
    }
    [ViewComponent(Name ="MyComments")]
    public class MyComments : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<(string newsTitle , string Body,string DatePosted)> model)
        {
            return View("MyComments", model);
        }
    }
    [ViewComponent(Name ="AdminList")]
    public class AdminList : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<string> permissions)
        {
            return View("AdminList",permissions);
        }
    }
}
