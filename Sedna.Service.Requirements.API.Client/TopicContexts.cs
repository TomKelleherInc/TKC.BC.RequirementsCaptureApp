using System;
using System.Collections.Generic;
using System.Text;

namespace Sedna.Service.Requirements.API.Client
{
    class TopicContexts : _ClientBase
    {

        public const string controller_name = "topiccontexts";

        public static List<DTO.TopicContext> GetAll()
        {
            string url = BuildUrl(controller_name);
            return ApiHttp.Get<List<DTO.TopicContext>>(url);
        }
        public static List<DTO.TopicContext> GetById(int context_id)
        {
            string url = BuildUrl(controller_name, context_id.ToString());
            return ApiHttp.Get<List<DTO.TopicContext>>(url);
        }
    
    }
}
