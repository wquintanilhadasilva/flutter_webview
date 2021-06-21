using System.Collections.Generic;

namespace sso.Models
{
    public class SsoAuthenticationDefaults
    {
        public const string AuthenticationScheme = "External";
    }

    public static class SsoProviderType
    {
        public const string Google = "google";
        public const string Facebook = "facebook";
        public const string Twitter = "twitter";
        public const string Legacy = "legacy";
    }

    public class SsoProvider
    {
        public string Name { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public string Authority { get; set; }

        public bool Https { get; set; } = false;
    }

    public class SsoProviders
    {
        public List<SsoProvider> Providers { get; set; }
    }
}
