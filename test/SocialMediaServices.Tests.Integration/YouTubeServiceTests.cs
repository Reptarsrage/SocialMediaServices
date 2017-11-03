using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SocialMediaServices;
using SocialMediaServices.Services;

namespace SocialMediaServices.Tests.Integration
{
    [TestFixture]
    public class YouTubeServiceTests
    {
        private string _apiKey;
        private string videoId;
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
            videoId = "Ui8kbl270kM";
            videoUrl = $"https://www.youtube.com/watch?v={videoId}";
            channelName = "StoneMountain64";
            playlistName = "World's Best Clip of the Week";
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

            var playlistItems = await service.GetPlaylistVideosAsync(playlistId);

            Assert.IsNotNull(playlistItems);
            Assert.IsNotEmpty(playlistItems);

            TestContext.WriteLine($"Playlist items: {playlistItems.Count}");
        }
    }
}