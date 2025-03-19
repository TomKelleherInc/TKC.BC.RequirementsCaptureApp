using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sedna.Service.Requirements.API.Middleware
{

    /// <summary>
    /// Logs all requests coming to the API.
    /// </summary>
    public class RequestLogger
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestLogger(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestLogger>();
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            // Need to do this because by default the Request.Body is a one-time use,
            // one-way stream.  We make it bufferable here, and later set the position
            // back to zero, so it's ready for re-use.

            httpContext.Request.EnableBuffering();

            // Open the streamreader with these parameters; "true" at the end ensures it's left open.
            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, false, 1024, true))
            {
                // Ready body
                var requestBody = reader.ReadToEndAsync().Result;
                _logger.LogInformation(requestBody);
                httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            }

            //Move to next delegate/middleware in the pipleline
            await _next.Invoke(httpContext);
        }
    }
}
