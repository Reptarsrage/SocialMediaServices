using Newtonsoft.Json;
using System;
using System.Collections.Generic;

// Generated @ https://quicktype.io/?l=cs&r=json2csharp
namespace SocialMediaServices.Models.Facebook
{
    internal sealed class FacebookPostsResponse
    {
        public string Id { get; set; }
        public Posts Posts { get; set; }
    }

    internal sealed class Posts
    {
        public List<Post> Data { get; set; }
        public Paging Paging { get; set; }
    }

    internal sealed class Post
    {
        public DateTime CreatedTime { get; set; }
        public string Message { get; set; }
        public Attachments Attachments { get; set; }
        public string Id { get; set; }
        public string Story { get; set; }
    }

    internal sealed class CommentsResponse
    {
        public List<Comment> Data { get; set; }
        public Paging Paging { get; set; }
    }

    internal sealed class PagesResponse
    {
        public List<Page> Data { get; set; }
        public Paging Paging { get; set; }
    }

    internal sealed class Page
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    internal sealed class Comment
    {
        public From From { get; set; }
        public System.DateTime CreatedTime { get; set; }
        public string Id { get; set; }
        public string Message { get; set; }
    }

    internal sealed class From
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    internal sealed class Attachments
    {
        public List<OtherDatum> Data { get; set; }
    }

    internal sealed class OtherDatum
    {
        public Media Media { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Target Target { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
    }

    internal sealed class Media
    {
        public Image Image { get; set; }
    }

    internal sealed class Image
    {
        public string Src { get; set; }
        public long Height { get; set; }
        public long Width { get; set; }
    }

    internal sealed class Target
    {
        public string Id { get; set; }
        public string Url { get; set; }
    }

    internal sealed class Paging
    {
        public Cursors Cursors { get; set; }
        public string Next { get; set; }
    }

    internal sealed class Cursors
    {
        public string After { get; set; }
        public string Before { get; set; }
    }

    internal sealed class AccessTokenResponse
    {
        [JsonProperty("Access_Token")]
        public string AccessToken { get; set; }

        [JsonProperty("Token_Type")]
        public string TokenType { get; set; }
    }
}