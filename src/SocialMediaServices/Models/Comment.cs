using System;

namespace SocialMediaServices.Models
{
    /// <summary>
    /// Comment
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Comment Author
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Comment Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Comment Content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Comment Published Date
        /// </summary>
        public DateTime PublishedDate { get; set; }

        /// <summary>
        /// Comment Source (YouTube, Google+, Facebook)
        /// </summary>
        public string Source { get; set; }
    }
}