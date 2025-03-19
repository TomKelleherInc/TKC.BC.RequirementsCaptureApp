using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace Sedna.Service.Requirements.API.Client
{
    public class _ClientBase
    {
        public static string token = string.Empty;

        public static string BuildUrl(string controller_name, string path = null, NameValueCollection querystring = null)
        {
            string query = "";

            if (querystring != null)
            {
                NameValueCollection qs = HttpUtility.ParseQueryString(""); // creates a special version of the NameValueCollection for web queries
                qs.Add(querystring);
                query = "?" + qs.ToString();
            }

            return string.Format("{0}/{1}{2}", controller_name, path, query);
        }

        public static List<int> GetDistinctIds(List<int> ids)
        {
            return ids.Distinct().ToList();
        }
    }
}
