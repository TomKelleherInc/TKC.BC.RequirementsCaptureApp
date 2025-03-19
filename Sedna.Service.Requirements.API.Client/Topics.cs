using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Sedna.Service.Requirements.API.Client
{
    public class Topics :_ClientBase
    {

        public const string controller_name = "topics";

        public static List<DTO.Topic> GetAll()
        {
            string url = BuildUrl(controller_name);
            return ApiHttp.Get<List<DTO.Topic>>(url);
        }
        public static List<DTO.Topic> GetById(int topic_id)
        {
            string url = BuildUrl(controller_name, topic_id.ToString());
            return ApiHttp.Get<List<DTO.Topic>>(url);
        }




        public static DTO.Topic Post(DTO.Topic Topic)
        {
            string url = BuildUrl(controller_name);
            var result = ApiHttp.Post<DTO.Topic>(url, Topic);
            return result;
        }


    }
}
