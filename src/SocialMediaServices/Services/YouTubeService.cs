using Microsoft.AspNetCore.WebUtilities;
using SocialMediaServices.Configuration;
using SocialMediaServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using YouTube = SocialMediaServices.Models.YouTube;

namespace SocialMediaServices.Services
{
    /// <inheritdoc />
    public class YouTubeService : IYouTubeService
    {
        private const string BASE_URL = "https://www.googleapis.com/youtube/v3/";
        private readonly ISafeHttpClient _client;
        private readonly string _apiKey;

        /// <summary>
        /// Instantiates a new instance of the <see cref="YouTubeService"/> class.
        /// </summary>
        /// <param name="client">Http Client</param>
        /// <param name="config">Configuration</param>
        public YouTubeService(ISafeHttpClient client, YouTubeConfiguration config)
        {
            _client = client;
            _apiKey = config.ApiKey;
        }

        /// <inheritdoc />
        public async Task<string> GetChannelIdAsync(string channelName)
        {
            var url = BASE_URL + "channels";
            url = QueryHelpers.AddQueryString(url, "part", "id");
            url = QueryHelpers.AddQueryString(url, "forUsername", channelName);
            url = QueryHelpers.AddQueryString(url, "key", _apiKey);
            url = QueryHelpers.AddQueryString(url, "maxResults", "1");
            url = QueryHelpers.AddQueryString(url, "fields", "items/id,nextPageToken");

            var parsedResp = await _client.GetAsync<YouTube.OnlyIdResponse>(url);
            return parsedResp?.Items?.FirstOrDefault()?.Id;
        }

        /// <inheritdoc />
        public async Task<string> GetPlaylistIdAsync(string channelId, string playlistName)
        {
            var pageToken = string.Empty;
            var url = BASE_URL + "playlists";
            url = QueryHelpers.AddQueryString(url, "part", "id,snippet");
            url = QueryHelpers.AddQueryString(url, "channelId", channelId);
            url = QueryHelpers.AddQueryString(url, "key", _apiKey);
            url = QueryHelpers.AddQueryString(url, "maxResults", "50");
            url = QueryHelpers.AddQueryString(url, "fields", "items(id,snippet/title),nextPageToken");

            do
            {
                var pageUrl = string.IsNullOrWhiteSpace(pageToken) ? url : QueryHelpers.AddQueryString(url, "pageToken", pageToken);
                var parsedResp = await _client.GetAsync<YouTube.PlaylistResponse>(pageUrl);
                var item = parsedResp?.Items?.FirstOrDefault(i => i?.Snippet?.Title?.Equals(playlistName, StringComparison.OrdinalIgnoreCase) ?? false);
                if (item != null)
                {
                    return item.Id;
                }
                pageToken = parsedResp?.NextPageToken;
            } while (!string.IsNullOrWhiteSpace(pageToken));
            return null;
        }

        /// <inheritdoc />
        public async Task<Video> GetVideoAsync(string id)
        {
            return (await ProcessVideos(new[] { id }, 1, null, new CancellationToken()))?.FirstOrDefault();
        }

        /// <inheritdoc />
        public string ParseVideoId(Uri uri)
        {
            var dict = QueryHelpers.ParseQuery(uri.Query);
            if (dict.ContainsKey("v"))
            {
                return dict["v"];
            }
            return null;
        }

        /// <inheritdoc />
        public async Task<IList<Comment>> GetCommentsAsync(string videoId)
        {
            var cts = new CancellationTokenSource();
            var ids = await IterateOverCommentThreads(videoId, null, cts.Token);
            return await BatchProcessComments(ids, cts.Token);
        }

        /// <inheritdoc />
        public async Task<IList<Comment>> GetCommentsAsync(string videoId, IProgress<int> progress)
        {
            var cts = new CancellationTokenSource();
            var ids = await IterateOverCommentThreads(videoId, progress, cts.Token);
            return await BatchProcessComments(ids, cts.Token);
        }

        /// <inheritdoc />
        public async Task<IList<Comment>> GetCommentsAsync(string videoId, IProgress<int> progress, CancellationToken ct)
        {
            var ids = await IterateOverCommentThreads(videoId, progress, ct);
            return await BatchProcessComments(ids, ct);
        }

        /// <inheritdoc />
        public async Task<IList<Comment>> GetCommentsAsync(string videoId, CancellationToken ct)
        {
            var ids = await IterateOverCommentThreads(videoId, null, ct);
            return await BatchProcessComments(ids, ct);
        }

        /// <inheritdoc />
        public async Task<IList<Video>> GetPlaylistVideosAsync(string playlistId)
        {
            var cts = new CancellationTokenSource();
            return await GetPlaylistVideosAsync(playlistId, null, cts.Token);
        }

        /// <inheritdoc />
        public async Task<IList<Video>> GetPlaylistVideosAsync(string playlistId, IProgress<int> progress, CancellationToken ct)
        {
            var pageToken = string.Empty;
            var list = new List<string>();
            var url = BASE_URL + "playlistItems";
            url = QueryHelpers.AddQueryString(url, "part", "contentDetails");
            url = QueryHelpers.AddQueryString(url, "playlistId", playlistId);
            url = QueryHelpers.AddQueryString(url, "key", _apiKey);
            url = QueryHelpers.AddQueryString(url, "maxResults", "50");
            url = QueryHelpers.AddQueryString(url, "fields", "items/contentDetails/videoId,nextPageToken");

            do
            {
                ct.ThrowIfCancellationRequested();

                // Get next set of items
                var pageUrl = string.IsNullOrWhiteSpace(pageToken) ? url : QueryHelpers.AddQueryString(url, "pageToken", pageToken);
                var parsedResp = await _client.GetAsync<YouTube.OnlyIdResponse>(pageUrl);
                list.AddRange(parsedResp?.Items?.Where(item => item?.ContentDetails?.VideoId != null).Select(item => item.ContentDetails.VideoId) ?? new string[0]);

                pageToken = parsedResp?.NextPageToken;
            } while (!string.IsNullOrWhiteSpace(pageToken));
            return await GetVideoListAsync(list, progress, ct);
        }

        /// <inheritdoc />
        public async Task<IList<Video>> GetPlaylistVideosAsync(string playlistId, CancellationToken ct)
        {
            return await GetPlaylistVideosAsync(playlistId, null, ct);
        }

        /// <inheritdoc />
        public async Task<IList<Video>> GetPlaylistVideosAsync(string playlistId, IProgress<int> progress)
        {
            var cts = new CancellationTokenSource();
            return await GetPlaylistVideosAsync(playlistId, progress, cts.Token);
        }

        /// <inheritdoc />
        public async Task<IList<Video>> GetChannelUploadsAsync(string channelId, CancellationToken ct)
        {
            return await GetChannelUploadsAsync(channelId, null, ct);
        }

        /// <inheritdoc />
        public async Task<IList<Video>> GetChannelUploadsAsync(string channelId)
        {
            var cts = new CancellationTokenSource();
            return await GetChannelUploadsAsync(channelId, null, cts.Token);
        }

        /// <inheritdoc />
        public async Task<IList<Video>> GetChannelUploadsAsync(string channelId, IProgress<int> progress, CancellationToken ct)
        {
            var url = BASE_URL + "channels";
            url = QueryHelpers.AddQueryString(url, "id", channelId);
            url = QueryHelpers.AddQueryString(url, "part", "contentDetails");
            url = QueryHelpers.AddQueryString(url, "key", _apiKey);

            var channelResponse = await _client.GetAsync<YouTube.OnlyIdResponse>(url);
            var channelItem = channelResponse?.Items?.FirstOrDefault();
            if (channelItem == null)
            {
                return null;
            }
            var uploadId = channelItem.ContentDetails.RelatedPlaylists.Uploads;
            return await GetPlaylistVideosAsync(uploadId, progress, ct);
        }

        /// <inheritdoc />
        public async Task<IList<Video>> GetChannelUploadsAsync(string channelId, IProgress<int> progress)
        {
            var cts = new CancellationTokenSource();
            return await GetChannelUploadsAsync(channelId, progress, cts.Token);
        }

        /// <inheritdoc />
        public async Task<IList<Video>> GetVideoListAsync(IEnumerable<string> ids)
        {
            var cts = new CancellationTokenSource();
            return await BatchProcessVideos(ids, 50, null, cts.Token);
        }

        /// <inheritdoc />
        public async Task<IList<Video>> GetVideoListAsync(IEnumerable<string> ids, IProgress<int> progress, CancellationToken ct)
        {
            return await BatchProcessVideos(ids, 50, progress, ct);
        }

        /// <inheritdoc />
        public async Task<IList<Video>> GetVideoListAsync(IEnumerable<string> ids, CancellationToken ct)
        {
            return await BatchProcessVideos(ids, 50, null, ct);
        }

        /// <inheritdoc />
        public async Task<IList<Video>> GetVideoListAsync(IEnumerable<string> ids, IProgress<int> progress)
        {
            var cts = new CancellationTokenSource();
            return await BatchProcessVideos(ids, 50, progress, cts.Token);
        }

        private async Task<IList<string>> IterateOverCommentThreads(string videoId, IProgress<int> progress, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var pageToken = string.Empty;
            var list = new List<string>();
            var url = BASE_URL + "commentThreads";
            url = QueryHelpers.AddQueryString(url, "part", "id,replies");
            url = QueryHelpers.AddQueryString(url, "videoId", videoId);
            url = QueryHelpers.AddQueryString(url, "key", _apiKey);
            url = QueryHelpers.AddQueryString(url, "maxResults", "100");
            url = QueryHelpers.AddQueryString(url, "fields", "items(id,replies(comments(id,snippet(publishedAt,textDisplay,authorDisplayName)))),nextPageToken");

            do
            {
                ct.ThrowIfCancellationRequested();

                // Get next set of threads
                var pageUrl = string.IsNullOrWhiteSpace(pageToken) ? url : QueryHelpers.AddQueryString(url, "pageToken", pageToken);
                var parsedResp = await _client.GetAsync<YouTube.CommentThreadResponse>(pageUrl);

                // Addc comments
                var comments = parsedResp?.Items?.Select(item => item.Id).Where(id => !string.IsNullOrWhiteSpace(id));
                if (comments != null && comments.Any())
                {
                    list.AddRange(comments);

                }

                // Add replies
                var replies = parsedResp?.Items?.SelectMany(item => item.Replies?.Comments?.Select(c => c.Id) ?? new string[0]).Where(id => !string.IsNullOrWhiteSpace(id));
                if (replies != null && replies.Any())
                {
                    list.AddRange(replies);

                }

                // Progress
                progress?.Report(list.Count);

                pageToken = parsedResp?.NextPageToken;
            } while (!string.IsNullOrWhiteSpace(pageToken));
            return list;
        }

        private async Task<IList<string>> IterateOverCommentReplies(string parentId, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var pageToken = string.Empty;
            var list = new List<string>();
            var url = BASE_URL + "comments";
            url = QueryHelpers.AddQueryString(url, "part", "id");
            url = QueryHelpers.AddQueryString(url, "parentId", parentId);
            url = QueryHelpers.AddQueryString(url, "key", _apiKey);
            url = QueryHelpers.AddQueryString(url, "maxResults", "100");
            url = QueryHelpers.AddQueryString(url, "fields", "items/id,nextPageToken");

            do
            {
                ct.ThrowIfCancellationRequested();

                var pageUrl = string.IsNullOrWhiteSpace(pageToken) ? url : QueryHelpers.AddQueryString(url, "pageToken", pageToken);
                var parsedResp = await _client.GetAsync<YouTube.OnlyIdResponse>(pageUrl);
                list.AddRange(parsedResp?.Items?.Select(item => item?.Id) ?? new string[0]);
                pageToken = parsedResp?.NextPageToken;
            } while (!string.IsNullOrWhiteSpace(pageToken));
            return list;
        }

        private async Task<IList<Comment>> BatchProcessComments(IList<string> commentIds, CancellationToken ct)
        {
            var pageToken = string.Empty;
            var queue = Chunkify(commentIds, 25);
            var results = await Task.WhenAll(queue.Select(chunk => ProcessComments(chunk, ct)));
            return results.SelectMany(chunk => chunk.Select(comment => comment)).ToList();
        }

        private async Task<IList<Comment>> ProcessComments(IEnumerable<string> commentIds, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var url = BASE_URL + "comments";
            url = QueryHelpers.AddQueryString(url, "part", "id,snippet");
            url = QueryHelpers.AddQueryString(url, "id", string.Join(",", commentIds));
            url = QueryHelpers.AddQueryString(url, "key", _apiKey);
            url = QueryHelpers.AddQueryString(url, "fields", "items(id,snippet(authorDisplayName,publishedAt,textDisplay))");
            url = QueryHelpers.AddQueryString(url, "textFormat", "plainText");

            var parsedResp = await _client.GetAsync<YouTube.CommentResponse>(url);
            return parsedResp?.Items?.Select(item => new Comment
            {
                Id = item?.Id,
                PublishedDate = item?.Snippet?.PublishedAt ?? DateTime.MinValue,
                Source = "YouTube",
                Author = item?.Snippet?.AuthorDisplayName,
                Content = item?.Snippet?.TextDisplay

            })?.ToList() ?? new List<Comment>();
        }

        private async Task<IList<Video>> BatchProcessVideos(IEnumerable<string> videoIds, int chunkSize, IProgress<int> progress, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var pageToken = string.Empty;
            var queue = Chunkify(videoIds, chunkSize);
            var count = 0;
            var result = await Task.WhenAll(queue.Select(async chunk =>
           {
               var results = await ProcessVideos(chunk, chunkSize, progress, ct);
               Interlocked.Add(ref count, results.Count());
               progress?.Report(count);
               return results;
           }));
            progress?.Report(100);
            return result.SelectMany(chunk => chunk).ToList();
        }

        private async Task<IList<Video>> ProcessVideos(IEnumerable<string> videoIds, int chunkSize, IProgress<int> progress, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            var url = BASE_URL + "videos";
            url = QueryHelpers.AddQueryString(url, "part", "id,statistics,snippet,contentDetails");
            url = QueryHelpers.AddQueryString(url, "maxResults", chunkSize.ToString());
            url = QueryHelpers.AddQueryString(url, "id", string.Join(",", videoIds));
            url = QueryHelpers.AddQueryString(url, "key", _apiKey);
            url = QueryHelpers.AddQueryString(url, "fields", "items(id,snippet(channelId,publishedAt,thumbnails,title),statistics,contentDetails(duration))");

            var parsedResp = await _client.GetAsync<YouTube.VideoResponse>(url);
            var items = parsedResp?.Items;
            return items?.Select(item => new Video
            {
                Id = item.Id,
                Duration = XmlConvert.ToTimeSpan(item.ContentDetails?.Duration ?? "PT0M0S").TotalSeconds,
                PublishedDate = item.Snippet?.PublishedAt ?? DateTime.MinValue,
                ChannelId = item.Snippet?.ChannelId,
                Title = item.Snippet?.Title,
                ViewCount = item.Statistics?.ViewCount ?? 0,
                LikeCount = item.Statistics?.LikeCount ?? 0,
                DislikeCount = item.Statistics?.DislikeCount ?? 0,
                FavoriteCount = item.Statistics?.FavoriteCount ?? 0,
                CommentCount = item.Statistics?.CommentCount ?? 0,
                ImageDefault = new Image
                {
                    Url = item.Snippet?.Thumbnails?.Default?.Url,
                    Width = item.Snippet?.Thumbnails?.Default?.Width ?? 0,
                    Height = item.Snippet?.Thumbnails?.Default?.Height ?? 0,
                },
                ImageHigh = new Image
                {
                    Url = item.Snippet?.Thumbnails?.High?.Url,
                    Width = item.Snippet?.Thumbnails?.High?.Width ?? 0,
                    Height = item.Snippet?.Thumbnails?.High?.Height ?? 0,
                },
                ImageMaxRes = new Image
                {
                    Url = item.Snippet?.Thumbnails?.Maxres?.Url,
                    Width = item.Snippet?.Thumbnails?.Maxres?.Width ?? 0,
                    Height = item.Snippet?.Thumbnails?.Maxres?.Height ?? 0,
                },
                ImageMedium = new Image
                {
                    Url = item.Snippet?.Thumbnails?.Medium?.Url,
                    Width = item.Snippet?.Thumbnails?.Medium?.Width ?? 0,
                    Height = item.Snippet?.Thumbnails?.Medium?.Height ?? 0,
                },
                ImageStandard = new Image
                {
                    Url = item.Snippet?.Thumbnails?.Standard?.Url,
                    Width = item.Snippet?.Thumbnails?.Standard?.Width ?? 0,
                    Height = item.Snippet?.Thumbnails?.Standard?.Height ?? 0,
                }
            })?.ToList() ?? new List<Video>();
        }

        private IEnumerable<IEnumerable<T>> Chunkify<T>(IEnumerable<T> list, int chunkSize)
        {
            return list
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value));
        }
    }
}