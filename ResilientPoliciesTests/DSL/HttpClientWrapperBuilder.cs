using Microsoft.Extensions.DependencyInjection;
using ResilientPoliciesTests.Fakes;
using System;
using System.Net;
using System.Net.Http;
using Users.Api.ResilientPolicies;
using Users.Api.ResilientPolicies.Settings;

namespace ResilientPoliciesTests.DSL
{
    public class HttpClientWrapperBuilder
    {
        private readonly Uri _uri = new ("http://localhost");

        private HttpStatusCode _responseStatusCode;

        private TimeSpan _responseLatency = TimeSpan.Zero;

        private IResilientPolicyWrapper _resilientPolicies;

        private RetryPolicySettings _retryPolicySettings;

        private CircuitBreakerPolicySettings _circuitBreakerPolicySettings;

        public HttpClientWrapperBuilder WithStatusCode(HttpStatusCode statusCode)
        {
            _responseStatusCode = statusCode;
            return this;
        }

        public HttpClientWrapperBuilder WithResponseLatency(TimeSpan responseLatency)
        {
            _responseLatency = responseLatency;
            return this;
        }

        public HttpClientWrapperBuilder WithRetryPolicySettings(RetryPolicySettings retryPolicySettings)
        {
            _retryPolicySettings = retryPolicySettings;
            return this;
        }

        public HttpClientWrapperBuilder WithCircuitBreakerPolicySettings(CircuitBreakerPolicySettings circuitBreakerPolicySettings)
        {
            _circuitBreakerPolicySettings = circuitBreakerPolicySettings;
            return this;
        }

        public HttpClientWrapperBuilder WithResilientPolicies(IResilientPolicyWrapper resilientPolicies)
        {
            _resilientPolicies = resilientPolicies;
            return this;
        }

        public HttpClientWrapper Build(string clientName)
        {
            var handler = new MockHttpMessageHandler(_responseStatusCode, _responseLatency);

            void DefaultClient(HttpClient client)
            {
                client.BaseAddress = _uri;
            }

            var services = new ServiceCollection();
            services
                .AddHttpClient<IMockHttpClient, MockHttpClient>(clientName, DefaultClient)
                .ConfigurePrimaryHttpMessageHandler(() => handler);

            var serviceProvider = services.BuildServiceProvider();
            var factory = serviceProvider.GetService<IHttpClientFactory>();

            var client = factory?.CreateClient(clientName) ?? throw new NullReferenceException("Http Client was not properly created");

            return new HttpClientWrapper(client, handler);
        }
    }
}
