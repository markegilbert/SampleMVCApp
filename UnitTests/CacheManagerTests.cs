using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit;
using NSubstitute;
using NUnit.Framework;
using Microsoft.Extensions.Caching.Memory;
using SampleApp.Cache;

namespace UnitTests
{
    [TestFixture]
    public class CacheManagerTests
    {
        private IMemoryCache? _CacheMock;
        private CacheManager? _CacheManager;
        private ArgumentNullException? _ArgumentNullException;
        private AggregateException? _AggregateException;
        private String? _ReturnValue, _ActualResult;
        private int _NumberOfTimesDelegateInvoked;


        [SetUp]
        public void SetUp()
        {
            this._ArgumentNullException = null;
            this._AggregateException = null;
            this._ReturnValue = Guid.NewGuid().ToString();
            this._NumberOfTimesDelegateInvoked = 0;
            this._ActualResult = "";
        }
        private async Task<String> GetAStringAsync()
        {
            this._NumberOfTimesDelegateInvoked++;
            return this._ReturnValue;
        }


        [Test]
        public void InstantiateClass_MemoryCacheIsNull_ExceptionThrown()
        {
            this._ArgumentNullException = Assert.Throws<ArgumentNullException>(() => new CacheManager(null));

            Assert.That(this._ArgumentNullException.Message.Contains("'Cache'"), "Argument exception message didn't mention the correct parameter");
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void GetFromCache_CacheKeyWasNotValid_ExceptionThrown(String TestValue)
        {
            this._CacheMock = Substitute.For<IMemoryCache>();
            this._CacheManager = new CacheManager(this._CacheMock);

            this._AggregateException = Assert.Throws<AggregateException>(() => this._CacheManager.GetFromCache<String>(TestValue, async () => await this.GetAStringAsync()).Wait());

            Assert.That(this._AggregateException.Message.Contains("'CacheKey'"), "Argument exception message didn't mention the correct parameter");
        }
        [Test]
        public void GetFromCache_CacheKeyValid_ItemNotInCache_IfNotFoundDelegateInvoked()
        {
            // This method doesn't use MemoryCache.Testing.NSubstitute at all


            this._CacheMock = Substitute.For<IMemoryCache>();
            this._CacheManager = new CacheManager(this._CacheMock);


            this._CacheManager.GetFromCache<String>("Cache Key 1", async () => await this.GetAStringAsync()).Wait();


            // CacheManager.GetFromCache() calls IMemoryCache.GetOrCreateAsync, so I'd like to do this:
            //      this._CacheMock.Received().GetOrCreateAsync<String>("Cache Key 1", Arg.Any<Func<ICacheEntry, Task<String>>>());
            // However, GetOrCreateAsync() is an extension method, so I can't directly mock it or check that it was called.
            // If the key passed in is not already in session, it will call CreateEntry(), which is not an extension method, so
            // I can verify that method was invoked using Received().
            this._CacheMock.Received(1).CreateEntry("Cache Key 1");

            // Also verify that the delegate itself was called
            Assert.That(this._NumberOfTimesDelegateInvoked, Is.EqualTo(1), "Delegate was not invoked the correct number of times");
        }
        [Test]
        public async Task GetFromCache_CacheKeyValid_ItemIsAlreadyInCache_IfNotFoundDelegateNotInvoked()
        {
            // This method uses MemoryCache.Testing.NSubstitute to mock the calls to IMemoryCache.Set and
            // IMemoryCache.GetOrCreateAsync (used by CacheManager.GetFromCache under the covers).

            this._CacheMock = MemoryCache.Testing.NSubstitute.Create.MockedMemoryCache();
            this._CacheMock.Set("Cache Key 1", this._ReturnValue);
            this._CacheManager = new CacheManager(this._CacheMock);


            this._ActualResult = await this._CacheManager.GetFromCache<String?>("Cache Key 1", async () => await this.GetAStringAsync());


            // Make sure the correct value came back out of the cache, but that the delegate function itself wasn't invoked to do it
            Assert.That(this._ActualResult, Is.EqualTo(this._ReturnValue), "The method didn't return the correct value");
            Assert.That(this._NumberOfTimesDelegateInvoked, Is.EqualTo(0), "Delegate function should not have been invoked");
        }




        [Test]
        public void GenerateUniqueName_ValidTrackName_ValidArtistName_ValidKeyReturned()
        {
            this._CacheMock = Substitute.For<IMemoryCache>();
            this._CacheManager = new CacheManager(this._CacheMock);

            Assert.That(this._CacheManager.GenerateUniqueName("TrackA", "ArtistB"), Is.EqualTo("TrackA_ArtistB"));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void GenerateUniqueName_ValidTrackName_ArtistNameInvalid_ValidKeyReturnedWithJustTrackName(String TestValue)
        {
            this._CacheMock = Substitute.For<IMemoryCache>();
            this._CacheManager = new CacheManager(this._CacheMock);

            Assert.That(this._CacheManager.GenerateUniqueName("TrackA", TestValue), Is.EqualTo("TrackA"));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void GenerateUniqueName_ValidTrackInvalid_ArtistNameValid_ValidKeyReturnedWithJustArtistName(String TestValue)
        {
            this._CacheMock = Substitute.For<IMemoryCache>();
            this._CacheManager = new CacheManager(this._CacheMock);

            Assert.That(this._CacheManager.GenerateUniqueName(TestValue, "ArtistB"), Is.EqualTo("ArtistB"));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void GenerateUniqueName_BothParametersAreInvalid_ExceptionThrown(String TestValue)
        {
            this._CacheMock = Substitute.For<IMemoryCache>();
            this._CacheManager = new CacheManager(this._CacheMock);

            Assert.Throws<ArgumentException>(() => this._CacheManager.GenerateUniqueName(TestValue, TestValue));
        }
    }
}
