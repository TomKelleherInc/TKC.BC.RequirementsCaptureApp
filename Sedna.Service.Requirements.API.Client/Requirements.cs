using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Sedna.Service.Requirements.API.Client
{
    public class Requirements : _ClientBase
    {
        public const string controller_name = "requirements";


        public static List<DTO.Requirement> GetAll()
        {
            string url = BuildUrl(controller_name);
            return ApiHttp.Get<List<DTO.Requirement>>(url);
        }
        public static List<DTO.Requirement> GetById(int requirement_id)
        {
            string url = BuildUrl(controller_name, requirement_id.ToString());
            return ApiHttp.Get<List<DTO.Requirement>>(url);
        }
        public static List<DTO.Requirement> GetByIds(List<int> requirement_ids)
        {
            var ids = GetDistinctIds(requirement_ids);
            var qs = HttpUtility.ParseQueryString(""); // makes a special kind of qs that's best for HTTP query strings
            ids.ForEach(i => qs.Add("ids", i.ToString()));
            string url = BuildUrl(controller_name, null, qs);
            return ApiHttp.Get<List<DTO.Requirement>>(url);
        }

        public static List<DTO.Requirement>GetBySubjectIds(List<int> subject_ids)
        {
            var qs = HttpUtility.ParseQueryString(""); // makes a special kind of qs that's best for HTTP query strings
            subject_ids.ForEach(i => qs.Add("subject_ids", i.ToString()));
            string url = BuildUrl(controller_name, null, qs);
            return ApiHttp.Get<List<DTO.Requirement>>(url);
        }

        public static List<DTO.Requirement> GetBySubjectExternalIdentifiers(List<string> external_identifiers)
        {
            var ids = string.Join(',', external_identifiers);

            var qs = HttpUtility.ParseQueryString(""); // makes a special kind of qs that's best for HTTP query strings
            qs.Add("external_identifiers", ids);
            string url = BuildUrl(controller_name, "bysubjects/external_identifiers", qs);
                return ApiHttp.Get<List<DTO.Requirement>>(url);
        }


        public static void Put(DTO.Requirement requirement)
        {
            string url = BuildUrl(controller_name, requirement.RequirementId.ToString());
            ApiHttp.Put<DTO.Requirement>(url, requirement);
        }

        public static DTO.Requirement Post(DTO.Requirement requirement)
        {
            string url = BuildUrl(controller_name);
            return ApiHttp.Post<DTO.Requirement>(url, requirement);
        }

        public static List<DTO.RequirementContext> ResetContexts(int requirement_id, List<int> context_ids)
        {
            string url = BuildUrl(controller_name, $"{requirement_id}/resetcontexts");
            return ApiHttp.Post<List<DTO.RequirementContext>>(url, context_ids);
        }


        public static void Delete(int requirement_id)
        {
            string url = BuildUrl(controller_name, requirement_id.ToString());
            ApiHttp.Delete<DTO.Requirement>(url);
        }



    }
}
