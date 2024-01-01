using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ServiceLayer.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateModel : Attribute, Microsoft.AspNetCore.Mvc.Filters.IActionFilter
    {
        private readonly string _redirectAction;



        public ValidateModel(string redirectAction)
        {
            _redirectAction = redirectAction;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Method == "POST")
            {
                var validationErros = context.ModelState.Where(i => i.Value.Errors.Count > 0).Select(i =>
                {
                    foreach (var item in i.Value.Errors)
                    {
                        return item.ErrorMessage.ToString();
                    }
                    return "";
                }).ToList();
                dynamic controller = context.Controller;
                var controllerName = controller.RouteData.Values["controller"];
                if (validationErros.Count > 0)
                {
                    controller.TempData["Errors"] = null;
                    controller.TempData["Errors"] = validationErros.ToList();
                    controller.TempData["ErrorTitle"] = "در پر کردن فیلد ها دقت کنید";
                    context.Result = new RedirectToActionResult(_redirectAction, controllerName, null);
                }
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

    }
}
