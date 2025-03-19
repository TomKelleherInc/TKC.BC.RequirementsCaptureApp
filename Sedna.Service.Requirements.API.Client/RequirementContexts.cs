using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Sedna.Service.Requirements.API.Client
{
    public class RequirementContexts : _ClientBase
    {
        public const string controller_name = "RequirementContexts";


        public static List<DTO.RequirementContext> GetAll()
        {
            string url = BuildUrl(controller_name);
            return ApiHttp.Get<List<DTO.RequirementContext>>(url);
        }
        public static List<DTO.RequirementContext> GetById(int context_id)
        {
            string url = BuildUrl(controller_name, context_id.ToString());
            return ApiHttp.Get<List<DTO.RequirementContext>>(url);
        }

        private DTO.RequirementContext StripRelatedObjects(DTO.RequirementContext requirementContext)
        {
            // Strip related objects, just pass along IDs
            requirementContext.Requirement = null;
            requirementContext.Context = null;
            return requirementContext;
        }

        public static DTO.RequirementContext Put(DTO.RequirementContext requirementContext)
        {
            // Strip related objects, just pass along IDs
            requirementContext.Requirement = null;
            requirementContext.Context = null;            

            var qs = HttpUtility.ParseQueryString(""); // makes a special kind of qs that's best for HTTP query strings
            qs.Add("requirement_context", requirementContext.RequirementContextId.ToString());
            string url = BuildUrl(controller_name, null, qs);
            return ApiHttp.Put<DTO.RequirementContext>(url, requirementContext);
        }
        public static DTO.RequirementContext Post(DTO.RequirementContext requirementContext)
        {


            string url = BuildUrl(controller_name);
            return ApiHttp.Post<DTO.RequirementContext>(url, requirementContext);
        }
    }
}
