namespace SMS.EPA.ClientHRM.Model
{
    using System;
    using System.Net;
    using System.Net.Http;

    public class WebApiException : Exception
    {
        public WebApiException(HttpStatusCode statusCode, HttpRequestMessage requestMessage, string data)
            : this(statusCode, requestMessage)
        {
            AttachedData = data;
        }

        public WebApiException(HttpStatusCode statusCode, HttpRequestMessage requestMessage)
        {
            StatusCode = statusCode;
            RequestMessage = requestMessage;
        }

        public HttpStatusCode StatusCode { get; set; }

        public string AttachedData { get; set; }

        public HttpRequestMessage RequestMessage { get; set; }
    }
}
