using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YAKD.Helpers
{
    /// <summary>
    /// Small JSON files the application fetches at startup: the statistics endpoint and the latest released version.
    /// </summary>
    /// <remarks>
    /// Both files are published on several independent hosts. The addresses are tried in the listed order.
    /// A blocked host usually does not refuse the connection, it simply does not answer,
    /// so every attempt is limited to a couple of seconds instead of the HTTP client default of a hundred.
    /// </remarks>
    public static class RemoteContent
    {
        #region Fields

        /// <summary>
        /// Statistics endpoint used when the configuration cannot be downloaded from anywhere.
        /// </summary>
        public const string DefaultStatisticUrl = "https://yakd.xyz/api/statistic/create.php";

        private const int AttemptTimeoutMilliseconds = 2500;

        private const int PostTimeoutMilliseconds = 5000;

        // The GitHub API lives on api.github.com and returns the same repository file with the "raw" media type.
        // It requires a User-Agent and it allows only 60 requests per hour per address;
        // when that limit is hit the query falls through to the next mirror.
        private const string RawMediaType = "application/vnd.github.raw";

        private static readonly string[] ConfigUrls =
        {
            "https://api.github.com/repos/Jagailo/YetAnotherKeyDisplayer/contents/config.json",
            "https://raw.githubusercontent.com/Jagailo/YetAnotherKeyDisplayer/master/config.json",
            "https://gist.githubusercontent.com/Jagailo/4b38d8c8d2db4e1f08eb21a8ff307b2e/raw/config.json",
            "https://raw.githack.com/Jagailo/YetAnotherKeyDisplayer/master/config.json",
            "https://cdn.statically.io/gh/Jagailo/YetAnotherKeyDisplayer@master/config.json",
            "https://cdn.jsdelivr.net/gh/Jagailo/YetAnotherKeyDisplayer@master/config.json"
        };

        private static readonly string[] VersionUrls =
        {
            "https://api.github.com/repos/Jagailo/YetAnotherKeyDisplayer/contents/version.json",
            "https://raw.githubusercontent.com/Jagailo/YetAnotherKeyDisplayer/master/version.json",
            "https://gist.githubusercontent.com/Jagailo/e5b15287a2e3edc77c02ff4a58399392/raw/version.json",
            "https://raw.githack.com/Jagailo/YetAnotherKeyDisplayer/master/version.json",
            "https://cdn.statically.io/gh/Jagailo/YetAnotherKeyDisplayer@master/version.json",
            "https://cdn.jsdelivr.net/gh/Jagailo/YetAnotherKeyDisplayer@master/version.json"
        };

        private static readonly HttpClient Client = CreateClient();

        #endregion

        #region Methods

        /// <summary>
        /// Downloads the configuration with the statistics endpoint.
        /// </summary>
        /// <returns>JSON of the configuration file.</returns>
        public static Task<string> DownloadConfigAsync()
        {
            return DownloadFirstAvailableAsync(ConfigUrls);
        }

        /// <summary>
        /// Downloads the file with the latest released version.
        /// </summary>
        /// <returns>JSON of the version file.</returns>
        public static Task<string> DownloadVersionAsync()
        {
            return DownloadFirstAvailableAsync(VersionUrls);
        }

        /// <summary>
        /// Posts a JSON document to the given address.
        /// </summary>
        /// <param name="url">Statistics endpoint.</param>
        /// <param name="json">JSON document.</param>
        /// <returns>Response of the server, the caller has to dispose it.</returns>
        public static async Task<HttpResponseMessage> PostJsonAsync(string url, string json)
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var tokenSource = new CancellationTokenSource(PostTimeoutMilliseconds))
            {
                return await Client.PostAsync(url, content, tokenSource.Token).ConfigureAwait(false);
            }
        }

        #endregion

        #region Helpers

        private static async Task<string> DownloadFirstAvailableAsync(string[] urls)
        {
            Exception lastException = null;

            foreach (var url in urls)
            {
                try
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        // The GitHub API returns the file itself only with this media type, the other mirrors simply ignore the header
                        request.Headers.Accept.ParseAdd(RawMediaType);

                        using (var tokenSource = new CancellationTokenSource(AttemptTimeoutMilliseconds))
                        using (var response = await Client.SendAsync(request, tokenSource.Token).ConfigureAwait(false))
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                lastException = new HttpRequestException($"{url} responded with HTTP {(int)response.StatusCode} {response.ReasonPhrase}.");
                                Logger.Write(lastException);

                                continue;
                            }

                            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception exception)
                {
                    // Timeout, DNS failure, TLS failure or a host that is blocked entirely:
                    // the next address is tried, statistics must not depend on one of them
                    lastException = exception;
                    Logger.Write(exception);
                }
            }

            throw new HttpRequestException("None of the addresses answered: " + string.Join(", ", urls), lastException);
        }

        private static HttpClient CreateClient()
        {
            var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            var client = new HttpClient(handler) { Timeout = Timeout.InfiniteTimeSpan };

            // The GitHub API rejects requests without a User-Agent, the other mirrors ignore it
            client.DefaultRequestHeaders.UserAgent.ParseAdd("YAKD/" + Assembly.GetExecutingAssembly().GetName().Version);

            return client;
        }

        #endregion
    }
}
