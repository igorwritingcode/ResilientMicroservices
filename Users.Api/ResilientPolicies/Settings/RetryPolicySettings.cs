namespace Users.Api.ResilientPolicies.Settings
{
    public class RetryPolicySettings
    {
        public int RetryCount { get; set; }

        public int InitialDetaySeconds { get; set; }
    }
}
