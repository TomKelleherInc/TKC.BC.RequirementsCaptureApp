using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Sedna.Service.Requirements.API.Data;
using AutoMapper;

namespace Sedna.Service.Requirements.API.Controllers
{
    /// <summary>
    /// A Sedna base controller that provides functions to do common tasks.
    /// All controllers should inherit this.
    /// </summary>
    public class BaseController : Controller
    {
        protected readonly RequirementsDbContext DbContext;
        protected readonly IMapper Mapper;

        public BaseController(RequirementsDbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext;
            Mapper = mapper;
        }

        

        public string ApiClientUsername
        {
            get
            {
                return Request.Headers["x-client-username"];
            }
        }


        /// <summary>
        /// Throws a standard ArgumentException with a standard error message about the parameter and the calling function.
        /// </summary>
        /// <param name="parameter_name"></param>
        protected static void ThrowArgumentException(string parameter_name, string message = null)
        {
            StackTrace stackTrace = new StackTrace();

            // get calling method name
            var method = stackTrace.GetFrame(1).GetMethod();

            string errMessage = $"Invalid value for argument '{parameter_name}' for {method.ReflectedType.Name}.{method.Name}()";
            if(message != null)
            {
                errMessage += "; " + message;
            }

            throw new ArgumentException(parameter_name, errMessage);

        }

        /// <summary>
        /// Throws a standard ArgumentNullException with a standard error message about the parameter and the calling function.
        /// </summary>
        /// <param name="parameter_name"></param>
        protected static void ThrowArgumentNullException(string parameter_name)
        {
            StackTrace stackTrace = new StackTrace();

            // get calling method name
            var method = stackTrace.GetFrame(1).GetMethod();

            throw new ArgumentNullException(parameter_name, $"Invalid value for argument '{parameter_name}' for {method.ReflectedType.Name}.{method.Name}()");

        }

    }
}
