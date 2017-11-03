using Moq;
using NUnit.Framework;
using SocialMediaServices.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SocialMediaServices;
using SocialMediaServices.Services;
using YouTube = SocialMediaServices.Models.YouTube;

namespace SocialMediaServices.Tests.Unit
{
    [TestFixture]
    public class YouTubeServiceTests
    {
        private readonly string _apiKey = "TEST_API_KEY";
        private readonly string _nextPageToken = "1234567";
        private YouTubeConfiguration config;
        private YouTube.OnlyIdResponse _mockOnlyIdResponse;
        private YouTube.OnlyIdResponse _mockOnlyIdResponseSecondPage;
        private YouTube.PlaylistResponse _mockPlaylistResponse;
        private YouTube.PlaylistResponse _mockPlaylistResponseSecondPage;
        private YouTube.VideoResponse _mockVideoResponse;
        private YouTube.VideoResponse _mockVideoResponseSecondPage;
        private YouTube.CommentResponse _mockCommentResponse;
         

        [SetUp]
        public void SetUp()
        {
            _mockOnlyIdResponse = new YouTube.OnlyIdResponse
            {
                Items = new List<YouTube.Item> {
                    new YouTube.Item {
                        Id = "1"
                    },
                    new YouTube.Item {
                        Id = "2"
                    },
                    new YouTube.Item {
                        Id = "3"
                    }
            }
            };
            _mockOnlyIdResponseSecondPage = new YouTube.OnlyIdResponse
            {
                Items = new List<YouTube.Item> {
                    new YouTube.Item {
                        Id = "4"
                    }
            }
            };
            _mockPlaylistResponse = new YouTube.PlaylistResponse
            {
                Items = new List<YouTube.Playlist> {
                    new YouTube.Playlist {
                        Id = "1",
                        Snippet = new YouTube.PlaylistSnippet
                        {
                            Title = "Test1"
                        }
                    },
                    new YouTube.Playlist {
                        Id = "2",
                        Snippet = new YouTube.PlaylistSnippet
                        {
                            Title = "Test2"
                        }
                    },
                    new YouTube.Playlist {
                        Id = "3",
                        Snippet = new YouTube.PlaylistSnippet
                        {
                            Title = "Test3"
                        }
                    }
            }
            };
            _mockPlaylistResponseSecondPage = new YouTube.PlaylistResponse
            {
                Items = new List<YouTube.Playlist> {
                    new YouTube.Playlist {
                        Id = "4",
                        Snippet = new YouTube.PlaylistSnippet
                        {
                            Title = "Test4"
                        }
                    }
            }
            };
            _mockVideoResponse = new YouTube.VideoResponse
            {
                Items = new List<YouTube.Video>
            {
                new YouTube.Video
                {
                    Snippet = new YouTube.VideoSnippet
                    {
                        Title = "Test1"
                    }
                },
                new YouTube.Video
                {
                    Snippet = new YouTube.VideoSnippet
                    {
                        Title = "Test2"
                    }
                },
                new YouTube.Video
                {
                    Snippet = new YouTube.VideoSnippet
                    {
                        Title = "Test3"
                    }
                }
            }
            };
            _mockVideoResponseSecondPage = new YouTube.VideoResponse
            {
                Items = new List<YouTube.Video>
            {
                new YouTube.Video
                {
                    Snippet = new YouTube.VideoSnippet
                    {
                        Title = "Test4"
                    }
                }
            }
            };
            _mockCommentResponse = new YouTube.CommentResponse
            {
                Items = new List<YouTube.Comment>
                {
                    new YouTube.Comment
                    {
                        Id = "1",
                        Snippet = new YouTube.CommentSnippet
                        {
                            TextDisplay = "Test1"
                        }
                    },
                    new YouTube.Comment
                    {
                        Id = "2",
                        Snippet = new YouTube.CommentSnippet
                        {
                            TextDisplay = "Test2"
                        }
                    },
                    new YouTube.Comment
                    {
                        Id = "3",
                        Snippet = new YouTube.CommentSnippet
                        {
                            TextDisplay = "Test3"
                        }
                    }
                }
            };
            config = new YouTubeConfiguration { ApiKey = _apiKey };
        }

        [Test]
        public async Task GetChannelIdAsyncTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            var youTubeService = new YouTubeService(httpMock.Object, config);
            httpMock.Setup(x => x.GetAsync<YouTube.OnlyIdResponse>(It.Is<string>(s => checkApiCall(s, "channels", null))))
                .ReturnsAsync(_mockOnlyIdResponse);

            // Run
            var channelId = await youTubeService.GetChannelIdAsync("TestChannelName");

            // Assert
            Assert.AreEqual(_mockOnlyIdResponse.Items.First().Id, channelId);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetChannelIdAsyncNotFoundTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            var youTubeService = new YouTubeService(httpMock.Object, config);
            httpMock.Setup(x => x.GetAsync<YouTube.OnlyIdResponse>(It.Is<string>(s => checkApiCall(s, "channels", null))))
                .ReturnsAsync(null as YouTube.OnlyIdResponse);

            // Run
            var channelId = await youTubeService.GetChannelIdAsync("TestChannelName");

            // Assert
            Assert.IsNull(channelId);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetPlaylistIdAsyncTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            var youTubeService = new YouTubeService(httpMock.Object, config);
            httpMock.Setup(x => x.GetAsync<YouTube.PlaylistResponse>(It.Is<string>(s => checkApiCall(s, "playlists", null))))
                .ReturnsAsync(_mockPlaylistResponse);

            // Run
            var playlistId = await youTubeService.GetPlaylistIdAsync("1", "Test2");

            // Assert
            Assert.AreEqual(_mockPlaylistResponse.Items.Where(i => i.Snippet.Title.Equals("Test2")).First().Id, playlistId);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetPlaylistIdAsyncNotFoundTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            var youTubeService = new YouTubeService(httpMock.Object, config);
            httpMock.Setup(x => x.GetAsync<YouTube.PlaylistResponse>(It.Is<string>(s => checkApiCall(s, "playlists", null))))
                .ReturnsAsync(null as YouTube.PlaylistResponse);

            // Run
            var playlistId = await youTubeService.GetPlaylistIdAsync("1", "Test2");

            // Assert
            Assert.IsNull(playlistId);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetPlaylistIdAsyncPagedTest()
        {
            // Model Set Up
            _mockPlaylistResponse.NextPageToken = _nextPageToken;

            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            var youTubeService = new YouTubeService(httpMock.Object, config);
            httpMock.SetupSequence(x => x.GetAsync<YouTube.PlaylistResponse>(It.Is<string>(s => checkApiCall(s, "playlists", null))))
                .ReturnsAsync(_mockPlaylistResponse)
                .ReturnsAsync(_mockPlaylistResponseSecondPage);

            // Run
            var playlistId = await youTubeService.GetPlaylistIdAsync("1", "Test4");

            // Assert
            Assert.AreEqual(_mockPlaylistResponseSecondPage.Items.Where(i => i.Snippet.Title.Equals("Test4")).First().Id, playlistId);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetPlaylistVideosAsyncTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            var youTubeService = new YouTubeService(httpMock.Object, config);
            httpMock.Setup(x => x.GetAsync<YouTube.OnlyIdResponse>(It.Is<string>(s => checkApiCall(s, "playlistItems", null))))
                .ReturnsAsync(_mockOnlyIdResponse);
            httpMock.SetupSequence(x => x.GetAsync<YouTube.VideoResponse>(It.Is<string>(s => checkApiCall(s, "videos", null))))
                .ReturnsAsync(new YouTube.VideoResponse { Items = new List<YouTube.Video> { _mockVideoResponse.Items[0] } })
                .ReturnsAsync(new YouTube.VideoResponse { Items = new List<YouTube.Video> { _mockVideoResponse.Items[1] } })
                .ReturnsAsync(new YouTube.VideoResponse { Items = new List<YouTube.Video> { _mockVideoResponse.Items[2] } });

            // Run
            var videos = await youTubeService.GetPlaylistVideosAsync("1");

            // Assert
            Assert.AreEqual(3, videos.Count, "First page retrieved");
            Assert.AreEqual(_mockOnlyIdResponse.Items[0].Id, videos[0].Id);
            Assert.AreEqual(_mockOnlyIdResponse.Items[1].Id, videos[1].Id);
            Assert.AreEqual(_mockOnlyIdResponse.Items[2].Id, videos[2].Id);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetPlaylistVideosAsyncNotFoundTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            var youTubeService = new YouTubeService(httpMock.Object, config);
            httpMock.Setup(x => x.GetAsync<YouTube.OnlyIdResponse>(It.Is<string>(s => checkApiCall(s, "playlistItems", null))))
                .ReturnsAsync(null as YouTube.OnlyIdResponse);

            // Run
            var videos = await youTubeService.GetPlaylistVideosAsync("1");

            // Assert
            Assert.IsEmpty(videos);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetPlaylistVideosAsyncPagedTest()
        {
            // Model Set Up
            _mockOnlyIdResponse.NextPageToken = _nextPageToken;

            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            var youTubeService = new YouTubeService(httpMock.Object, config);
            httpMock.SetupSequence(x => x.GetAsync<YouTube.OnlyIdResponse>(It.Is<string>(s => checkApiCall(s, "playlistItems", null))))
                .ReturnsAsync(_mockOnlyIdResponse)
                .ReturnsAsync(_mockOnlyIdResponseSecondPage);
            httpMock.SetupSequence(x => x.GetAsync<YouTube.VideoResponse>(It.Is<string>(s => checkApiCall(s, "videos", null))))
                .ReturnsAsync(new YouTube.VideoResponse { Items = new List<YouTube.Video> { _mockVideoResponse.Items[0] } })
                .ReturnsAsync(new YouTube.VideoResponse { Items = new List<YouTube.Video> { _mockVideoResponse.Items[1] } })
                .ReturnsAsync(new YouTube.VideoResponse { Items = new List<YouTube.Video> { _mockVideoResponse.Items[2] } })
                .ReturnsAsync(new YouTube.VideoResponse { Items = new List<YouTube.Video> { _mockVideoResponseSecondPage.Items[0] } });

            // Run
            var videos = await youTubeService.GetPlaylistVideosAsync("1");

            // Assert
            Assert.AreEqual(4, videos.Count, "Second page retrieved");
            Assert.AreEqual(_mockOnlyIdResponse.Items[0].Id, videos[0].Id);
            Assert.AreEqual(_mockOnlyIdResponse.Items[1].Id, videos[1].Id);
            Assert.AreEqual(_mockOnlyIdResponse.Items[2].Id, videos[2].Id);
            Assert.AreEqual(_mockOnlyIdResponseSecondPage.Items[0].Id, videos[3].Id);


            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetVideoAsyncTest()
        {
            // Model Set Up
            _mockOnlyIdResponse.NextPageToken = _nextPageToken;

            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            var youTubeService = new YouTubeService(httpMock.Object, config);
            httpMock.Setup(x => x.GetAsync<YouTube.VideoResponse>(It.Is<string>(s => checkApiCall(s, "videos", null))))
                .ReturnsAsync(new YouTube.VideoResponse { Items = new List<YouTube.Video> { _mockVideoResponse.Items[0] } });

            // Run
            var video = await youTubeService.GetVideoAsync("1");

            // Assert
            Assert.AreEqual(_mockVideoResponse.Items[0].Snippet.Title, video.Title);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetVideoAsyncNotFoundTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            var youTubeService = new YouTubeService(httpMock.Object, config);
            httpMock.Setup(x => x.GetAsync<YouTube.VideoResponse>(It.Is<string>(s => checkApiCall(s, "videos", null))))
                .ReturnsAsync(null as YouTube.VideoResponse);

            // Run
            var video = await youTubeService.GetVideoAsync("1");

            // Assert
            Assert.IsNull(video);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        [TestCase("https://www.youtube.com/watch?v=s-mXHE-VTag", "s-mXHE-VTag")]
        [TestCase("http://literally.any.url.com/fake?v=s-mXHE-VTag", "s-mXHE-VTag")]
        [TestCase("http://literally.any.url.com/fake?t=test&v=s-mXHE-VTag&h=mustard", "s-mXHE-VTag")]
        [TestCase("http://www.literally.any.url.com/fake?t=test&v=s-mXHE-VTag&h=mustard", "s-mXHE-VTag")]
        public void ParseVideoIdTest(string url, string excpectedId)
        {
            var youTubeService = new YouTubeService(null, config);
            var actualId = youTubeService.ParseVideoId(new Uri(url));
            Assert.AreEqual(excpectedId, actualId);
        }

        [Test]
        [TestCase("https://www.youtube.com/watch")]
        [TestCase("http://literally.any.url.com/fake?t=test&h=mustard")]
        public void ParseVideoIdNotFoundTest(string url)
        {
            var youTubeService = new YouTubeService(null, config);
            var actualId = youTubeService.ParseVideoId(new Uri(url));
            Assert.IsNull(actualId);
        }

        [Test]
        public async Task GetCommentsAsyncTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            var youTubeService = new YouTubeService(httpMock.Object, config);
            httpMock.Setup(x => x.GetAsync<YouTube.OnlyIdResponse>(It.Is<string>(s => s.Contains("commentThreads") && checkApiCall(s, "commentThreads", null))))
                .ReturnsAsync(_mockOnlyIdResponse);
            httpMock.Setup(x => x.GetAsync<YouTube.OnlyIdResponse>(It.Is<string>(s => s.Contains("comments") && checkApiCall(s, "comments", null))))
                .ReturnsAsync(_mockOnlyIdResponse);
            httpMock.Setup(x => x.GetAsync<YouTube.CommentResponse>(It.Is<string>(s => checkApiCall(s, "comments", null))))
                .ReturnsAsync(_mockCommentResponse);

            // Run
            var comments = await youTubeService.GetCommentsAsync("1");

            // Assert
            Assert.AreEqual(3, comments.Count);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetCommentsAsyncNotFoundTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            var youTubeService = new YouTubeService(httpMock.Object, config);
            httpMock.Setup(x => x.GetAsync<YouTube.OnlyIdResponse>(It.Is<string>(s => checkApiCall(s, "commentThreads", null))))
                .ReturnsAsync(null as YouTube.OnlyIdResponse);

            // Run
            var comments = await youTubeService.GetCommentsAsync("1");

            // Assert
            Assert.IsEmpty(comments);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public void GetCommentsAsyncWithCancellationTokenTest()
        {
            // Mock
            var ctSource = new CancellationTokenSource();
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            var youTubeService = new YouTubeService(httpMock.Object, config);
            httpMock.Setup(x => x.GetAsync<YouTube.OnlyIdResponse>(It.Is<string>(s => s.Contains("commentThreads") && checkApiCall(s, "commentThreads", null))))
                .ReturnsAsync(_mockOnlyIdResponse);
            httpMock.Setup(x => x.GetAsync<YouTube.OnlyIdResponse>(It.Is<string>(s => s.Contains("comments") && checkApiCall(s, "comments", null))))
                .Callback(() => ctSource.Cancel())
                .ReturnsAsync(_mockOnlyIdResponse);

            // Assert
            Assert.ThrowsAsync<TaskCanceledException>(async () => await youTubeService.GetCommentsAsync("1", ctSource.Token));

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetCommentsAsyncWithProgressTest()
        {
            var progressCalled = 0;
            var progressIndicator = new Progress<int>(i => Interlocked.Increment(ref progressCalled));

            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            var youTubeService = new YouTubeService(httpMock.Object, config);
            httpMock.Setup(x => x.GetAsync<YouTube.OnlyIdResponse>(It.Is<string>(s => s.Contains("commentThreads") && checkApiCall(s, "commentThreads", null))))
                .ReturnsAsync(_mockOnlyIdResponse);
            httpMock.Setup(x => x.GetAsync<YouTube.OnlyIdResponse>(It.Is<string>(s => s.Contains("comments") && checkApiCall(s, "comments", null))))
                .ReturnsAsync(_mockOnlyIdResponse);
            httpMock.Setup(x => x.GetAsync<YouTube.CommentResponse>(It.Is<string>(s => checkApiCall(s, "comments", null))))
                .ReturnsAsync(_mockCommentResponse);

            // Run
            var comments = await youTubeService.GetCommentsAsync("1", progressIndicator);

            // Assert
            Assert.AreEqual(3, comments.Count);
            Assert.Greater(progressCalled, 0);

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

            Assert.AreEqual($"https://www.googleapis.com/youtube/v3/{part}", path, "Base path is correct");
            Assert.IsTrue(queryDictionary.AllKeys.Contains("key"), $"query parameter {"key"} is present");
            Assert.AreEqual(_apiKey, queryDictionary["key"], $"query parameter {"key"} are equal");
            return true;
        }
    }
}