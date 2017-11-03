using System;

namespace SocialMediaServices.Models
{
    /// <summary>
    /// Facebook Post
    /// </summary>
    public class Post
    {
        /// <summary>
        /// Facebook Post Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Facebook Post Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Facebook Post Published Date
        /// </summary>
        public DateTime PublishedDate { get; set; }
    }
}