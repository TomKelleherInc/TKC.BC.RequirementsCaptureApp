using System;
using System.Collections.Generic;
using System.Text;

namespace Sedna.Service.Requirements.API.Client.Test
{
    class Instance
    {

        private static string _sednaApiToken = string.Empty;
        private static string api_base_url = string.Empty;

        public static void InitializeApiClient(string access_id, string access_secret, string base_url)
        {
            Sedna.Service.Requirements.API.Client.ApiHttp.InitializeHttpClient(access_id, access_secret, base_url, Environment.UserName);

        }

    }
}
