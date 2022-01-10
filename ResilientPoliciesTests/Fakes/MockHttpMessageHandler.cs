using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ResilientPoliciesTests.Fakes
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _responseCode;
        private readonly TimeSpan _latency;

        public long NumberOfCalls => _numberOfCalls;
        private long _numberOfCalls = 0;

        public MockHttpMessageHandler(HttpStatusCode responseCode, TimeSpan latency)
        {
            _latency = latency;
            _responseCode = responseCode;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref _numberOfCalls);

            await Task.Delay(_latency, cancellationToken);

            var result = new HttpResponseMessage
            {
                RequestMessage = request,
                StatusCode = _responseCode
            };

            return await Task.FromResult(result);
        }
    }
}
