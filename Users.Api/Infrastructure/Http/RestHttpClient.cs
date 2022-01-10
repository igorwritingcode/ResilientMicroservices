using System.Net.Http;
using System.Threading.Tasks;

namespace Users.Api.Infrastructure.Http
{
    public interface IRestHttpClient
    {
        Task<HttpCustomResponse<T>> ExecuteRequest<T>(HttpCustomRequest request) where T : new();
    }

    public class RestHttpClient : IRestHttpClient
    {
        private readonly HttpClient _httpClient;

        public RestHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

		public async Task<HttpCustomResponse<T>> ExecuteRequest<T>(HttpCustomRequest request) where T : new()
		{
            using var httpRequestMessage = request.RequestMessage;
            var response = await _httpClient.SendAsync(httpRequestMessage);
            var content = response.Content != null ? await response.Content.ReadAsStringAsync() : string.Empty;
            return new HttpCustomResponse<T>(response)
            {
                Content = content
            };
        }
	}
}
