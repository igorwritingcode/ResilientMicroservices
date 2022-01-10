using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace Users.Api.Infrastructure.Http
{
    public class HttpCustomResponse<T> : IHttpCustomResponse<T>
    {
        private T _data;

        private bool _isSuccessful { get; set; }

        public bool IsSuccessful { get { return _isSuccessful; } set { _isSuccessful = value; } }

        public string Content { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public HttpCustomResponse() { }

        public HttpCustomResponse(HttpResponseMessage response)
        {
            _isSuccessful = response.IsSuccessStatusCode;
            StatusCode = response.StatusCode;
        }

        public T Data
        {
            get
            {
                if (IsSuccessful && !string.IsNullOrEmpty(Content))
                {
                    _data = JsonSerializer.Deserialize<T>(Content);
                }

                return _data;
            }
            set
            {
                if (IsSuccessful && !string.IsNullOrEmpty(Content))
                {
                    _data = JsonSerializer.Deserialize<T>(Content);
                }
                else
                {
                    _data = value;
                }
            }
        }
    }
}
