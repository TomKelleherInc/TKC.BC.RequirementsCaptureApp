using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sedna.Service.Requirements.API.Middleware
{
    public static class LoggerMiddlewareExtension
    {
        public static IApplicationBuilder UseLogger(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<RequestLogger>();
        }
    }


    public static class HttpHeaderCheckerExtension
    {
        public static IApplicationBuilder UseHttpHeaderChecker(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<HttpHeaderChecker>();
        }
    }
}
