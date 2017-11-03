using System.Threading.Tasks;

namespace SocialMediaServices
{
    /// <summary>
    /// Wraps the Http Client so thart errors are caught
    /// </summary>
    public interface ISafeHttpClient
    {
        /// <summary>
        /// Disposes the underlying <see cref="System.Net.Http.HttpClient"/>
        /// </summary>
        void Dispose();

        /// <summary>
        /// <see cref="System.Net.Http.HttpClient.GetAsync(string)"/>
        /// </summary>
        Task<T> GetAsync<T>(string url);
    }
}