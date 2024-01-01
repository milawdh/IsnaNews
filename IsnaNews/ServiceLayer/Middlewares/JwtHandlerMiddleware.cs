using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Utils;
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

            var token = context.Request.Cookies["Token"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers.Add("Authorization", "Bearer " + token);
                var claim = Extentions.DecodeJwt(token, configuration).claimsPrincipal;
                if (claim.Claims.Any(_ => _.Type == ClaimTypes.NameIdentifier))
                {
                    var UserName = claim.Claims.FirstOrDefault(_ => _.Type == ClaimTypes.NameIdentifier).Value;
                    context.User.AddIdentity(new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.NameIdentifier, UserName) }));
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
