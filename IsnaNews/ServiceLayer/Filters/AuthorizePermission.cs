using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Utils;
using Services.Services;
using System.Security.Claims;

namespace ServiceLayer.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AuthorizePermission : TypeFilterAttribute
    {
        public static string? _RequiredPermission { get; set; }
        public AuthorizePermission(string? requiredPermission = null) : base(typeof(AuthorizePermissionImpl))
        {
            _RequiredPermission = requiredPermission;
        }
        private class AuthorizePermissionImpl : Attribute, IActionFilter
        {
            private readonly IConfiguration _configuration;
            private readonly Core _core;

            public AuthorizePermissionImpl(IConfiguration configuration, Core core)
            {
                _configuration = configuration;
                _core = core;
            }

            public void OnActionExecuting(ActionExecutingContext context)

            {
                //Token Validate
                var token = context.HttpContext.Request.Headers["Authorization"];
                if (token.IsNullOrEmpty())
                {
                    context.HttpContext.Response.StatusCode = 404;
                    context.Result = new NotFoundResult();
                    return;
                }
                token = token.ToString().Split("Bearer ")[1];
                var validateResult = Extentions.DecodeJwt(token, _configuration);
                //Not Valid Token
                if (!validateResult.isAuthenticated ||
                    !validateResult.claimsPrincipal.Claims.Any(_ => _.Type == ClaimTypes.NameIdentifier)
                    )
                {
                    context.HttpContext.Response.StatusCode = 404;
                    context.Result = new NotFoundResult();
                    return;
                }

                //Permission Validate
                string UserName = context.HttpContext.User.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value;
                if (!_core.User.Any(_ => _.UserName == UserName))
                {
                    context.HttpContext.Abort();
                    return;
                }


                TblUser tblUser = _core.User.Get(_ => _.UserName == UserName).FirstOrDefault();
                List<string> userPermissions = _core.RolePermissionsRel.Get(_ => _.RoleId == tblUser.RoleId)
                    .Select(_ =>
                    {
                        TblRolePermissions permission = _core.Permissions.GetById(_.PermissionId);
                        return permission.Name;
                    }).ToList();
                if (!_RequiredPermission.IsNullOrEmpty())
                {
                    if (!userPermissions.Any(_ => _ == _RequiredPermission))
                    {
                        context.HttpContext.Response.StatusCode = 404;
                        context.Result = new NotFoundResult();
                    }
                }
            }
            public void OnActionExecuted(ActionExecutedContext context)
            {
            }
        }
    }
}
