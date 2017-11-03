using Moq;
using NUnit.Framework;
using SocialMediaServices.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialMediaServices;
using SocialMediaServices.Services;
using GooglePlus = SocialMediaServices.Models.GooglePlus;

namespace SocialMediaServices.Tests.Unit
{
    [TestFixture]
    public class GooglePlusServiceTests
    {
        private readonly string _apiKey = "TEST_API_KEY";
        private GooglePlusConfiguration config;
        private GooglePlus.ActivityResponse _mockActivityResponse;
        private GooglePlus.PeopleResponse _mockPeopleResponse;
        private GooglePlus.PeopleResponse _mockPeopleResponseSecondPage;
        private GooglePlus.CommentsResponse _mockCommentsResponse;
        private GooglePlus.CommentsResponse _mockCommentsResponseSecondPage;

         [SetUp]
        public void SetUp()
        {
            _mockActivityResponse = new GooglePlus.ActivityResponse
            {
                Items = new List<GooglePlus.Activity>
                {
                    new GooglePlus.Activity
                    {
                        Id = "1",
                        Object = new GooglePlus.ActivityObject
                        {
                            Attachments = new List<GooglePlus.Attachment>
                            {
                                new GooglePlus.Attachment
                                {
                                    ObjectType = "photo",
                                    Url = "Photo1"
                                }
                            }
                        }
                    },
                    new GooglePlus.Activity
                    {
                        Id = "2",
                        Object = new GooglePlus.ActivityObject
                        {
                            Attachments = new List<GooglePlus.Attachment>
                            {
                                new GooglePlus.Attachment
                                {
                                    ObjectType = "video",
                                    Url = "Video1"
                                }
                            }
                        }
                    }
                }
            };
            _mockPeopleResponse = new GooglePlus.PeopleResponse
            {
                Items = new List<GooglePlus.Person>
                {
                    new GooglePlus.Person
                    {
                        Id = "1",
                        DisplayName = "PersonName1"
                    },
                    new GooglePlus.Person
                    {
                        Id = "2",
                        DisplayName = "PersonName2"
                    },
                    new GooglePlus.Person
                    {
                        Id = "3",
                        DisplayName = "PersonName3"
                    }
                }
            };
            _mockPeopleResponseSecondPage = new GooglePlus.PeopleResponse
            {
                Items = new List<GooglePlus.Person>
                {
                    new GooglePlus.Person
                    {
                        Id = "4",
                        DisplayName = "PersonName4"
                    }
                }
            };
            _mockCommentsResponse = new GooglePlus.CommentsResponse
            {
                Items = new List<GooglePlus.Comment>
                {
                    new GooglePlus.Comment
                    {
                         Id = "1",
                         Object = new GooglePlus.CommentObject
                         {
                             Content = "Comment1"
                         }
                    },
                    new GooglePlus.Comment
                    {
                         Id = "2",
                         Object = new GooglePlus.CommentObject
                         {
                             Content = "Comment2"
                         }
                    },
                    new GooglePlus.Comment
                    {
                         Id = "3",
                         Object = new GooglePlus.CommentObject
                         {
                             Content = "Comment3"
                         }
                    }
                }
            };
            _mockCommentsResponseSecondPage = new GooglePlus.CommentsResponse
            {
                Items = new List<GooglePlus.Comment>
                {
                    new GooglePlus.Comment
                    {
                         Id = "4",
                         Object = new GooglePlus.CommentObject
                         {
                             Content = "Comment4"
                         }
                    }
                }
            };
            config = new GooglePlusConfiguration { ApiKey = _apiKey };
        }

        [Test]
        public async Task GetActivityWithVideoAsyncTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.Setup(x => x.GetAsync<GooglePlus.ActivityResponse>(It.Is<string>(s => checkApiCall(s, "people/Person1/activities/public", null))))
                .ReturnsAsync(_mockActivityResponse);

            // Run
            var service = new GooglePlusService(httpMock.Object, config);
            var activity = await service.GetActivityWithVideoAsync("Person1", "Video1");

            // Assert
            Assert.AreEqual(_mockActivityResponse.Items.First(i => i.Object.Attachments.Any(a => a.ObjectType.Equals("video") && a.Url.Contains("Video1"))).Id, activity.Id);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetActivityWithVideoAsyncNotFoundTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.Setup(x => x.GetAsync<GooglePlus.ActivityResponse>(It.Is<string>(s => checkApiCall(s, "people/Person1/activities/public", null))))
                .ReturnsAsync(null as GooglePlus.ActivityResponse);

            // Run
            var service = new GooglePlusService(httpMock.Object, config);
            var activity = await service.GetActivityWithVideoAsync("Person1", "Video1");

            // Assert
            Assert.IsNull(activity);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetPersonIdAsyncTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.Setup(x => x.GetAsync<GooglePlus.PeopleResponse>(It.Is<string>(s => checkApiCall(s, "people", null))))
                .ReturnsAsync(_mockPeopleResponse);

            // Run
            var service = new GooglePlusService(httpMock.Object, config);
            var personId = await service.GetPersonIdAsync("PersonName2");

            // Assert
            Assert.AreEqual(_mockPeopleResponse.Items.First(i => i.DisplayName.Equals("PersonName2")).Id, personId);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetPersonIdAsyncPagingTest()
        {
            // Set Up Model
            _mockPeopleResponse.NextPageToken = "SOME PAGE TOKEN";

            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.SetupSequence(x => x.GetAsync<GooglePlus.PeopleResponse>(It.Is<string>(s => checkApiCall(s, "people", null))))
                .ReturnsAsync(_mockPeopleResponse)
                .ReturnsAsync(_mockPeopleResponseSecondPage);

            // Run
            var service = new GooglePlusService(httpMock.Object, config);
            var personId = await service.GetPersonIdAsync("PersonName4");

            // Assert
            Assert.AreEqual(_mockPeopleResponseSecondPage.Items.First(i => i.DisplayName.Equals("PersonName4")).Id, personId);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetPersonIdAsyncNotFoundTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.Setup(x => x.GetAsync<GooglePlus.PeopleResponse>(It.Is<string>(s => checkApiCall(s, "people", null))))
                .ReturnsAsync(_mockPeopleResponse);

            // Run
            var service = new GooglePlusService(httpMock.Object, config);
            var personId = await service.GetPersonIdAsync("PersonName999");

            // Assert
            Assert.IsNull(personId);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetPersonIdAsyncNullTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.Setup(x => x.GetAsync<GooglePlus.PeopleResponse>(It.Is<string>(s => checkApiCall(s, "people", null))))
                .ReturnsAsync(null as GooglePlus.PeopleResponse);

            // Run
            var service = new GooglePlusService(httpMock.Object, config);
            var personId = await service.GetPersonIdAsync("PersonName1");

            // Assert
            Assert.IsNull(personId);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetCommentsAsyncTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.Setup(x => x.GetAsync<GooglePlus.CommentsResponse>(It.Is<string>(s => checkApiCall(s, "activities/Activity1/comments", null))))
                .ReturnsAsync(_mockCommentsResponse);

            // Run
            var service = new GooglePlusService(httpMock.Object, config);
            var comments = await service.GetCommentsAsync("Activity1");

            // Assert
            Assert.IsNotNull(comments);
            Assert.AreEqual(3, comments.Count);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetCommentsAsyncPAgingTest()
        {
            // Set Up Model
            _mockCommentsResponse.NextPageToken = "SOME PAGE TOKEN";

            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.SetupSequence(x => x.GetAsync<GooglePlus.CommentsResponse>(It.Is<string>(s => checkApiCall(s, "activities/Activity1/comments", null))))
                .ReturnsAsync(_mockCommentsResponse)
                .ReturnsAsync(_mockCommentsResponseSecondPage);

            // Run
            var service = new GooglePlusService(httpMock.Object, config);
            var comments = await service.GetCommentsAsync("Activity1");

            // Assert
            Assert.IsNotNull(comments);
            Assert.AreEqual(4, comments.Count);

            // Verify
            httpMock.VerifyAll();
        }

        [Test]
        public async Task GetCommentsAsyncNullTest()
        {
            // Mock
            var httpMock = new Mock<ISafeHttpClient>(MockBehavior.Strict);
            httpMock.Setup(x => x.GetAsync<GooglePlus.CommentsResponse>(It.Is<string>(s => checkApiCall(s, "activities/Activity1/comments", null))))
                .ReturnsAsync(null as GooglePlus.CommentsResponse);

            // Run
            var service = new GooglePlusService(httpMock.Object, config);
            var comments = await service.GetCommentsAsync("Activity1");

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

            Assert.AreEqual($"https://www.googleapis.com/plus/v1/{part}", path, "Base path is correct");
            Assert.IsTrue(queryDictionary.AllKeys.Contains("key"), $"query parameter {"key"} is present");
            Assert.AreEqual(_apiKey, queryDictionary["key"], $"query parameter {"key"} are equal");
            return true;
        }
    }
}