using NUnit.Framework;
using SampleApp.Services;
using NSubstitute;
using Microsoft.Extensions.Logging;

namespace UnitTests
{
    [TestFixture]
    public class GeniusServiceTests
    {
        private GeniusService? _Service;
        private MockHttpMessageHandler _HttpMessageHandlerMock;
        private HttpClient? _HttpClientMock;
        private String? _ExpectedQueryString;
        private ILogger<GeniusService> _LoggerMock;
        private ArgumentNullException? _ArgumentNullException;

        [SetUp]
        public void SetUp()
        {
            // I wanted to do this:
            //      GeniusSearchResponse Y = new GeniusSearchResponse() { Meta = new GeniusResponseMeta() { Status = 200 }, 
            //                                                        ResponseData = new GeniusSearchResponseData() { Hits = new GeniusSearchResponseHit[0] } };
            //      var X = new MockHttpMessageHandler(JsonConvert.SerializeObject(Y), System.Net.HttpStatusCode.OK);
            // But the serializer isn't respecting the JsonPropertyName attributes, so I need to handcraft the test JSON.  See this SO post for more:
            //      https://stackoverflow.com/questions/70601659/is-there-a-way-to-serialize-json-property-name-in-newtonsoft
            this._HttpMessageHandlerMock = new MockHttpMessageHandler("{\"meta\":{\"status\":200},\"response\":{\"hits\":[]}}", System.Net.HttpStatusCode.OK);

            this._HttpClientMock = new HttpClient(this._HttpMessageHandlerMock);
            this._HttpClientMock.BaseAddress = new Uri("http://localhost");

            this._LoggerMock = Substitute.For<ILogger<GeniusService>>();

            this._Service = new GeniusService(this._HttpClientMock, this._LoggerMock);

            this._ArgumentNullException = null;
        }


        [Test]
        public void InstantiateClass_HttpClientIsNull_ExceptionThrown()
        {
            this._ArgumentNullException = Assert.Throws<ArgumentNullException>(() => new GeniusService(null, this._LoggerMock));
            Assert.That(this._ArgumentNullException.Message.Contains("'Client'"), "The exception didn't reference the correct parameter");
        }
        [Test]
        public void InstantiateClass_LoggerIsNull_ExceptionThrown()
        {
            this._ArgumentNullException = Assert.Throws<ArgumentNullException>(() => new GeniusService(this._HttpClientMock, null));
            Assert.That(this._ArgumentNullException.Message.Contains("'Logger'"), "The exception didn't reference the correct parameter");
        }


        [Test]
        public void GenerateSearchString_TrackNameAndArtistNameSpecified_ValidSearchStringReturned()
        {
            Assert.That(this._Service.GenerateSearchString("TrackA", "ArtistB"), Is.EqualTo("TrackA by ArtistB"));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void GenerateSearchString_TrackNameIsInvalid_SearchStringOnlyIncludesArtistName(String TestValue)
        {
            Assert.That(this._Service.GenerateSearchString(TestValue, "ArtistB"), Is.EqualTo("ArtistB"));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void GenerateSearchString_ArtistNameIsInvalid_SearchStringOnlyIncludesTrackName(String TestValue)
        {
            Assert.That(this._Service.GenerateSearchString("TrackA", TestValue), Is.EqualTo("TrackA"));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void GenerateSearchString_NeitherParameterIsValid_ReturnsEmptyString(String TestValue)
        {
            Assert.That(this._Service.GenerateSearchString(TestValue, TestValue), Is.Empty);
        }
        [Test]
        public void GenerateSearchString_TrackNameHasExtraSpaces_ReturnedSearchStringTrimsThoseOut()
        {
            Assert.That(this._Service.GenerateSearchString(" TrackA ", "ArtistB"), Is.EqualTo("TrackA by ArtistB"));
        }
        [Test]
        public void GenerateSearchString_ArtistNameHasExtraSpaces_ReturnedSearchStringTrimsThoseOut()
        {
            Assert.That(this._Service.GenerateSearchString("TrackA", " ArtistB "), Is.EqualTo("TrackA by ArtistB"));
        }


        [Test]
        public async Task SearchByTrackAndArtistAsync_BothParametersAreValid_ValidSearchRequestGenerated()
        {
            this._Service = new GeniusService(this._HttpClientMock, this._LoggerMock);
            this._ExpectedQueryString = "?q=TrackA%20by%20ArtistB";


            var Result = await this._Service.SearchByTrackAndArtistAsync("TrackA", "ArtistB");


            Assert.That(Result, Is.Not.Null, "Response should not have been null");
            Assert.That(this._HttpMessageHandlerMock.RequestUri.Query, Is.EqualTo(this._ExpectedQueryString), "Querystring was not correct");
            Assert.That(this._HttpMessageHandlerMock.RequestUri.LocalPath, Is.EqualTo("/search"), "LocalPath was not correct");
            Assert.That(this._HttpMessageHandlerMock.NumberOfCalls, Is.EqualTo(1), "The method didn't make the correct number of requests");
        }
        [Test]
        public async Task SearchByTrackAndArtistAsync_OnlyTrackSupplied_ValidSearchRequestGenerated()
        {
            this._Service = new GeniusService(this._HttpClientMock, this._LoggerMock);
            this._ExpectedQueryString = "?q=Track%20A";


            var Result = await this._Service.SearchByTrackAndArtistAsync("Track A", "");


            Assert.That(Result, Is.Not.Null, "Response should not have been null");
            Assert.That(this._HttpMessageHandlerMock.RequestUri.Query, Is.EqualTo(this._ExpectedQueryString), "Querystring was not correct");
            Assert.That(this._HttpMessageHandlerMock.RequestUri.LocalPath, Is.EqualTo("/search"), "LocalPath was not correct");
            Assert.That(this._HttpMessageHandlerMock.NumberOfCalls, Is.EqualTo(1), "The method didn't make the correct number of requests");
        }
        [Test]
        public async Task SearchByTrackAndArtistAsync_OnlyArtistSupplied_ValidSearchRequestGenerated()
        {
            this._Service = new GeniusService(this._HttpClientMock, this._LoggerMock);
            this._ExpectedQueryString = "?q=Artist%20B";


            var Result = await this._Service.SearchByTrackAndArtistAsync("", "Artist B");


            Assert.That(Result, Is.Not.Null, "Response should not have been null");
            Assert.That(this._HttpMessageHandlerMock.RequestUri.Query, Is.EqualTo(this._ExpectedQueryString), "Querystring was not correct");
            Assert.That(this._HttpMessageHandlerMock.RequestUri.LocalPath, Is.EqualTo("/search"), "LocalPath was not correct");
            Assert.That(this._HttpMessageHandlerMock.NumberOfCalls, Is.EqualTo(1), "The method didn't make the correct number of requests");
        }
        [Test]
        public async Task SearchByTrackAndArtistAsync_NeitherParameterSupplied_RequestIsNotMade()
        {
            this._Service = new GeniusService(this._HttpClientMock, this._LoggerMock);


            var Result = await this._Service.SearchByTrackAndArtistAsync("", "");


            Assert.That(Result, Is.Not.Null, "Response should not have been null");
            Assert.That(this._HttpMessageHandlerMock.NumberOfCalls, Is.EqualTo(0), "The method didn't make the correct number of requests");
        }
    }
}
