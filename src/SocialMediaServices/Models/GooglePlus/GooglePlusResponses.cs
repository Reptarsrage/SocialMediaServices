using System;
using System.Collections.Generic;

// Generated @ https://quicktype.io/?l=cs&r=json2csharp
namespace SocialMediaServices.Models.GooglePlus
{
    internal sealed class PeopleResponse
    {
        public List<Person> Items { get; set; }
        public string NextPageToken { get; set; }
    }

    internal sealed class Person
    {
        public string DisplayName { get; set; }
        public string Id { get; set; }
    }

    internal sealed class ActivityResponse
    {
        public List<Activity> Items { get; set; }
        public string NextPageToken { get; set; }
    }

    internal sealed class Activity
    {
        public ActivityObject Object { get; set; }
        public string Title { get; set; }
        public string Id { get; set; }
        public DateTime Published { get; set; }
        public string Url { get; set; }
    }

    internal sealed class ActivityObject
    {
        public Counter Plusoners { get; set; }
        public List<Attachment> Attachments { get; set; }
        public Counter Replies { get; set; }
        public Counter Resharers { get; set; }
    }

    internal sealed class Counter
    {
        public int TotalItems { get; set; }
    }

    internal sealed class Attachment
    {
        public string ObjectType { get; set; }
        public string Url { get; set; }
    }

    internal sealed class CommentsResponse
    {
        public List<Comment> Items { get; set; }
        public string NextPageToken { get; set; }
    }

    internal sealed class Comment
    {
        public string Id { get; set; }
        public Actor Actor { get; set; }
        public CommentObject Object { get; set; }
        public DateTime Published { get; set; }
    }

    internal sealed class Actor
    {
        public string DisplayName { get; set; }
    }

    internal sealed class CommentObject
    {
        public string Content { get; set; }
    }
}