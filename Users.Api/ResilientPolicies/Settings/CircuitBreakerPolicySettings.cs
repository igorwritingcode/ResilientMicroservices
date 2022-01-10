using System;

namespace Users.Api.ResilientPolicies.Settings
{
    public sealed class CircuitBreakerPolicySettings
    {
        public int ExceptionsAllowedBeforeBreaking { get; set; }

        public int DurationOfBreakInSeconds { get; set; }

        internal Action OnBreak { get; set; }

        internal Action OnReset { get; set; }

        internal Action OnHalfOpen { get; set; }

        public CircuitBreakerPolicySettings()
        {
            OnBreak = DoNothingOnBreak;
            OnReset = DoNothingOnReset;
            OnHalfOpen = DoNothingOnHalfOpen;
        }

        private static readonly Action DoNothingOnBreak = () => { };
        private static readonly Action DoNothingOnReset = () => { };
        private static readonly Action DoNothingOnHalfOpen = () => { };
    }
}
