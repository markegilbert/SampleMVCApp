using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SampleApp.Services;


namespace UnitTests
{
    [TestFixture]
    public class GeniusSearchResponseTests
    {
        private GeniusSearchResponse _Response;

        [Test]
        public void GetEmptyObject_ReturnsAVeryBasicObject()
        {
            this._Response = GeniusSearchResponse.GetEmptyObject();

            Assert.That(this._Response, Is.Not.Null, "Response should not have been null");
            Assert.That(this._Response.Meta, Is.Not.Null, "Response.Meta should not have been null");
            Assert.That(this._Response.Meta.Status, Is.EqualTo(200), "Response.Meta.Status was not correct");
            Assert.That(this._Response.ResponseData, Is.Not.Null, "Response.ResponseData should not have been null");
            Assert.That(this._Response.ResponseData.Hits, Is.Not.Null, "Response.ResponseData.Hits should not have been null");
            Assert.That(this._Response.ResponseData.Hits, Is.Empty, "Response.ResponseData.Hits should have been empty");
        }

    }
}
