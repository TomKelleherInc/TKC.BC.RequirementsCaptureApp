using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Sedna.Service.Requirements.API.Client
{
    public class TopicSearches : _ClientBase
    {
        public const string controller_name = "topicsearches";


        public static List<DTO.TopicSearch> GetAll()
        {
            string url = BuildUrl(controller_name);
            return ApiHttp.Get<List<DTO.TopicSearch>>(url);
        }
        public static List<DTO.TopicSearch> GetById(int context_id)
        {
            string url = BuildUrl(controller_name, context_id.ToString());
            return ApiHttp.Get<List<DTO.TopicSearch>>(url);
        }
        public static List<DTO.TopicSearch> GetByIds(List<int> context_ids)
        {
            var ids = GetDistinctIds(context_ids);
            var qs = HttpUtility.ParseQueryString(""); // makes a special kind of qs that's best for HTTP query strings
            ids.ForEach(i => qs.Add("ids", i.ToString()));
            string url = BuildUrl(controller_name, null, qs);
            return ApiHttp.Get<List<DTO.TopicSearch>>(url);
        }





        public static DTO.TopicSearch Post(DTO.TopicSearch topicSearch)
        {
            string url = BuildUrl(controller_name);
            return ApiHttp.Post<DTO.TopicSearch>(url, topicSearch);
        }
    }
}
