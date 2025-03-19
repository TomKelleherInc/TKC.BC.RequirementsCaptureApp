using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Sedna.Service.Requirements.API.Client
{
    public class SubjectTypes : _ClientBase
    {
        public const string controller_name = "subjecttypes";

        public static List<DTO.SubjectType> GetAll()
        {
            string url = BuildUrl(controller_name);
            return ApiHttp.Get<List<DTO.SubjectType>>(url);
        }
        public static List<DTO.SubjectType> GetById(int context_id)
        {
            string url = BuildUrl(controller_name, context_id.ToString());
            return ApiHttp.Get<List<DTO.SubjectType>>(url);
        }
        public static List<DTO.SubjectType> GetByIds(List<int> context_ids)
        {
            var ids = GetDistinctIds(context_ids);
            var qs = HttpUtility.ParseQueryString(""); // makes a special kind of qs that's best for HTTP query strings
            ids.ForEach(i => qs.Add("ids", i.ToString()));
            string url = BuildUrl(controller_name, null, qs);
            return ApiHttp.Get<List<DTO.SubjectType>>(url);
        }


        public static List<DTO.Topic> GetTopicDetails(int subject_type_id)
        {
            string path = $"{subject_type_id}/topicdetails";
            string url = BuildUrl(controller_name, path);
            return ApiHttp.Get<List<DTO.Topic>>(url);
        }


    }
}
