using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Sedna.Service.Requirements.API.Client.Test
{

    [TestClass]
    public class BaseTest
    {



        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext tc)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build();

            string api_client_id = config["requirements_api:client_access_id"];
            string api_client_secret = config["requirements_api:client_access_secret"];
            string api_base_url = config["requirements_api:base_url"];

            Instance.InitializeApiClient(api_client_id, api_client_secret, api_base_url);

            //get token
            //string client_id = ConfigurationManager.AppSettings["TestClientId"];
            //string client_secret = ConfigurationManager.AppSettings["TestClientSecret"];
            //string password = "admin";


        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {

        }

        [TestInitialize]
        public void TestInitialize()
        {

        }

        [TestCleanup]
        public void TestCleanup()
        {

        }
    }
}
