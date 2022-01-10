using AutoFixture;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using ResilientPoliciesTests.DSL;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Users.Api.Infrastructure.Exceptions;
using Users.Api.Infrastructure.Http;
using Users.Api.ResilientPolicies;
using Users.Api.ResilientPolicies.Settings;

namespace ResilientPoliciesTests
{
    public class ResilientPoliciesWrapperTests
    {
        readonly IOptions<CircuitBreakerPolicySettings> _breakerOptions;

        readonly IOptions<RetryPolicySettings> _retryOptions;

        const string ClientName = "TestClient";

        public ResilientPoliciesWrapperTests()
        {
            _breakerOptions = Substitute.For<IOptions<CircuitBreakerPolicySettings>>();

            _retryOptions = Substitute.For<IOptions<RetryPolicySettings>>();
        }

        [Test]
        public async Task ShouldRetry3Times_When503Received()
        {
            const int retryCount = 3;

            _retryOptions.Value.Returns(CreateRetryPolicySettings(retryCount));
            _breakerOptions.Value.Returns(CreateCircuitBreakerPolicySettings());

            var resilietPolicyWrapper = Build.ResilientPolicyWrapperBuilder
                .WithRetrySettings(_retryOptions)
                .WithBreakerSettings(_breakerOptions)
                .Build();

            var httpClientWrapper = Build.HttpClientWrapperBuilder
                .WithStatusCode(HttpStatusCode.ServiceUnavailable)
                .Build(ClientName);

            var httpRestClient = Build.RestHttpClientBuilder
                .WithHttpClient(httpClientWrapper.Client)
                .Build();

            var request = new HttpCustomRequest("resource", HttpMethod.Post);
            var response = await resilietPolicyWrapper.ExecuteAsync(() => httpRestClient.ExecuteRequest<object>(request));

            Assert.AreEqual(HttpStatusCode.ServiceUnavailable, response.StatusCode);

            Assert.AreEqual(retryCount + 1, httpClientWrapper.NumberOfCalls);
        }

        [Test]
        public async Task Should_ThrowCustomException_WhenBreakerStateOpen()
        {
            const int retryCount = 3;

            _retryOptions.Value.Returns(CreateRetryPolicySettings(retryCount));
            _breakerOptions.Value.Returns(CreateCircuitBreakerPolicySettings());

            var resilietPolicyWrapper = Build.ResilientPolicyWrapperBuilder
                .WithRetrySettings(_retryOptions)
                .WithBreakerSettings(_breakerOptions)
                .Build();

            var httpClientWrapper = Build.HttpClientWrapperBuilder
                .WithStatusCode(HttpStatusCode.ServiceUnavailable)
                .Build(ClientName);

            var httpRestClient = Build.RestHttpClientBuilder
                .WithHttpClient(httpClientWrapper.Client)
                .Build();

            var request = new HttpCustomRequest("resource", HttpMethod.Post);
            var response = await resilietPolicyWrapper.ExecuteAsync(() => httpRestClient.ExecuteRequest<object>(request));

            await CallHelper.InvokeMultipleHttpRequests(httpRestClient, retryCount, HttpMethod.Post);

            Func<Task> action = async () => { await httpRestClient.ExecuteRequest<object>(request); };

            await Assert.ThrowsAsync<CustomException>(action);
        }

        private static RetryPolicySettings CreateRetryPolicySettings(int retryCount) => new ()
        {
            RetryCount = retryCount,
            InitialDetaySeconds = 2
        };

        private static CircuitBreakerPolicySettings CreateCircuitBreakerPolicySettings() => new()
        {
            DurationOfBreakInSeconds = 1,
            ExceptionsAllowedBeforeBreaking = 2
        };
    }
}
