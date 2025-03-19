using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sedna.Service.Requirements.API.Middleware
{

    /// <summary>
    /// Used to check that the headers include an "x-client-username" for all requests
    /// that are not Method=GET.  These are used in the POST and PUT operations 
    /// to ensure we have a username to put into the Created_By and Updated_By fields.
    /// <para>
    /// Add to this checker as needed.  
    /// </para>
    /// </summary>
    public class HttpHeaderChecker
    {
        private readonly RequestDelegate _next;

        public HttpHeaderChecker(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            
            var request = httpContext.Request;

            if (request.Method != "GET")  // that is, if it's a POST, PUT or DELETE...
            {
                // We need a username to log it with
                if (request.Headers.ContainsKey("x-client-username") == false
                    || string.IsNullOrWhiteSpace(request.Headers["x-client-username"])
                    )
                {
                    throw new Exception("PUT, POST and DELETE require x-client-username header");
                }
            }

            //Move to next delegate/middleware in the pipleline
            await _next.Invoke(httpContext);
        }
    }
}