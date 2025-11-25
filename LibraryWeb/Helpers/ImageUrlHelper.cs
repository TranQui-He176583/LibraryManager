using Microsoft.Extensions.Configuration;

namespace LibraryWeb.Helpers
{
    public static class ImageUrlHelper
    {
        private static string? _apiBaseUrl;

        public static void Initialize(IConfiguration configuration)
        {
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
        }

        public static string? GetFullImageUrl(string? relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
                return null;

            if (relativeUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                relativeUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return relativeUrl;
            }
            if (relativeUrl.StartsWith("/"))
            {
                var baseUrl = _apiBaseUrl ?? "http://localhost:5000";
                return baseUrl + relativeUrl;
            }

            return relativeUrl;
        }
    }
}



