using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SocialMediaServices
{
    /// <inheritdoc />
    public class SafeHttpClient : IDisposable, ISafeHttpClient
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerSettings _settings;

        /// <summary>
        /// Bytes
        /// </summary>
        public static int NetworkCt = 0;

        /// <inheritdoc />
        public SafeHttpClient()
        {
            _client = new HttpClient();

            _settings = new JsonSerializerSettings
            {
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
                DateParseHandling = DateParseHandling.None
            };
        }

        /// <inheritdoc />
        public async Task<T> GetAsync<T>(string url)
        {
            try
            {
                var resp = await _client.GetStringAsync(url);
                Interlocked.Add(ref NetworkCt, resp.Length); // TODO: Remove
                return JsonConvert.DeserializeObject<T>(resp, _settings);
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
                return default(T);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
