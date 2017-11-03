namespace SocialMediaServices.Configuration
{
    /// <summary>
    /// Configuration Needed for Facebook graph API Service
    /// </summary>
    public class FaceBookConfiguration
    {
        /// <summary>
        /// API App Id
        /// <para />
        /// https://developers.facebook.com/docs/graph-api
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// API App Secret
        /// </summary>
        public string AppSecret { get; set; }
    }
}