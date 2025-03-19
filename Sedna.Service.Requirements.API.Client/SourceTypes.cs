using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Sedna.Service.Requirements.API.Client
{
    public class SourceTypes : _ClientBase
    {
        public const string controller_name = "sourcetypes";


        public static List<DTO.SourceType> GetAll()
        {
            string url = BuildUrl(controller_name);
            return ApiHttp.Get<List<DTO.SourceType>>(url);
        }
        public static List<DTO.SourceType> GetById(int context_id)
        {
            string url = BuildUrl(controller_name, context_id.ToString());
            return ApiHttp.Get<List<DTO.SourceType>>(url);
        }
        public static List<DTO.SourceType> GetByIds(List<int> context_ids)
        {
            var ids = GetDistinctIds(context_ids);
            var qs = HttpUtility.ParseQueryString(""); // makes a special kind of qs that's best for HTTP query strings
            ids.ForEach(i => qs.Add("ids", i.ToString()));
            string url = BuildUrl(controller_name, null, qs);
            return ApiHttp.Get<List<DTO.SourceType>>(url);
        }

    }
}
