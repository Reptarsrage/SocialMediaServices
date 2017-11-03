using Moq;
using NUnit.Framework;
using SocialMediaServices.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialMediaServices.Services;
using Facebook = SocialMediaServices.Models.Facebook;

namespace SocialMediaServices.Tests.Unit
{
    [TestFixture]
    public class FacebookServiceTests
    {
        private readonly string _appSecret = "TEST_API_KEY";
        private readonly string _appId = "TEST_API_KEY";
        private FaceBookConfiguration config;

        private Facebook.PagesResponse _mockPagesResponse;
        private Facebook.PagesResponse _mockPagesResponseSecondPage;
        private Facebook.AccessTokenResponse _mockAccessTokenResponse;
        private Facebook.FacebookPostsResponse _mockFacebookPostsResponse;
        private Facebook.FacebookPostsResponse _mockFacebookPostsResponseSecondPage;
        private Facebook.CommentsResponse _mockCommentsResponse;
        private Facebook.CommentsResponse _mockCommentsResponseSecondPage;

        [SetUp]
        public void SetUp()
        {
            _mockAccessTokenResponse = new Facebook.AccessTokenResponse
            {
                AccessToken = "Test Token",
                TokenType = "Bearer"
            };
            _mockPagesResponse = new Facebook.PagesResponse
            {
                Data = new List<Facebook.Page>
                {
                    new Facebook.Page
                    {
                        Id = "1",
                        Name = "PageName1"
                    },
                    new Facebook.Page
                    {
                        Id = "2",
                        Name = "PageName2"
                    },
                    new Facebook.Page
                    {
                        Id = "3",
                        Name = "PageName3"
                    }
                }
            };
            _mockPagesResponseSecondPage = new Facebook.PagesResponse
            {
                Data = new List<Facebook.Page>
                {
                    new Facebook.Page
                    {
                        Id = "4",
                        Name = "PageName4"
                    }
                }
            };
            _mockFacebookPostsResponse = new Facebook.FacebookPostsResponse
            {
                Posts = new Facebook.Posts
                {
                    Data = new List<Facebook.Post>
                    {
                        new Facebook.Post
                        {
                            Id = "1",
                            Story = "Story1"
                        },
                        new Facebook.Post
                        {
                            Id = "2",
                            Story = "Story2"
                        },
                        new Facebook.Post
                        {
                            Id = "3",
                            Story = "Story3"
                        }
                    }
                }
            };
            _mockFacebookPostsResponseSecondPage = new Facebook.FacebookPostsResponse
            {
                Posts = new Facebook.Posts
                {
                    Data = new List<Facebook.Post>
                    {
                        new Facebook.Post
                        {
                            Id = "4",
                            Story = "Story4"
                        }
                    }
                }
            };
            _mockCommentsResponse = new Facebook.CommentsResponse
            {
                Data = new List<Facebook.Comment>
                {
                    new Facebook.Comment
                    {
                        Id = "1",
                        Message = "Message1"
                    },new Facebook.Comment
                    {
                        Id = "2",
                        Message = "Message2"
                    },new Facebook.Comment
                    {
                        Id = "3",
                        Message = "Message3"
                    }
                }
            };
            _mockCommentsResponseSecondPage = new Facebook.CommentsResponse
            {
                Data = new List<Facebook.Comment>
                {
                    new Facebook.Comment
                    {
                        Id = "4",
                        Message = "Message4"
                    }
                }
            };
            config = new FaceBookConfiguration { AppId = _appId, AppSecret = _appSecret };
        }

        [Test]
        public async Task GetPagesAsyncTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.Setup(x => x.GetAsync<Facebook.PagesResponse>(It.Is<string>(s => checkApiCall(s, "/v2.10/search", null))))
                .ReturnsAsync(_mockPagesResponse);
            httpMock.Setup(x => x.GetAsync<Facebook.AccessTokenResponse>(It.Is<string>(s => s.Contains("https://graph.facebook.com/oauth/access_token"))))
                .ReturnsAsync(_mockAccessTokenResponse);

            // Run
            var service = new FaceBookService(httpMock.Object, config);
            var pages = await service.GetPagesAsync("PageName1");

            // Assert
            Assert.IsNotNull(pages);
            Assert.AreEqual(3, pages.Count);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetPagesAsyncNoPagingTest()
        {
            // Model Setup
            _mockPagesResponse.Paging = new Facebook.Paging
            {
                Next = $"https://graph.facebook.com/v2.10/search?access_token={_mockAccessTokenResponse.AccessToken}"
            };

            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.SetupSequence(x => x.GetAsync<Facebook.PagesResponse>(It.Is<string>(s => checkApiCall(s, "/v2.10/search", null))))
                .ReturnsAsync(_mockPagesResponse);
            httpMock.Setup(x => x.GetAsync<Facebook.AccessTokenResponse>(It.Is<string>(s => s.Contains("https://graph.facebook.com/oauth/access_token"))))
                .ReturnsAsync(_mockAccessTokenResponse);

            // Run
            var service = new FaceBookService(httpMock.Object, config);
            var pages = await service.GetPagesAsync("PageName1");

            // Assert
            Assert.IsNotNull(pages);
            Assert.AreEqual(3, pages.Count);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetPagesAsyncNullTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.Setup(x => x.GetAsync<Facebook.PagesResponse>(It.Is<string>(s => checkApiCall(s, "/v2.10/search", null))))
                .ReturnsAsync(null as Facebook.PagesResponse);
            httpMock.Setup(x => x.GetAsync<Facebook.AccessTokenResponse>(It.Is<string>(s => s.Contains("https://graph.facebook.com/oauth/access_token"))))
                .ReturnsAsync(_mockAccessTokenResponse);

            // Run
            var service = new FaceBookService(httpMock.Object, config);
            var pages = await service.GetPagesAsync("PageName1");

            // Assert
            Assert.IsNotNull(pages);
            Assert.IsEmpty(pages);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetPostsAsyncTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.Setup(x => x.GetAsync<Facebook.FacebookPostsResponse>(It.Is<string>(s => checkApiCall(s, "/v2.10/Page1", null))))
                .ReturnsAsync(_mockFacebookPostsResponse);
            httpMock.Setup(x => x.GetAsync<Facebook.AccessTokenResponse>(It.Is<string>(s => s.Contains("https://graph.facebook.com/oauth/access_token"))))
                .ReturnsAsync(_mockAccessTokenResponse);

            // Run
            var service = new FaceBookService(httpMock.Object, config);
            var posts = await service.GetPostsAsync("Page1");

            // Assert
            Assert.IsNotNull(posts);
            Assert.AreEqual(3, posts.Count);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetPostsAsyncPagingTest()
        {
            // Model Setup
            _mockFacebookPostsResponse.Posts.Paging = new Facebook.Paging
            {
                Next = $"https://graph.facebook.com/v2.10/Page1?access_token={_mockAccessTokenResponse.AccessToken}"
            };

            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.SetupSequence(x => x.GetAsync<Facebook.FacebookPostsResponse>(It.Is<string>(s => checkApiCall(s, "/v2.10/Page1", null))))
                .ReturnsAsync(_mockFacebookPostsResponse)
                .ReturnsAsync(_mockFacebookPostsResponseSecondPage);
            httpMock.Setup(x => x.GetAsync<Facebook.AccessTokenResponse>(It.Is<string>(s => s.Contains("https://graph.facebook.com/oauth/access_token"))))
                .ReturnsAsync(_mockAccessTokenResponse);

            // Run
            var service = new FaceBookService(httpMock.Object, config);
            var posts = await service.GetPostsAsync("Page1");

            // Assert
            Assert.IsNotNull(posts);
            Assert.AreEqual(4, posts.Count);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetPostsAsyncNullTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.Setup(x => x.GetAsync<Facebook.FacebookPostsResponse>(It.Is<string>(s => checkApiCall(s, "/v2.10/Page1", null))))
                .ReturnsAsync(null as Facebook.FacebookPostsResponse);
            httpMock.Setup(x => x.GetAsync<Facebook.AccessTokenResponse>(It.Is<string>(s => s.Contains("https://graph.facebook.com/oauth/access_token"))))
                .ReturnsAsync(_mockAccessTokenResponse);

            // Run
            var service = new FaceBookService(httpMock.Object, config);
            var posts = await service.GetPostsAsync("Page1");

            // Assert
            Assert.IsNotNull(posts);
            Assert.IsEmpty(posts);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetCommentsAsyncTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.Setup(x => x.GetAsync<Facebook.CommentsResponse>(It.Is<string>(s => checkApiCall(s, "/v2.10/Post1/comments", null))))
                .ReturnsAsync(_mockCommentsResponse);
            httpMock.Setup(x => x.GetAsync<Facebook.AccessTokenResponse>(It.Is<string>(s => s.Contains("https://graph.facebook.com/oauth/access_token"))))
                .ReturnsAsync(_mockAccessTokenResponse);

            // Run
            var service = new FaceBookService(httpMock.Object, config);
            var comments = await service.GetCommentsAsync("Post1");

            // Assert
            Assert.IsNotNull(comments);
            Assert.AreEqual(3, comments.Count);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetCommentsAsyncPagingTest()
        {
            // Model Setup
            _mockCommentsResponse.Paging = new Facebook.Paging
            {
                Next = $"https://graph.facebook.com/v2.10/Post1/comments?access_token={_mockAccessTokenResponse.AccessToken}"
            };

            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.SetupSequence(x => x.GetAsync<Facebook.CommentsResponse>(It.Is<string>(s => checkApiCall(s, "/v2.10/Post1/comments", null))))
                .ReturnsAsync(_mockCommentsResponse)
                .ReturnsAsync(_mockCommentsResponseSecondPage);
            httpMock.Setup(x => x.GetAsync<Facebook.AccessTokenResponse>(It.Is<string>(s => s.Contains("https://graph.facebook.com/oauth/access_token"))))
                .ReturnsAsync(_mockAccessTokenResponse);

            // Run
            var service = new FaceBookService(httpMock.Object, config);
            var comments = await service.GetCommentsAsync("Post1");

            // Assert
            Assert.IsNotNull(comments);
            Assert.AreEqual(4, comments.Count);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GGetCommentsAsyncNullTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.Setup(x => x.GetAsync<Facebook.CommentsResponse>(It.Is<string>(s => checkApiCall(s, "/v2.10/Post1/comments", null))))
                .ReturnsAsync(null as Facebook.CommentsResponse);
            httpMock.Setup(x => x.GetAsync<Facebook.AccessTokenResponse>(It.Is<string>(s => s.Contains("https://graph.facebook.com/oauth/access_token"))))
                .ReturnsAsync(_mockAccessTokenResponse);

            // Run
            var service = new FaceBookService(httpMock.Object, config);
            var comments = await service.GetCommentsAsync("Post1");

            // Assert
            Assert.IsNotNull(comments);
            Assert.IsEmpty(comments);

            // Verify
            httpMock.VerifyAll();
        }


        private bool checkApiCall(string url, string part, Dictionary<string, string> additionalParamsToCheck)
        {
            Assert.IsTrue(Uri.TryCreate(url, UriKind.Absolute, out Uri uri));

            var path = String.Format("{0}{1}{2}{3}", uri.Scheme, Uri.SchemeDelimiter, uri.Authority, uri.AbsolutePath);
            var queryDictionary = System.Web.HttpUtility.ParseQueryString(uri.Query);

            foreach (var param in additionalParamsToCheck ?? new Dictionary<string, string>())
            {
                Assert.IsTrue(queryDictionary.AllKeys.Contains(param.Key), $"query parameter {param.Key} is present");
                Assert.AreEqual(param.Value, queryDictionary[param.Key], $"query parameter {param.Key} are equal");
            }

            Assert.AreEqual($"https://graph.facebook.com{part}", path, "Base path is correct");
            Assert.IsTrue(queryDictionary.AllKeys.Contains("access_token"), $"query parameter {"access_token"} is present");
            Assert.AreEqual(_mockAccessTokenResponse.AccessToken, queryDictionary["access_token"], $"query parameter {"access_token"} are equal");
            return true;
        }
    }
}