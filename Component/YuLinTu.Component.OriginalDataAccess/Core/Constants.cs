using System;

namespace YuLinTu.Component.OriginalDataAccess.Core
{
    /// <summary>
    /// Provides configuration keys and default values used by the original vector
    /// data access tool. The values are intentionally left empty so that they can
    /// be configured at runtime when the user supplies their <c>appid</c> and
    /// <c>appkey</c>.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// Name of the HTTP header that carries the application identifier.
        /// </summary>
        public const string AppIdHeader = "appid";

        /// <summary>
        /// Name of the HTTP header that carries the application secret.
        /// </summary>
        public const string AppKeyHeader = "appkey";

        /// <summary>
        /// Base address of the provincial platform API.
        /// </summary>
        public static readonly string BaseUrl = ""; // e.g. "https://example.com";

        /// <summary>
        /// Reads the configured app identifier from local storage.
        /// </summary>
        public static string AppIdValue => _appIdValue;
        private static string _appIdValue = string.Empty;

        /// <summary>
        /// Reads the configured app key from local storage.
        /// </summary>
        public static string AppKeyValue => _appKeyValue;
        private static string _appKeyValue = string.Empty;

        /// <summary>
        /// Allows the host application to update the <c>appid</c> and
        /// <c>appkey</c> values after prompting the user.
        /// </summary>
        public static void Configure(string appId, string appKey)
        {
            _appIdValue = appId ?? string.Empty;
            _appKeyValue = appKey ?? string.Empty;
        }
    }
}

