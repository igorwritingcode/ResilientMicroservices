using Microsoft.Extensions.Options;
using Users.Api.ResilientPolicies;
using Users.Api.ResilientPolicies.Settings;

namespace ResilientPoliciesTests.DSL
{
    public class ResilientPolicyWrapperBuilder
    {
        IOptions<CircuitBreakerPolicySettings> _circuitBreakerPolicySettings;
        IOptions<RetryPolicySettings> _retryPolicySettings;

        public ResilientPolicyWrapperBuilder WithBreakerSettings(IOptions<CircuitBreakerPolicySettings> circuitBreakerPolicySettings)
        {
            _circuitBreakerPolicySettings = circuitBreakerPolicySettings;
            return this;
        }

        public ResilientPolicyWrapperBuilder WithRetrySettings(IOptions<RetryPolicySettings> retryPolicySettings)
        {
            _retryPolicySettings = retryPolicySettings;
            return this;
        }

        public ResilientPolicyWrapper Build()
        {
            return new ResilientPolicyWrapper(_circuitBreakerPolicySettings, _retryPolicySettings);
        }
    }
}
