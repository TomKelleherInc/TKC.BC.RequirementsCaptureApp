
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using Sedna.API;

namespace Senda.Requirements.Capture.UI
{
    public class Instance
    {
        private static string _sednaApiToken = string.Empty;
        private static string api_base_url = string.Empty;
        private static string _slackChannelName = string.Empty;

        public static void InitializeApiClient(string access_id, string access_secret, string base_url)
        {
            Sedna.Service.Requirements.API.Client.ApiHttp.InitializeHttpClient(access_id, access_secret, base_url, Environment.UserName);

        }


        public static void SetSlackChannelName(string slackChannelName)
        {
            _slackChannelName = slackChannelName;
        }

        public static string SlackChannelName
        {
            get
            {
                return _slackChannelName;
            }
        }



        /// <summary>
        /// Performs an innocuous test on the token.  If if results in an error from
        /// the Sedna.API.Client.Lib, then it's not working.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool TestSednaApiToken(string token)
        {
            try
            {               
                var users = Sedna.API.Client.Lib.Users.GetById(token, 1);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void SetSednaApiToken(string token)
        {
            _sednaApiToken = token;
        }

        public static string SednaApiToken
        {
            get
            {
                return _sednaApiToken;
            }
        }
    }
}
