using System.Collections.Generic;
using System.Net.Http;

namespace Users.Api.Infrastructure.Http
{
    public class HttpCustomRequest
    {
        private string _resource { get; set; }

        private HttpMethod _method { get; set; }

        private object _content { get; set; }

        public HttpCustomRequest(string resource, HttpMethod method)
        {
            _resource = resource;
            _method = method;
            HeaderList = new List<HttpParameter>();
        }

        public object Content { get { return _content; } }

        public string Resource { get { return _resource; } }

        public HttpMethod Method { get { return _method; } }

        public List<HttpParameter> HeaderList { get; set; }

        public HttpRequestMessage RequestMessage { get { return GetHttpRequestMessage(); } }

        public void AddHeader(string headerName, string headerValue)
        {
            var headers = new List<HttpParameter> { new HttpParameter { Name = headerName, Value = headerValue } };
            AddHeaders(headers);
        }

        public void AddHeaders(List<HttpParameter> headers)
        {
            HeaderList.AddRange(headers);
        }

        private HttpRequestMessage GetHttpRequestMessage()
        {
            return new HttpMessageBuilder(_resource, _method, HeaderList, _content).GetMessage();
        }
    }
}
