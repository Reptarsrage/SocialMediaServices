using Microsoft.AspNetCore.WebUtilities;
using SocialMediaServices.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialMediaServices.Models;
using Facebook = SocialMediaServices.Models.Facebook;

namespace SocialMediaServices.Services
{
    /// <inheritdoc />
    public class FaceBookService : IFaceBookService
    {
        private const string BASE_URL = "https://graph.facebook.com";
        private const string VERSION = "v2.10";

        private readonly ISafeHttpClient _client;
        private readonly string _appId;
        private readonly string _appSecret;
        private string accessToken;
        private DateTime expires;

        /// <summary>
        /// Instantiates a new instance of the <see cref="FaceBookService"/> class.
        /// </summary>
        /// <param name="client">Http Client</param>
        /// <param name="config"></param>
        public FaceBookService(ISafeHttpClient client, FaceBookConfiguration config)
        {
            _client = client;
            _appId = config.AppId;
            _appSecret = config.AppSecret;
        }

        /// <inheritdoc />
        public async Task<IList<Post>> GetPostsAsync(string pageId)
        {
            var token = await BootStrap(); ;
            var list = new List<Facebook.Post>();
            var url = $"{BASE_URL}/{VERSION}/{pageId}";
            url = QueryHelpers.AddQueryString(url, "access_token", token);
            url = QueryHelpers.AddQueryString(url, "fields", "posts{attachments,id,created_time,message,story}");

            do
            {
                var resp = await _client.GetAsync<Facebook.FacebookPostsResponse>(url);
                list.AddRange(resp?.Posts?.Data ?? new List<Facebook.Post>(0));

                url = resp?.Posts?.Paging?.Next;
            } while (!string.IsNullOrWhiteSpace(url));

            return list.Select(item => new Post
            {
                Id = item?.Id,
                PublishedDate = item?.CreatedTime ?? DateTime.MinValue,
                Title = item?.Story
            }).ToList();
        }

        /// <inheritdoc />
        public async Task<IList<Comment>> GetCommentsAsync(string postId)
        {
            var token = await BootStrap();
            var list = new List<Facebook.Comment>();
            var url = $"{BASE_URL}/{VERSION}/{postId}/comments";
            url = QueryHelpers.AddQueryString(url, "access_token", token);

            do
            {
                var resp = await _client.GetAsync<Facebook.CommentsResponse>(url);
                list.AddRange(resp?.Data ?? new List<Facebook.Comment>(0));

                url = resp?.Paging?.Next;
            } while (!string.IsNullOrWhiteSpace(url));
            return list.Select(item => new Comment
            {
                Author = item?.From?.Name,
                Content = item?.Message,
                Id = item?.Id,
                PublishedDate = item?.CreatedTime ?? DateTime.MinValue,
                Source = "Facebook"
            }).ToList();
        }

        /// <inheritdoc />
        public async Task<IList<Page>> GetPagesAsync(string name)
        {
            var token = await BootStrap();
            var nextPageUrl = string.Empty;
            var list = new List<Facebook.Page>();
            var url = $"{BASE_URL}/{VERSION}/search";
            url = QueryHelpers.AddQueryString(url, "access_token", token);
            url = QueryHelpers.AddQueryString(url, "q", name);
            url = QueryHelpers.AddQueryString(url, "type", "page");


            var resp = await _client.GetAsync<Facebook.PagesResponse>(url);
            list.AddRange(resp?.Data ?? new List<Facebook.Page>(0));

            return list.Select(item => new Page
            {
                Id = item?.Id,
                Name = item?.Name
            }).ToList();
        }

        /// <summary>
        /// Get Access Token
        /// </summary>
        private async Task<string> BootStrap()
        {
            if (!string.IsNullOrWhiteSpace(accessToken) && expires >= DateTime.Now)
            {
                return accessToken;
            }

            var url = $"{BASE_URL}/oauth/access_token";
            url = QueryHelpers.AddQueryString(url, "client_id", _appId);
            url = QueryHelpers.AddQueryString(url, "client_secret", _appSecret);
            url = QueryHelpers.AddQueryString(url, "grant_type", "client_credentials");


            var resp = await _client.GetAsync<Facebook.AccessTokenResponse>(url);
            accessToken = resp.AccessToken;
            expires = DateTime.Now + TimeSpan.FromMinutes(30);
            return accessToken;
        }
    }
}