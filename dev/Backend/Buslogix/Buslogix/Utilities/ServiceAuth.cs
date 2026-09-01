namespace Buslogix.Utilities
{
    public static class ServiceAuth
    {
        public const string SchemeName = "ApiKey";
        public const string ApiKeyHeaderName = "X-Api-Key";
        public const string ServiceTokenClaimType = "token_type";
        public const string ServiceTokenClaimValue = "service";
        public const string ServiceNameClaimType = "service_name";
    }
}
