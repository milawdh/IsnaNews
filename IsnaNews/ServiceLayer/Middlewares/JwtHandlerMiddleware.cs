using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Utils;
using Services.Services;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ServiceLayer.MiddleWares
{
    public class JwtHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            var request = context.Request;
            context.Request.EnableBuffering();
            if (request.Path.ToString().Contains("AddNews"))
            {
                var body = context.Request.ReadFormAsync().Result;
            }

            var token = context.Request.Cookies["Token"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers.Add("Authorization", "Bearer " + token);
                var decodedJwt = Extentions.DecodeJwt(token, configuration);
                if (!decodedJwt.isAuthenticated)
                {
                    context.Abort();
                }
                var claim = decodedJwt.claimsPrincipal;
                if (claim.Claims.Any(_ => _.Type == ClaimTypes.NameIdentifier))
                {
                    Core core = new Core();
                    var UserName = claim.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value;
                    if (core.User.Any(_ => _.UserName == UserName))
                    {
                        context.User.AddIdentity(new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.NameIdentifier, UserName) }));
                    }
                    else
                    {
                        context.Response.Cookies.Delete("Token");
                    }

                }
            }
            await _next(context);
        }
    }
    public static class JwtHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtHandlerMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtHandlerMiddleware>();
        }
    }
}
