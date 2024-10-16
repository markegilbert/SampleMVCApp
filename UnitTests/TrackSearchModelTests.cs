using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SampleApp.Models;
//using NSubstitute;

namespace UnitTests
{
    [TestFixture]
    public class TrackSearchModelTests
    {
        private TrackSearchModel _Model;
        private IEnumerable<ValidationResult>? _ValidationResults;

        [SetUp]
        public void SetUp()
        {
            this._Model = new TrackSearchModel();
            this._ValidationResults = null;
        }


        [Test]
        public void Validate_NoPropertiesSet_ValidationFails()
        {
            this._ValidationResults = this._Model.Validate(new ValidationContext(this._Model));

            Assert.That(this._ValidationResults, Is.Not.Null, "ValidationResults should not have been null");
            Assert.That(this._ValidationResults.Count(), Is.EqualTo(1), "ValidationResults should have had a entry");
        }
        [Test]
        public void Validate_TrackNameSetToSomethingValid_ValidationPasses()
        {
            this._Model.TrackName = "Test Track";

            this._ValidationResults = this._Model.Validate(new ValidationContext(this._Model));

            Assert.That(this._ValidationResults, Is.Not.Null, "ValidationResults should not have been null");
            Assert.That(this._ValidationResults.Count(), Is.EqualTo(0), "ValidationResults should have been empty");
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void Validate_TrackNameSetToSomethingInvalid_ValidationFails(String TestValue)
        {
            this._Model.TrackName = TestValue;

            this._ValidationResults = this._Model.Validate(new ValidationContext(this._Model));

            Assert.That(this._ValidationResults, Is.Not.Null, "ValidationResults should not have been null");
            Assert.That(this._ValidationResults.Count(), Is.EqualTo(1), "ValidationResults should have been empty");
        }
        [Test]
        public void Validate_ArtistNameSet_ValidationPasses()
        {
            this._Model.ArtistName = "Test Artist";

            this._ValidationResults = this._Model.Validate(new ValidationContext(this._Model));

            Assert.That(this._ValidationResults, Is.Not.Null, "ValidationResults should not have been null");
            Assert.That(this._ValidationResults.Count(), Is.EqualTo(0), "ValidationResults should have been empty");
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void Validate_ArtistNameSetToSomethingInvalid_ValidationFails(String TestValue)
        {
            this._Model.ArtistName = TestValue;

            this._ValidationResults = this._Model.Validate(new ValidationContext(this._Model));

            Assert.That(this._ValidationResults, Is.Not.Null, "ValidationResults should not have been null");
            Assert.That(this._ValidationResults.Count(), Is.EqualTo(1), "ValidationResults should have been empty");
        }
        [Test]
        public void Validate_BothTrackAndArtistNameSet_ValidationPasses()
        {
            this._Model.TrackName = "Test Track";
            this._Model.ArtistName = "Test Artist";

            this._ValidationResults = this._Model.Validate(new ValidationContext(this._Model));

            Assert.That(this._ValidationResults, Is.Not.Null, "ValidationResults should not have been null");
            Assert.That(this._ValidationResults.Count(), Is.EqualTo(0), "ValidationResults should have been empty");
        }
    }
}
