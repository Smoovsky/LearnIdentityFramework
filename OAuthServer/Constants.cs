namespace OAuthServer
{
    public static class Constants
    {
        public const string Audiance = "https://localhost:5001";
        public const string Issuer = Audiance; // issue to self for now
        public const string Secret = "SampleSecretLongEnough";
    }
}
