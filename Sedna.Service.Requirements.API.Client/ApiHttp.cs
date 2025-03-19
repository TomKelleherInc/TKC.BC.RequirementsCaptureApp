using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sedna.Service.Requirements.API.Client
{
    public static class ApiHttp
    {
        private static HttpClient http;

        private static string api_client_access_id = string.Empty;
        private static string api_client_access_secret = string.Empty;
        private static string api_base_url = string.Empty;
        private static string requirements_api_access_token = string.Empty;
        private static string api_client_username = string.Empty;


        //private static HttpClient GetHttpClient(string token)
        //{
        //    HttpClient hc = new HttpClient();
        //    if (!string.IsNullOrEmpty(api_base_url)) hc.BaseAddress = new Uri(api_base_url);
        //    if (!string.IsNullOrEmpty(token)) hc.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        //    hc.Timeout = TimeSpan.FromSeconds(200);
        //    return hc;
        //}

        /// <summary>
        /// Prepares the HttpClient for usage.
        /// </summary>
        /// <param name="access_id">The API Access ID that indicates this Client can talk to the API</param>
        /// <param name="access_secret">The API </param>
        /// <param name="base_url"></param>
        /// <param name="client_username">The username of the person using the system; to be used in the Created_by and Updated_by fields 
        /// when records are created or changed.</param>
        /// <param name="timeout_seconds"></param>
        public static void InitializeHttpClient(string access_id, string access_secret, string base_url, string client_username, int timeout_seconds = 90)
        {
            if (string.IsNullOrWhiteSpace(access_id)) { throw new ArgumentNullException("Missing access_id parameter"); }
            if (string.IsNullOrWhiteSpace(access_secret)) { throw new ArgumentNullException("Missing access_secret parameter"); }
            if (string.IsNullOrWhiteSpace(base_url)) { throw new ArgumentNullException("Missing base_url parameter"); }
            if (string.IsNullOrWhiteSpace(client_username)) { throw new ArgumentNullException("Missing client_username parameter"); }

            api_client_access_id = access_id;
            api_client_access_secret = access_secret;
            api_base_url = base_url;
            api_client_username = client_username;

            http = new HttpClient();

            http.DefaultRequestHeaders.Clear();
            http.DefaultRequestHeaders.Add("x-client-access_id", api_client_access_id);
            http.DefaultRequestHeaders.Add("x-client-access_secret", api_client_access_secret);

            http.DefaultRequestHeaders.Add("x-client-username", api_client_username);

            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            http.Timeout = TimeSpan.FromSeconds(timeout_seconds);
            http.BaseAddress = new Uri(api_base_url);
        }

     

        private static HttpContent GetHttpContent(object body)
        {
            HttpContent hc = body == null ? new StringContent("") : body.GetType() == typeof(byte[]) ? new ByteArrayContent((byte[])body) : new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
            if (hc is StringContent) hc.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            return hc;
        }
        private static T DeserializeResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                if (typeof(T) == typeof(byte[]))
                {
                    byte[] body = response.Content.ReadAsByteArrayAsync().Result;
                    return (T)(object)body;
                }
                else
                {
                    string body = response.Content.ReadAsStringAsync().Result;
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(body);
                }
            }
            else
            {
                string body = response.Content.ReadAsStringAsync().Result;
                Newtonsoft.Json.Linq.JObject jo = null;
                try
                {
                    jo = Newtonsoft.Json.Linq.JObject.Parse(body);
                }
                catch (Exception)
                {
                    //throw;
                }

                string msg = response.ReasonPhrase;
                if (jo != null)
                {
                    if (jo["ExceptionMessage"] != null) msg = jo["ExceptionMessage"].ToString();
                    else if (jo["Message"] != null) msg = jo["Message"].ToString();
                }

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        throw new BadRequestApiException(msg, body);
                        break;
                    case HttpStatusCode.Unauthorized:
                        throw new UnauthorizedApiException(msg, body);
                        break;
                    case HttpStatusCode.Forbidden:
                        throw new ForbiddenApiException(msg, body);
                        break;
                    case HttpStatusCode.NotFound:
                        throw new NotFoundApiException(msg, body);
                        break;
                    case HttpStatusCode.MethodNotAllowed:
                        throw new MethodNotAllowedApiException(msg, body);
                        break;
                    case HttpStatusCode.InternalServerError:
                        throw new ServerApiException(msg, body);
                        break;
                    default:
                        throw new ApiException(msg, (int)response.StatusCode, body);
                        break;
                }
            }
        }

        public static T Get<T>(string url)
        {
            var response = http.GetAsync(url).Result;
            return DeserializeResponse<T>(response);
        }

        public static T Post<T>(string url, object body)
        {
            var response = http.PostAsync(url, GetHttpContent(body)).Result;
            return DeserializeResponse<T>(response);
        }

        public static T Put<T>(string url, object body)
        {
            var response = http.PutAsync(url, GetHttpContent(body)).Result;
            return DeserializeResponse<T>(response);
        }

        public static T Delete<T>(string url)
        {
            var response = http.DeleteAsync(url).Result;
            return DeserializeResponse<T>(response);
        }

        public class ApiClientException : Instance.ClientException
        {
            public ApiClientException(string message) : base(message) { }
        }

        public class BaseAddressException : ApiClientException
        {
            public BaseAddressException(string message) : base(message) { }
        }

        public class ApiException : Instance.ClientException
        {
            public ApiException(string message, int? status_code, string body) : base(message) { StatusCode = status_code; Body = body; }
            public int? StatusCode { get; private set; }
            public string Body { get; private set; }
        }

        public class BadRequestApiException : ApiException
        {
            public BadRequestApiException(string message, string body) : base(message, 400, body) { }
        }

        public class UnauthorizedApiException : ApiException
        {
            public UnauthorizedApiException(string message, string body) : base(message, 401, body) { }
        }

        public class ForbiddenApiException : ApiException
        {
            public ForbiddenApiException(string message, string body) : base(message, 403, body) { }
        }

        public class NotFoundApiException : ApiException
        {
            public NotFoundApiException(string message, string body) : base(message, 404, body) { }
        }

        public class MethodNotAllowedApiException : ApiException
        {
            public MethodNotAllowedApiException(string message, string body) : base(message, 405, body) { }
        }

        public class ServerApiException : ApiException
        {
            public ServerApiException(string message, string body) : base(message, 500, body) { }
        }

    }
}
