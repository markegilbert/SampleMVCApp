using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SampleApp.Services;
using NSubstitute;

namespace UnitTests
{
    [TestFixture]
    public class GeniusServiceTests
    {
        private GeniusService? _Service;
        private HttpMessageHandler? _HttpMessageHandlerMock;
        private HttpClient? _HttpClientMock;

        [SetUp]
        public void SetUp()
        {
            this._HttpMessageHandlerMock = Substitute.For<HttpMessageHandler>();
            this._HttpClientMock = new HttpClient(this._HttpMessageHandlerMock);

            this._Service = new GeniusService(this._HttpClientMock);
        }


        [Test]
        public void GenerateUniqueName_ValidTrackName_ValidArtistName_ValidKeyReturned()
        {
            Assert.That(this._Service.GenerateUniqueName("TrackA", "ArtistB"), Is.EqualTo("TrackA_ArtistB"));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void GenerateUniqueName_ValidTrackName_ArtistNameInvalid_ValidKeyReturnedWithJustTrackName(String TestValue)
        {
            Assert.That(this._Service.GenerateUniqueName("TrackA", TestValue), Is.EqualTo("TrackA"));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void GenerateUniqueName_ValidTrackInvalid_ArtistNameValid_ValidKeyReturnedWithJustArtistName(String TestValue)
        {
            Assert.That(this._Service.GenerateUniqueName(TestValue, "ArtistB"), Is.EqualTo("ArtistB"));
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void GenerateUniqueName_BothParametersAreInvalid_ExceptionThrown(String TestValue)
        {
            Assert.Throws<ArgumentException>(() => this._Service.GenerateUniqueName(TestValue, TestValue));
        }
    }
}
