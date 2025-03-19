using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Sedna.Service.Requirements.API.Client
{
    public class SubjectTypeTopics : _ClientBase
    {

        public const string controller_name = "SubjectTypeTopics";


        public static List<DTO.SubjectTypeTopic> GetAll()
        {
            string url = BuildUrl(controller_name);
            return ApiHttp.Get<List<DTO.SubjectTypeTopic>>(url);
        }
        public static List<DTO.SubjectTypeTopic> GetById(int subject_type_topic_id)
        {
            string url = BuildUrl(controller_name, subject_type_topic_id.ToString());
            return ApiHttp.Get<List<DTO.SubjectTypeTopic>>(url);
        }
        public static List<DTO.SubjectTypeTopic> GetByIds(List<int> subject_type_topic_ids)
        {
            var ids = GetDistinctIds(subject_type_topic_ids);
            var qs = HttpUtility.ParseQueryString(""); // makes a special kind of qs that's best for HTTP query strings
            ids.ForEach(i => qs.Add("ids", i.ToString()));
            string url = BuildUrl(controller_name, null, qs);
            return ApiHttp.Get<List<DTO.SubjectTypeTopic>>(url);
        }





        public static DTO.SubjectTypeTopic Post(DTO.SubjectTypeTopic SubjectTypeTopic)
        {
            string url = BuildUrl(controller_name);
            var result = ApiHttp.Post<DTO.SubjectTypeTopic>(url, SubjectTypeTopic);
            return result;
        }
    }
}
