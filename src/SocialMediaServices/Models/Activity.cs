using System;

namespace SocialMediaServices.Models
{
    /// <summary>
    /// Google+ Activity
    /// </summary>
    public class Activity
    {
        /// <summary>
        /// Activity Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Activity Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Activity Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Activity Published Date
        /// </summary>
        public DateTime PublishedDate { get; set; }

        /// <summary>
        ///Activity  Reply Count
        /// </summary>
        public int ReplyCount { get; set; }

        /// <summary>
        /// Activity PlusOne Count
        /// </summary>
        public int PlusOneCount { get; set; }

        /// <summary>
        /// Activity Share Count
        /// </summary>
        public int ShareCount { get; set; }
    }
}