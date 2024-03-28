using DataLayer.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ServiceLayer.MiddleWares;
using Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Middlewares
{
    public class RequestAttemtpsMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestAttemtpsMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, IApplicationBuilder builder)
        {
            var _attemptSessionName = RequestAttemtpsMiddlewareExtentions.AttemptSessionName;
            var AttemptCount = context.Session.GetInt32(_attemptSessionName) ?? 0;
            AttemptCount += 1;
            context.Session.SetInt32(_attemptSessionName, AttemptCount);
            await _next(context);
        }
    }
    public static class RequestAttemtpsMiddlewareExtentions
    {
        internal static string AttemptSessionName;
        public static IApplicationBuilder UseRequestAttemtpsMiddleware(this IApplicationBuilder builder, string attemptSessionName)
        {
            AttemptSessionName = attemptSessionName;
            return builder.UseMiddleware<RequestAttemtpsMiddleware>();
        }
    }
}
