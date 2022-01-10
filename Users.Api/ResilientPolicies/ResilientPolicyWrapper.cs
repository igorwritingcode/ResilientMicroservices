using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;
using Polly.Wrap;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Users.Api.Infrastructure.Exceptions;
using Users.Api.Infrastructure.Http;
using Users.Api.ResilientPolicies.Settings;

namespace Users.Api.ResilientPolicies
{
    public interface IResilientPolicyWrapper
    {
        Task<HttpCustomResponse<object>> ExecuteAsync(Func<Task<HttpCustomResponse<object>>> action);

        Action OnBreak { get; set; }

        Action OnReset { get; set; }

        Action OnHalfOpen { get; set; }
    }

    public class ResilientPolicyWrapper : IResilientPolicyWrapper
    {
        private readonly AsyncPolicyWrap<HttpCustomResponse<object>> _resilentPolicy;

        private readonly RetryPolicySettings _retryPolicySettings;

        private readonly CircuitBreakerPolicySettings _circuitBreakerSettings;

        public Action OnBreak
        {
            get => _circuitBreakerSettings.OnBreak;
            set => _circuitBreakerSettings.OnBreak = value;
        }

        public Action OnReset
        {
            get => _circuitBreakerSettings.OnReset;
            set => _circuitBreakerSettings.OnReset = value;
        }

        public Action OnHalfOpen
        {
            get => _circuitBreakerSettings.OnHalfOpen;
            set => _circuitBreakerSettings.OnHalfOpen = value;
        }

        public ResilientPolicyWrapper(
            IOptions<CircuitBreakerPolicySettings> circuitBreakerOptions,
            IOptions<RetryPolicySettings> retryPolicyOptions)
        {
            _circuitBreakerSettings = circuitBreakerOptions.Value;
            _retryPolicySettings = retryPolicyOptions.Value;
            _resilentPolicy = GetCircuitBreakerPolicy().WrapAsync(GetTransientErrorRetryPolicy());
        }

        public async Task<HttpCustomResponse<object>> ExecuteAsync(Func<Task<HttpCustomResponse<object>>> action)
        {
            var policy = _resilentPolicy.Outer as AsyncCircuitBreakerPolicy<HttpCustomResponse<object>>;

            if (policy.CircuitState == CircuitState.Open)
            {
                throw new CustomException("Service unavailable", HttpStatusCode.ServiceUnavailable);
            }

            return await _resilentPolicy.ExecuteAsync(() => action());
        }

        private AsyncRetryPolicy<HttpCustomResponse<object>> GetTransientErrorRetryPolicy()
        {
            return Policy.HandleResult<HttpCustomResponse<object>>
            (message => (int)message.StatusCode >= 503)
            .WaitAndRetryAsync(_retryPolicySettings.RetryCount, retryAttempt =>
            {
                Console.WriteLine($"Retrying because of transient error. Attempt {retryAttempt}");
                var backOffProvider = Backoff.ConstantBackoff(TimeSpan.FromSeconds(_retryPolicySettings.InitialDetaySeconds), retryAttempt);
                return backOffProvider.ToArray()[retryAttempt - 1];
            });
        }

        private AsyncCircuitBreakerPolicy<HttpCustomResponse<object>> GetCircuitBreakerPolicy()
        {
            return
                Policy
                .HandleResult<HttpCustomResponse<object>>(message => (int)message.StatusCode == 503)
                .CircuitBreakerAsync(
                    _circuitBreakerSettings.ExceptionsAllowedBeforeBreaking,
                    TimeSpan.FromSeconds(_circuitBreakerSettings.DurationOfBreakInSeconds),
                    (message, breakDelay) =>
                    {
                        Console.WriteLine($"Executing OnBreak Delegate");
                        _circuitBreakerSettings.OnBreak();
                    },
                    _circuitBreakerSettings.OnReset,
                    _circuitBreakerSettings.OnHalfOpen);
        }
    }
}
