using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;

namespace ServiceLayer.Filters
{
    public class AuthorizeAttempts : Attribute, IActionFilter
    {
        string AttemptSession;
        string RedirectAction;
        public AuthorizeAttempts(string attemptSession, string redirectAction)
        {
            AttemptSession = attemptSession;
            RedirectAction = redirectAction;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {

            var attempts = context.HttpContext.Session.GetInt32(AttemptSession);
            if (attempts > 10)
            {
                dynamic controller = context.Controller;
                controller.TempData["ErrorTitle"] = "تعداد درخواست بیش از حد";
                controller.TempData["Errors"] = new List<string>() { "لطفا چند دقیقه بعد امتحان کنید" };
                context.Result = new RedirectToActionResult(RedirectAction, controller.RouteData.Values["controller"], null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
