namespace ResilientPoliciesTests.DSL
{
    public static class Build
    {
        public static HttpClientWrapperBuilder HttpClientWrapperBuilder => new();

        public static ResilientPolicyWrapperBuilder ResilientPolicyWrapperBuilder => new();

        public static RestHttpClientBuilder RestHttpClientBuilder => new();
    }
}
