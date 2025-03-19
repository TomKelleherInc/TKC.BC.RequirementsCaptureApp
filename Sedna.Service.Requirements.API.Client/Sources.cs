using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Sedna.Service.Requirements.API.Client
{
    public class Sources : _ClientBase
    {
        public const string controller_name = "sources";


        public static List<DTO.Source> GetAll()
        {
            string url = BuildUrl(controller_name);
            return ApiHttp.Get<List<DTO.Source>>(url);
        }
        public static List<DTO.Source> GetById(int source_id)
        {
            string url = BuildUrl(controller_name, source_id.ToString());
            return ApiHttp.Get<List<DTO.Source>>(url);
        }
        public static List<DTO.Source> GetByIds(List<int> source_ids)
        {
            var ids = GetDistinctIds(source_ids);
            var qs = HttpUtility.ParseQueryString(""); // makes a special kind of qs that's best for HTTP query strings
            ids.ForEach(i => qs.Add("ids", i.ToString()));
            string url = BuildUrl(controller_name, null, qs);
            return ApiHttp.Get<List<DTO.Source>>(url);
        }


        public static DTO.Source GetByExternalIdentifier(string external_identifier)
        {
            try
            {
                var path = $"external_identifier/{external_identifier}";
                string url = BuildUrl(controller_name, path);
                return ApiHttp.Get<DTO.Source>(url);
            }
            catch (Exception ex)
            {
                return null;
                //throw;
            }
        }


        //public static List<DTO.Source> GetByExternalIdentifier(List<string> external_identifiers)
        //{
        //    var path = $"external_identifier/{external_identifiers}";
        //    string url = BuildUrl(controller_name, path);
        //    return ApiHttp.Get<List<DTO.Source>>(url);
        //}



        public static DTO.Source Post(DTO.Source source)
        {
            string url = BuildUrl(controller_name);
            return ApiHttp.Post<DTO.Source>(url, source);
        }

    }
}
