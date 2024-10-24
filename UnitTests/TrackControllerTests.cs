using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using SampleApp.Controllers;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using SampleApp.Database;
using SampleApp.Services;
using Microsoft.Extensions.Caching.Memory;
using SampleApp.Cache;

namespace UnitTests
{
    [TestFixture]
    public class TrackControllerTests
    {
        private TrackController? _Controller;

        private ILogger<TrackController>? _Logger;

        private IChinookDbContext? _DbContext;

        private HttpMessageHandler? _HttpMessageHandlerMock;
        private HttpClient? _HttpClientMock;
        private GeniusService? _ImageService;

        private IMemoryCache? _CacheMock;
        private CacheManager? _CacheManager;

        private ArgumentNullException? _ArgumentNullException;

        private ILogger<GeniusService> _LoggerMock;


        [SetUp]
        public void SetUp()
        {
            this._Logger = Substitute.For<ILogger<TrackController>>();

            this._DbContext = Substitute.For<IChinookDbContext>();

            // This is the simpler of the two methods used in this test suite for mocking HttpClient.  Please see GeniusServiceTests
            // and the MockHttpMessageHandler class for the more sophisticated version that allows testing of the actual requests
            // being made.
            this._HttpMessageHandlerMock = Substitute.For<HttpMessageHandler>();
            this._HttpClientMock = new HttpClient(this._HttpMessageHandlerMock);

            this._LoggerMock = Substitute.For<ILogger<GeniusService>>();
            this._ImageService = new GeniusService(this._HttpClientMock, this._LoggerMock);

            this._CacheMock = Substitute.For<IMemoryCache>();
            this._CacheManager = new CacheManager(this._CacheMock);

            this._ArgumentNullException = null;
        }


        [Test]
        public void InstantiateClass_LoggerIsNull_ExceptionThrown()
        {
            this._ArgumentNullException = Assert.Throws<ArgumentNullException>(() => new TrackController(null, this._DbContext, this._ImageService, this._CacheManager));
            Assert.That(this._ArgumentNullException is not null && this._ArgumentNullException.Message.Contains("'Logger'"), "Exception doesn't mention the correct parameter");
        }
        [Test]
        public void InstantiateClass_ContextIsNull_ExceptionThrown()
        {
            this._ArgumentNullException = Assert.Throws<ArgumentNullException>(() => new TrackController(this._Logger, null, this._ImageService, this._CacheManager));
            Assert.That(this._ArgumentNullException is not null && this._ArgumentNullException.Message.Contains("'Context'"), "Exception doesn't mention the correct parameter");
        }
        [Test]
        public void InstantiateClass_ImageServiceIsNull_ExceptionThrown()
        {
            this._ArgumentNullException = Assert.Throws<ArgumentNullException>(() => new TrackController(this._Logger, this._DbContext, null, this._CacheManager));
            Assert.That(this._ArgumentNullException is not null && this._ArgumentNullException.Message.Contains("'ImageService'"), "Exception doesn't mention the correct parameter");
        }
        [Test]
        public void InstantiateClass_CacheManagerIsNull_ExceptionThrown()
        {
            this._ArgumentNullException = Assert.Throws<ArgumentNullException>(() => new TrackController(this._Logger, this._DbContext, this._ImageService, null));
            Assert.That(this._ArgumentNullException is not null && this._ArgumentNullException.Message.Contains("'CacheManager'"), "Exception doesn't mention the correct parameter");
        }


        [Test]
        public async Task Search_TrackAndArtistAreSpecifiedInModel_ImageServiceSeachIsInvokedWithCorrectParameters()
        {
            this._Controller = new TrackController(this._Logger, this._DbContext, this._ImageService, this._CacheManager);

            await this._Controller.Search(new SampleApp.Models.TrackSearchModel() { TrackName = "Test Track", ArtistName = "Test Artist" });

            await this._DbContext.Received(1).FindTrackByNameAndOrArtist("Test Track", "Test Artist");
        }
        [Test]
        public async Task Search_OnlyTrackIsSpecifiedInModel_ImageServiceSeachIsInvokedWithCorrectParameters()
        {
            this._Controller = new TrackController(this._Logger, this._DbContext, this._ImageService, this._CacheManager);

            await this._Controller.Search(new SampleApp.Models.TrackSearchModel() { TrackName = "Test Track" });

            await this._DbContext.Received(1).FindTrackByNameAndOrArtist("Test Track", "");
        }
        [Test]
        public async Task Search_OnlyArtistIsSpecifiedInModel_ImageServiceSeachIsInvokedWithCorrectParameters()
        {
            this._Controller = new TrackController(this._Logger, this._DbContext, this._ImageService, this._CacheManager);

            await this._Controller.Search(new SampleApp.Models.TrackSearchModel() { ArtistName = "Test Artist" });

            await this._DbContext.Received(1).FindTrackByNameAndOrArtist("", "Test Artist");
        }
    }
}
