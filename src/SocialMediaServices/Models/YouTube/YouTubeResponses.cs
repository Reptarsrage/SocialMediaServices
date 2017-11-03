using System;
using System.Collections.Generic;

// Generated @ https://quicktype.io/?l=cs&r=json2csharp
namespace SocialMediaServices.Models.YouTube
{
    internal sealed class CommentResponse
    {
        public List<Comment> Items { get; set; }
    }

    internal sealed class Comment
    {
        public string Id { get; set; }
        public CommentSnippet Snippet { get; set; }
    }

    internal sealed class CommentSnippet
    {
        public DateTime PublishedAt { get; set; }
        public string AuthorDisplayName { get; set; }
        public string TextDisplay { get; set; }
    }

    internal sealed class OnlyIdResponse
    {
        public List<Item> Items { get; set; }
        public string NextPageToken { get; set; }
    }

    internal sealed class Item
    {
        public string Id { get; set; }
    }

    internal sealed class VideoResponse
    {
        public List<Video> Items { get; set; }
    }

    internal sealed class Video
    {
        public VideoSnippet Snippet { get; set; }
        public Statistics Statistics { get; set; }
    }

    internal sealed class VideoSnippet
    {
        public DateTime PublishedAt { get; set; }
        public string ChannelId { get; set; }
        public Thumbnails Thumbnails { get; set; }
        public string Title { get; set; }
    }

    internal sealed class Thumbnails
    {
        public Image High { get; set; }
        public Image Medium { get; set; }
        public Image Default { get; set; }
        public Image Maxres { get; set; }
        public Image Standard { get; set; }
    }

    internal sealed class Image
    {
        public string Url { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }

    internal sealed class Statistics
    {
        public int DislikeCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public int FavoriteCount { get; set; }
        public int ViewCount { get; set; }
    }

    internal sealed class PlaylistResponse
    {
        public List<Playlist> Items { get; set; }
        public string NextPageToken { get; set; }
    }

    internal sealed class Playlist
    {
        public string Id { get; set; }
        public PlaylistSnippet Snippet { get; set; }
    }

    internal sealed class PlaylistSnippet
    {
        public string Title { get; set; }
    }
}