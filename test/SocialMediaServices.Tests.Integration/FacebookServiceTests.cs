using NUnit.Framework;
using SocialMediaServices.Configuration;
using SocialMediaServices.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMediaServices.Tests.Integration
{
    [TestFixture]
    public class FacebookServiceTests
    {
        private string _appId;
        private string _appSecret;


        [OneTimeSetUp]
        public void Initialize()
        {
            _appId = TestParamUtility.GetParamOrDefault("Facebook.AppId");
            _appSecret = TestParamUtility.GetParamOrDefault("Facebook.AppSecret");

            TestContext.WriteLine($"INFO: Using appId = '{_appId}'");
            TestContext.WriteLine($"INFO: Using appSecret {_appSecret}");
            Assert.False(string.IsNullOrWhiteSpace(_appId), "Please pass a valid app id using the cmd line arg '--params=Facebook.AppId=\"[APP ID]\"'");
            Assert.False(string.IsNullOrWhiteSpace(_appSecret), "Please pass a valid app secret using the cmd line arg '--params=Facebook.AppSecret=\"[APP SECRET]\"'");
        }

        [Test]
        public async Task TestCommentsFaceBookService()
        {
            var service = new FaceBookService(new SafeHttpClient(), new FaceBookConfiguration { AppId = _appId, AppSecret = _appSecret });

            var pageIds = await service.GetPagesAsync("WorldsBestOfTheBest");
            Assert.IsNotNull(pageIds);
            Assert.IsNotEmpty(pageIds);

            var postIds = await service.GetPostsAsync(pageIds.FirstOrDefault().Id);
            Assert.IsNotNull(postIds);
            Assert.IsNotEmpty(postIds);

            var comments = await service.GetCommentsAsync(postIds.FirstOrDefault().Id);
            Assert.IsNotNull(comments);
            Assert.IsNotEmpty(comments);

            TestContext.WriteLine($"Comments: {comments.Count}");
            foreach (var comment in comments.Take(100))
            {
                Assert.IsNotNull(comment.Content);
                Assert.IsNotNull(comment.Author);
                Assert.AreEqual("Facebook", comment.Source);
            }
        }
    }
}