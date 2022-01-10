using System.Net;

namespace Users.Api.Infrastructure.Http
{
    public interface IHttpCustomResponse
    {
        string Content { get; set; }

        bool IsSuccessful { get; set; }

        HttpStatusCode StatusCode { get; set; }
    }

    public interface IHttpCustomResponse<T> : IHttpCustomResponse
    {
        T Data { get; set; }
    }
}
