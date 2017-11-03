using System;

namespace SocialMediaServices.Models
{
    /// <summary>
    /// YouTube Video
    /// </summary>
    public class Video
    {
        /// <summary>
        /// Video Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// YouTube Video Published Date
        /// </summary>
        public DateTime PublishedDate { get; set; }

        /// <summary>
        /// YouTube Video Channel Id
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// YouTube Video Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// YouTube Video View Count
        /// </summary>
        public int ViewCount { get; set; }

        /// <summary>
        /// YouTube Video Like Count
        /// </summary>
        public int LikeCount { get; set; }

        /// <summary>
        /// YouTube Video Dislike Count
        /// </summary>
        public int DislikeCount { get; set; }

        /// <summary>
        /// YouTube Video Favorite Count
        /// </summary>
        public int FavoriteCount { get; set; }

        /// <summary>
        /// YouTube Video Comment Count
        /// </summary>
        public int CommentCount { get; set; }

        /// <summary>
        /// YouTube Video Thumbnail Image Default Resolution
        /// </summary>
        public Image ImageDefault { get; set; }

        /// <summary>
        /// YouTube Video Thumbnail Image Medium Resolution
        /// </summary>
        public Image ImageMedium { get; set; }

        /// <summary>
        /// YouTube Video Thumbnail Image Max Resolution
        /// </summary>
        public Image ImageMaxRes { get; set; }

        /// <summary>
        /// YouTube Video Thumbnail Image Standard Resolution
        /// </summary>
        public Image ImageStandard { get; set; }

        /// <summary>
        /// YouTube Video Thumbnail Image High Resolution
        /// </summary>
        public Image ImageHigh { get; set; }
    }
}