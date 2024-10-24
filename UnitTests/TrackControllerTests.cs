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
using Microsoft.AspNetCore.Mvc;
using SampleApp.Models;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
        private ViewResult? _ReturnedView;
        private ViewDataDictionary? _RawView;
        private TrackSearchModel? _SearchModel;
        private List<Track>? _SampleSearchResults;


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

            this._ReturnedView = null;
            this._RawView = null;
            this._SearchModel = null;

            this._SampleSearchResults = new List<Track>();
            this._SampleSearchResults.Add(new Track() { TrackId = 1, Name = Guid.NewGuid().ToString() });
            this._SampleSearchResults.Add(new Track() { TrackId = 2, Name = Guid.NewGuid().ToString() });
            this._SampleSearchResults.Add(new Track() { TrackId = 3, Name = Guid.NewGuid().ToString() });
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
        public async Task Search_GET_NeitherTrackNorArtistSpecified_ViewReturnedWithEmptyModel()
        {
            this._Controller = new TrackController(this._Logger, this._DbContext, this._ImageService, this._CacheManager);

            this._ReturnedView = await this._Controller.Search(null, null, 1) as ViewResult;

            Assert.That(this._ReturnedView?.ViewName, Is.EqualTo("Search"), "The view returned was not correct");
            this._RawView = this._ReturnedView?.ViewData;
            Assert.That(this._RawView, Is.Not.Null, "ViewData should not have been null");
            Assert.That(this._RawView?.Model, Is.Not.Null, "Model should not have been null");
            Assert.That(this._RawView?.Model, Is.InstanceOf<TrackSearchModel>(), "Model returned was not of the correct type");
            this._SearchModel = (TrackSearchModel)this._RawView?.Model;
            Assert.That(this._SearchModel.TrackName, Is.Empty, "Model.TrackName should have been empty");
            Assert.That(this._SearchModel.ArtistName, Is.Empty, "Model.ArtistName should have been empty");
            Assert.That(this._SearchModel.Page, Is.EqualTo(0), "Model.Page was not correct");
        }
        [Test]
        public async Task Search_GET_TrackNameSpecified_ViewReturnedWithPrePopulatedModel()
        {
            this._Controller = new TrackController(this._Logger, this._DbContext, this._ImageService, this._CacheManager);

            this._ReturnedView = await this._Controller.Search("Test Track", null, 0) as ViewResult;

            Assert.That(this._ReturnedView?.ViewName, Is.EqualTo("Search"), "The view returned was not correct");
            this._RawView = this._ReturnedView?.ViewData;
            Assert.That(this._RawView, Is.Not.Null, "ViewData should not have been null");
            Assert.That(this._RawView?.Model, Is.Not.Null, "Model should not have been null");
            Assert.That(this._RawView?.Model, Is.InstanceOf<TrackSearchModel>(), "Model returned was not of the correct type");
            this._SearchModel = (TrackSearchModel)this._RawView?.Model;
            Assert.That(this._SearchModel.TrackName, Is.EqualTo("Test Track"), "Model.TrackName didn't match");
            Assert.That(this._SearchModel.ArtistName, Is.Empty, "Model.ArtistName should have been empty");
            Assert.That(this._SearchModel.Page, Is.EqualTo(0), "Model.Page was not correct");
        }
        [Test]
        public async Task Search_GET_ArtistNameSpecified_ViewReturnedWithPrePopulatedModel()
        {
            this._Controller = new TrackController(this._Logger, this._DbContext, this._ImageService, this._CacheManager);

            this._ReturnedView = await this._Controller.Search(null, "Test Artist", 0) as ViewResult;

            Assert.That(this._ReturnedView?.ViewName, Is.EqualTo("Search"), "The view returned was not correct");
            this._RawView = this._ReturnedView?.ViewData;
            Assert.That(this._RawView, Is.Not.Null, "ViewData should not have been null");
            Assert.That(this._RawView?.Model, Is.Not.Null, "Model should not have been null");
            Assert.That(this._RawView?.Model, Is.InstanceOf<TrackSearchModel>(), "Model returned was not of the correct type");
            this._SearchModel = (TrackSearchModel)this._RawView?.Model;
            Assert.That(this._SearchModel.TrackName, Is.Empty, "Model.TrackName should have been empty");
            Assert.That(this._SearchModel.ArtistName, Is.EqualTo("Test Artist"), "Model.ArtistName didn't match");
            Assert.That(this._SearchModel.Page, Is.EqualTo(0), "Model.Page was not correct");
        }

        [Test]
        public async Task Search_POST_EmptyModelPassedIn_ModelStateIsNotValid_ViewReturnedWithModelThatWasPassedIn()
        {
            this._Controller = new TrackController(this._Logger, this._DbContext, this._ImageService, this._CacheManager);
            this._Controller.ModelState.AddModelError("ModelKey", "Something wrong with the world");

            this._ReturnedView = await this._Controller.Search(new TrackSearchModel()) as ViewResult;

            Assert.That(this._ReturnedView?.ViewName, Is.EqualTo("Search"), "The view returned was not correct");
            this._RawView = this._ReturnedView?.ViewData;
            Assert.That(this._RawView, Is.Not.Null, "ViewData should not have been null");
            Assert.That(this._RawView?.Model, Is.Not.Null, "Model should not have been null");
            Assert.That(this._RawView?.Model, Is.InstanceOf<TrackSearchModel>(), "Model returned was not of the correct type");
            this._SearchModel = (TrackSearchModel)this._RawView?.Model;
            Assert.That(this._SearchModel.TrackName, Is.Empty, "Model.TrackName should have been empty");
            Assert.That(this._SearchModel.ArtistName, Is.Empty, "Model.ArtistName should have been empty");
            Assert.That(this._SearchModel.Page, Is.EqualTo(0), "Model.Page was not correct");
        }
        [Test]
        public async Task Search_POST_ValidModelPassedIn_OnlyTrackIsSpecified_FindTrackByNameAndOrArtistIsInvokedWithCorrectParameters()
        {
            this._Controller = new TrackController(this._Logger, this._DbContext, this._ImageService, this._CacheManager);
            this._DbContext.FindTrackByNameAndOrArtist(default, default).ReturnsForAnyArgs(x => this._SampleSearchResults);


            this._ReturnedView = await this._Controller.Search(new TrackSearchModel() { TrackName = "Test Track" }) as ViewResult;


            await this._DbContext.Received(1).FindTrackByNameAndOrArtist("Test Track", "");
            Assert.That(this._ReturnedView?.ViewName, Is.EqualTo("Search"), "The view returned was not correct");
            this._RawView = this._ReturnedView?.ViewData;
            Assert.That(this._RawView, Is.Not.Null, "ViewData should not have been null");
            Assert.That(this._RawView?.Model, Is.Not.Null, "Model should not have been null");
            Assert.That(this._RawView?.Model, Is.InstanceOf<TrackSearchModel>(), "Model returned was not of the correct type");
            this._SearchModel = (TrackSearchModel)this._RawView?.Model;
            Assert.That(this._SearchModel.Results.Count, Is.EqualTo(this._SampleSearchResults.Count), "Returned results did not match the expected list");
            Assert.That(this._SearchModel.Results[0].TrackName, Is.EqualTo(this._SampleSearchResults[0].Name), "Returned results 0's name did not match");
        }
        [Test]
        public async Task Search_POST_ValidModelPassedIn_OnlyArtistIsSpecified_FindTrackByNameAndOrArtistIsInvokedWithCorrectParameters()
        {
            this._Controller = new TrackController(this._Logger, this._DbContext, this._ImageService, this._CacheManager);


            this._ReturnedView = await this._Controller.Search(new TrackSearchModel() { ArtistName = "Test Artist" }) as ViewResult;


            await this._DbContext.Received(1).FindTrackByNameAndOrArtist("", "Test Artist");
            Assert.That(this._ReturnedView?.ViewName, Is.EqualTo("Search"), "The view returned was not correct");
            this._RawView = this._ReturnedView?.ViewData;
            Assert.That(this._RawView, Is.Not.Null, "ViewData should not have been null");
            Assert.That(this._RawView?.Model, Is.Not.Null, "Model should not have been null");
            Assert.That(this._RawView?.Model, Is.InstanceOf<TrackSearchModel>(), "Model returned was not of the correct type");
            this._SearchModel = (TrackSearchModel)this._RawView?.Model;
        }

        [Test]
        public async Task Search_POST_ValidModelPassedIn_WithBothTrackAndArtistSpecified_FindTrackByNameAndOrArtistIsInvokedWithCorrectParameters()
        {
            this._Controller = new TrackController(this._Logger, this._DbContext, this._ImageService, this._CacheManager);


            this._ReturnedView = await this._Controller.Search(new TrackSearchModel() { TrackName = "Test Track", ArtistName = "Test Artist" }) as ViewResult;


            await this._DbContext.Received(1).FindTrackByNameAndOrArtist("Test Track", "Test Artist");
            Assert.That(this._ReturnedView?.ViewName, Is.EqualTo("Search"), "The view returned was not correct");
            this._RawView = this._ReturnedView?.ViewData;
            Assert.That(this._RawView, Is.Not.Null, "ViewData should not have been null");
            Assert.That(this._RawView?.Model, Is.Not.Null, "Model should not have been null");
            Assert.That(this._RawView?.Model, Is.InstanceOf<TrackSearchModel>(), "Model returned was not of the correct type");
            this._SearchModel = (TrackSearchModel)this._RawView?.Model;
        }

    }
}
