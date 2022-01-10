using System.Net.Http;
using Users.Api.Infrastructure.Http;

namespace ResilientPoliciesTests.DSL
{
    public class RestHttpClientBuilder
    {
        HttpClient _client;

        public RestHttpClientBuilder WithHttpClient(HttpClient client)
        {
            _client = client;
            return this;
        }

        public RestHttpClient Build()
        {
            return new RestHttpClient(_client);
        }
    }
}
