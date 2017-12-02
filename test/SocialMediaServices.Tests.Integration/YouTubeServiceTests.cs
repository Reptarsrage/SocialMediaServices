using NUnit.Framework;
using SocialMediaServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SocialMediaServices.Tests.Integration
{
    [TestFixture]
    public class YouTubeServiceTests
    {
        private string _apiKey;
        private string videoId;
        private List<string> videoIds;
        private string videoUrl;
        private string channelName;
        private string playlistName;

        [OneTimeSetUp]
        public void Initialize()
        {
            _apiKey = TestParamUtility.GetParamOrDefault("YouTube.ApiKey");

            TestContext.WriteLine($"INFO: Using api key = '{_apiKey}'");
            Assert.False(string.IsNullOrWhiteSpace(_apiKey), "Please pass a valid api key using the cmd line arg '--params=YouTube.ApiKey=\"[API KEY]\"'");
        }

        [SetUp]
        public void SetUp()
        {
            videoId = "pY4xtxbvkxI";
            videoIds = new List<string> { "834qhOKX2Tg", "p2i-iA2GNu4", "s4p50QVzQKg", "sPZS9djzAzc" };
            videoUrl = $"https://www.youtube.com/watch?v={videoId}";
            channelName = "StoneMountain64";
            playlistName = "World's Best Clip of the Week";
        }

        [Test]
        public async Task TestVideoList()
        {
            var progressCounter = 0;
            var service = new YouTubeService(new SafeHttpClient(), new Configuration.YouTubeConfiguration { ApiKey = _apiKey });
            TestContext.WriteLine($"Videos: {string.Join(", ", videoIds)}");

            TestContext.Write("Getting Videos... ");
            var progressIndicator = new Progress<int>(i => Interlocked.Increment(ref progressCounter));
            var results = await service.GetVideoListAsync(videoIds, progressIndicator);
            TestContext.WriteLine("Done.");

            var videosCt = results.Count;
            TestContext.WriteLine($"\nVideos retrieved: { videosCt }");

            Assert.AreEqual(videoIds.Count, videosCt);
            Assert.Greater(progressCounter, 0);
            foreach (var video in results)
            {
                Assert.IsNotNull(video.Id);
                Assert.IsNotNull(video.Title);
            }
        }

        [Test]
        public async Task TestComments()
        {
            var progressCounter = 0;
            var service = new YouTubeService(new SafeHttpClient(), new Configuration.YouTubeConfiguration { ApiKey = _apiKey });
            var videoId = videoUrl;
            videoId = service.ParseVideoId(new Uri(videoId)) ?? videoId;
            var video = service.GetVideoAsync(videoId).Result;
            var totalCommentCount = video.CommentCount;

            TestContext.WriteLine($"Title: {video.Title}");
            TestContext.WriteLine($"Comments: {totalCommentCount}");

            TestContext.Write("Parsing comments... ");

            var progressIndicator = new Progress<int>(i => Interlocked.Increment(ref progressCounter));
            var results = await service.GetCommentsAsync(videoId, progressIndicator);
            TestContext.WriteLine("Done.");

            var commentsCt = results.Count;
            TestContext.WriteLine($"\nComments retrieved: { commentsCt }");

            Assert.Greater(commentsCt, 100);
            Assert.Greater(progressCounter, 0);
            foreach (var comment in results.Take(100))
            {
                Assert.IsNotNull(comment.Content);
                Assert.IsNotNull(comment.Author);
                Assert.AreEqual("YouTube", comment.Source);
            }
        }

        [Test]
        public async Task TestPlayList()
        {
            var service = new YouTubeService(new SafeHttpClient(), new Configuration.YouTubeConfiguration { ApiKey = _apiKey });

            var channelId = await service.GetChannelIdAsync(channelName);
            Assert.IsNotNull(channelId);

            var playlistId = await service.GetPlaylistIdAsync(channelId, playlistName);
            Assert.IsNotNull(playlistId);

            var progressCounter = 0;
            var progressIndicator = new Progress<int>(i => Interlocked.Increment(ref progressCounter));
            var playlistItems = await service.GetPlaylistVideosAsync(playlistId, progressIndicator);

            Assert.IsNotNull(playlistItems);
            Assert.IsNotEmpty(playlistItems);
            Assert.Greater(progressCounter, 0);

            TestContext.WriteLine($"Playlist items: {playlistItems.Count}");
            foreach (var item in playlistItems)
            {
                Assert.IsNotNull(item?.Id);
            }
        }

        [Test]
        public async Task TestChannelUploads()
        {
            var service = new YouTubeService(new SafeHttpClient(), new Configuration.YouTubeConfiguration { ApiKey = _apiKey });

            var channelId = await service.GetChannelIdAsync(channelName);
            Assert.IsNotNull(channelId);

            var progressCounter = 0;
            var progressIndicator = new Progress<int>(i => Interlocked.Increment(ref progressCounter));
            var channelItems = await service.GetChannelUploadsAsync(channelId, progressIndicator);

            Assert.IsNotNull(channelItems);
            Assert.IsNotEmpty(channelItems);
            Assert.Greater(progressCounter, 0);

            TestContext.WriteLine($"Channel items: {channelItems.Count}");
            foreach (var item in channelItems)
            {
                Assert.IsNotNull(item?.Id);
            }
        }
    }
}