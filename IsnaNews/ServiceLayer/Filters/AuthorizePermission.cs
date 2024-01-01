using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AuthorizePermission : TypeFilterAttribute
    {
        public string RequiredPermission { get; set; }
        public AuthorizePermission() : base(typeof(AuthorizePermissionImpl))
        {
        }
        private class AuthorizePermissionImpl : Attribute, IActionFilter
        {
            private readonly IConfiguration _configuration;

            public AuthorizePermissionImpl(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                //Token Validate
                var token = context.HttpContext.Request.Headers["Authorization"];
                if (token.IsNullOrEmpty())
                {
                    context.HttpContext.Response.StatusCode = 404;
                    context.Result = new RedirectToActionResult("Index", "NotFound", null);
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
                    context.Result = new RedirectToActionResult("Index", "NotFound", null);
                    return;
                }

                //Permission Validate


            }
            public void OnActionExecuted(ActionExecutedContext context)
            {
            }
        }
    }
}
