using System;
using System.Collections.Generic;
using System.Text;

namespace Sedna.Service.Requirements.API.Client
{

    public static class Instance
    {

        public static string Username = string.Empty;

        public class ClientException : Exception
        {
            public ClientException() : base() { }
            public ClientException(string message) : base(message) { }
            public ClientException(string message, Exception inner_exception) : base(message, inner_exception) { }
        }

    }
}
