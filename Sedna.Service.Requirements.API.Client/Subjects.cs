using System;
using System.Collections.Generic;
using System.Text;

namespace Sedna.Service.Requirements.API.Client
{
    public class Subjects : _ClientBase
    {
        public const string controller_name = "subjects";

        public static List<DTO.Subject> GetAll()
        {
            string url = BuildUrl(controller_name);
            return ApiHttp.Get<List<DTO.Subject>>(url);
        }
        public static List<DTO.Subject> GetById(int context_id)
        {
            string url = BuildUrl(controller_name, context_id.ToString());
            return ApiHttp.Get<List<DTO.Subject>>(url);
        }


        public static DTO.Subject GetByExternalIdentifierWithRequirements(string external_identifier)
        {
            //external_identifier/{external_identifier}/requirements
            string path = $"external_identifier/{external_identifier}/requirements";
            string url = BuildUrl(controller_name, path);
            try
            {
                var result = ApiHttp.Get<DTO.Subject>(url);
                return result;
            }
            catch (Exception ex)
            {
                return null;
                //throw;
            }
             
        }





        public static DTO.Subject Post(DTO.Subject subject)
        {
            string url = BuildUrl(controller_name);
            return ApiHttp.Post<DTO.Subject>(url, subject);
        }
    }
}
