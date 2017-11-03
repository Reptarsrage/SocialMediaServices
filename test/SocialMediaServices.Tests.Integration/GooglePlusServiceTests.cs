using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using SocialMediaServices;
using SocialMediaServices.Services;

namespace SocialMediaServices.Tests.Integration
{
    [TestFixture]
    public class GooglePlusServiceTests
    {
        private string _apiKey;
        private string videoId;
        private string postId;
        private string name;

        [OneTimeSetUp]
        public void Initialize()
        {
            _apiKey = TestParamUtility.GetParamOrDefault("GooglePlus.ApiKey");

            TestContext.WriteLine($"INFO: Using api key = '{_apiKey}'");
            Assert.False(string.IsNullOrWhiteSpace(_apiKey), "Please pass a valid api key using the cmd line arg '--params=GooglePlus.ApiKey=\"[API KEY]\"'");
        }

        [SetUp]
        public void SetUp()
        {
            videoId = "Ui8kbl270kM";
            name = "StoneMountain64";
            postId = "z13wd1f5xlets1ij204cjlugwwetv304rtw0k";
        }

        [Test]
        public async Task TestGooglePlusComments()
        {
            var service = new GooglePlusService(new SafeHttpClient(), new Configuration.GooglePlusConfiguration { ApiKey = _apiKey });
            var comments = await service.GetCommentsAsync(postId);

            Assert.IsNotNull(comments);
            Assert.IsNotEmpty(comments);

            TestContext.WriteLine($"Comments: {comments.Count}");
            foreach (var comment in comments.Take(100))
            {
                Assert.IsNotNull(comment.Content);
                Assert.IsNotNull(comment.Author);
                Assert.AreEqual("Google+", comment.Source);
            }
        }

        [Test]
        public async Task TestGooglePlusPost()
        {
            var service = new GooglePlusService(new SafeHttpClient(), new Configuration.GooglePlusConfiguration { ApiKey = _apiKey });
            var personId = await service.GetPersonIdAsync(name);

            Assert.IsNotNull(personId);

            var post = await service.GetActivityWithVideoAsync(personId, videoId);

            Assert.IsNotNull(post);
            Assert.IsFalse(string.IsNullOrWhiteSpace(post.Title));

            TestContext.WriteLine($"Post: {post.Title}");
        }
    }
}