using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class SetUserInfo : Attribute, IActionFilter
    {

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Core _core = new Core();
            dynamic controller = context.Controller;
            controller.TempData["AdminPermissions"] = null;
            controller.TempData["UserProfileImage"] = null;

            if (context.HttpContext.User.Claims.Any(_ => _.Type == ClaimTypes.NameIdentifier))
            {
                string userName = context.HttpContext.User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value;
                if(!_core.User.Any(x => x.UserName == userName))
                {
                    context.HttpContext.Response.StatusCode = 400;
                    context.Result = new NotFoundResult();
                }
                TblUser tblUser = _core.User.Get(_ => _.UserName == userName, includes: "Role,ProfileImage").First();
                var userpermissions = new List<string>();
                var rolePermissions = _core.RolePermissionsRel.Get(i => i.RoleId == tblUser.RoleId).Select(i =>
                {
                    var result = _core.Permissions.GetById(i.PermissionId);
                    return result;
                }).ToList();

                rolePermissions.ForEach(i => { userpermissions.Add(i.Name); });
                controller.TempData["AdminPermissions"] = userpermissions;
                controller.TempData["UserProfileImage"] = tblUser.ProfileImage.ImageUrl;
                controller.TempData["UserName"] = tblUser.UserName;
                controller.TempData["UserId"] = tblUser.Id;

            }
        }
    }
}
