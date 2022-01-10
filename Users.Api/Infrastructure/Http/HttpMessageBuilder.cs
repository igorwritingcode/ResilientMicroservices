using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Users.Api.Infrastructure.Http
{
    public class HttpMessageBuilder
    {
        private Uri _uri { get; set; }

        private string _resource { get; set; }

        private HttpMethod _method { get; set; }

        private List<HttpParameter> _headers { get; set; }

        private object _content { get; set; }

        public HttpMessageBuilder(string resource, HttpMethod method, List<HttpParameter> headers, object content)
        {
            _resource = resource;
            _method = method;
            _headers = headers;
            _content = content;
        }

        public HttpRequestMessage GetMessage()
        {
            var url = GenerateUrl();
            var requestMessage = new HttpRequestMessage(_method, url);

            AddHeaders(requestMessage);

            if (_method == HttpMethod.Post || _method == HttpMethod.Put)
            {
                requestMessage.Content = GetBodyFromRestRequest();
            }
            return requestMessage;
        }

        private string GenerateUrl()
        {
            return _uri + _resource;
        }

        private void AddHeaders(HttpRequestMessage httpRequestMessage)
        {
            foreach (var header in _headers)
            {
                httpRequestMessage.Headers.Add(header.Name, header.Value.ToString());
            }
        }

        private ByteArrayContent GetBodyFromRestRequest()
        {
            if (_content != null)
            {
                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var serializedBody = !(_content is string) ? JsonSerializer.Serialize(_content, options) : _content.ToString();
                var buffer = Encoding.UTF8.GetBytes(serializedBody);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return byteContent;
            }
            return null;
        }
    }
}
