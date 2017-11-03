using Microsoft.AspNetCore.WebUtilities;
using SocialMediaServices.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialMediaServices.Models;
using GooglePlus = SocialMediaServices.Models.GooglePlus;

namespace SocialMediaServices.Services
{
    /// <inheritdoc />
    public class GooglePlusService : IGooglePlusService
    {
        private const string BASE_URL = "https://www.googleapis.com/plus/v1/";
        private readonly string _apiKey;
        private readonly ISafeHttpClient _client;

        /// <summary>
        /// Instantiates a new instance of the <see cref="GooglePlusService"/> class.
        /// </summary>
        /// <param name="client">Http Client</param>
        /// <param name="config">Configuration</param>
        public GooglePlusService(ISafeHttpClient client, GooglePlusConfiguration config)
        {
            _client = client;
            _apiKey = config.ApiKey;
        }

        /// <inheritdoc />
        public async Task<Activity> GetActivityWithVideoAsync(string personId, string videoId)
        {
            var post = await ActivityWithVideo(personId, videoId);

            return post == null ? null : new Activity
            {
                Title = post?.Title,
                Id = post?.Id,
                PlusOneCount = post?.Object?.Plusoners?.TotalItems ?? 0,
                ShareCount = post?.Object?.Resharers?.TotalItems ?? 0,
                ReplyCount = post?.Object?.Replies?.TotalItems ?? 0,
                PublishedDate = post?.Published ?? DateTime.MinValue,
                Url = post?.Url
            };
        }

        /// <inheritdoc />
        public async Task<string> GetPersonIdAsync(string name)
        {
            var url = BASE_URL + "people";
            var pageToken = string.Empty;
            url = QueryHelpers.AddQueryString(url, "query", $"\"{name}\"");
            url = QueryHelpers.AddQueryString(url, "key", _apiKey);
            url = QueryHelpers.AddQueryString(url, "maxResults", "50");
            url = QueryHelpers.AddQueryString(url, "fields", "items(displayName,id),nextPageToken");

            do
            {
                var pageUrl = string.IsNullOrWhiteSpace(pageToken) ? url : QueryHelpers.AddQueryString(url, "pageToken", pageToken);
                var parsedResp = await _client.GetAsync<GooglePlus.PeopleResponse>(pageUrl);
                var foundItem = parsedResp?.Items?.FirstOrDefault(item => item?.DisplayName?.Equals(name, StringComparison.OrdinalIgnoreCase) ?? false);
                if (foundItem != null)
                {
                    return foundItem.Id;
                }
                pageToken = parsedResp?.NextPageToken;
            } while (!string.IsNullOrWhiteSpace(pageToken));
            return null;
        }

        /// <inheritdoc />
        public async Task<IList<Comment>> GetCommentsAsync(string activityId)
        {
            var url = $"{BASE_URL}activities/{activityId}/comments";
            var list = new List<GooglePlus.Comment>();
            var pageToken = string.Empty;
            url = QueryHelpers.AddQueryString(url, "key", _apiKey);
            url = QueryHelpers.AddQueryString(url, "maxResults", "50");
            url = QueryHelpers.AddQueryString(url, "fields", "items(actor/displayName,id,object/content,published),nextPageToken");

            do
            {
                var pageUrl = string.IsNullOrWhiteSpace(pageToken) ? url : QueryHelpers.AddQueryString(url, "pageToken", pageToken);
                var parsedResp = await _client.GetAsync<GooglePlus.CommentsResponse>(pageUrl);
                list.AddRange(parsedResp?.Items ?? new List<GooglePlus.Comment>(0));

                pageToken = parsedResp?.NextPageToken;
            } while (!string.IsNullOrWhiteSpace(pageToken));

            return list.Select(item => new Comment
            {
                Author = item?.Actor?.DisplayName,
                Content = item?.Object?.Content,
                Id = item?.Id,
                PublishedDate = item?.Published ?? DateTime.MinValue,
                Source = "Google+"
            }).ToList();
        }

        private async Task<GooglePlus.Activity> ActivityWithVideo(string personId, string videoId)
        {
            var url = $"{BASE_URL}people/{personId}/activities/public";
            var pageToken = string.Empty;
            url = QueryHelpers.AddQueryString(url, "key", _apiKey);
            url = QueryHelpers.AddQueryString(url, "maxResults", "50");
            url = QueryHelpers.AddQueryString(url, "fields", "items(id,object(attachments(objectType,url),plusoners/totalItems,replies/totalItems,resharers/totalItems),published,title,url),nextPageToken");

            do
            {
                var pageUrl = string.IsNullOrWhiteSpace(pageToken) ? url : QueryHelpers.AddQueryString(url, "pageToken", pageToken);
                var parsedResp = await _client.GetAsync<GooglePlus.ActivityResponse>(pageUrl);
                var foundItem = parsedResp?.Items?.FirstOrDefault(item => item?.Object?.Attachments?.Any(att =>
                    (att?.ObjectType?.Equals("video", StringComparison.OrdinalIgnoreCase) ?? false) &&
                    (att?.Url?.Contains(videoId) ?? false)) ?? false);

                if (foundItem != null)
                {
                    return foundItem;
                }
                pageToken = parsedResp?.NextPageToken;
            } while (!string.IsNullOrWhiteSpace(pageToken));
            return null;
        }
    }
}