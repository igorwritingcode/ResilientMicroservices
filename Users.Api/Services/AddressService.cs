using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Users.Api.Infrastructure.Http;
using Users.Api.Models;
using Users.Api.ResilientPolicies;

namespace Users.Api.Services
{
    public interface IAddressService
    {
        Task<Address> GetAddress(string userId);
    }

    public class AddressService : IAddressService
    {
        private readonly IRestHttpClient _client;
        
        private readonly IResilientPolicyWrapper _resilientHttpClientWrapper;

        public AddressService(IRestHttpClient client, IResilientPolicyWrapper resilientHttpClientWrapper)
        {
            _client = client;
            _resilientHttpClientWrapper = resilientHttpClientWrapper;
        }

        public async Task<Address> GetAddress(string userId)
        {
            var request = new HttpCustomRequest($"Address/addresses/{userId}", HttpMethod.Get);
            var response = await _resilientHttpClientWrapper.ExecuteAsync(() => _client.ExecuteRequest<object>(request));
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            if(response.IsSuccessful)
            {
                var addresses = JsonSerializer.Deserialize<IEnumerable<Address>>(response.Content, options);
                return addresses?.FirstOrDefault();
            }

            return default;
        }
    }
}
