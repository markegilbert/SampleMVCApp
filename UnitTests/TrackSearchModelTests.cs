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


        [Test]
        public void NumberOfResultsPerPage_InstantiateClass_DefaultsTo5()
        {
            Assert.That(this._Model.NumberOfResultsPerPage, Is.EqualTo(5));
        }
        [Test]
        public void NumberOfResultsPerPage_OverriddenTo10_Returns10()
        {
            this._Model.NumberOfResultsPerPage = 10;
            Assert.That(this._Model.NumberOfResultsPerPage, Is.EqualTo(10));
        }


        [Test]
        public void NumberOfPages_TotalNumberOfResultsIs0_Returns0()
        {
            this._Model.TotalNumberOfResults = 0;
            Assert.That(this._Model.NumberOfPages, Is.EqualTo(0));
        }
        [Test]
        public void NumberOfPages_TotalNumberOfResults_IsLessThan_NumberOfResultsPerPage_Returns1()
        {
            this._Model.TotalNumberOfResults = 2;
            this._Model.NumberOfResultsPerPage = 5;
            Assert.That(this._Model.NumberOfPages, Is.EqualTo(1));
        }
        [Test]
        public void NumberOfPages_TotalNumberOfResults_IsEqualTo_NumberOfResultsPerPage_Returns1()
        {
            this._Model.TotalNumberOfResults = 5;
            this._Model.NumberOfResultsPerPage = 5;
            Assert.That(this._Model.NumberOfPages, Is.EqualTo(1));
        }
        [Test]
        public void NumberOfPages_TotalNumberOfResults_IsOneMoreThan_NumberOfResultsPerPage_Returns2()
        {
            this._Model.TotalNumberOfResults = 6;
            this._Model.NumberOfResultsPerPage = 5;
            Assert.That(this._Model.NumberOfPages, Is.EqualTo(2));
        }
        [Test]
        public void NumberOfPages_TotalNumberOfResults_IsTwice_NumberOfResultsPerPage_Returns2()
        {
            this._Model.TotalNumberOfResults = 10;
            this._Model.NumberOfResultsPerPage = 5;
            Assert.That(this._Model.NumberOfPages, Is.EqualTo(2));
        }
        [Test]
        public void NumberOfPages_TotalNumberOfResults_IsTwicePlus1_NumberOfResultsPerPage_Returns3()
        {
            this._Model.TotalNumberOfResults = 11;
            this._Model.NumberOfResultsPerPage = 5;
            Assert.That(this._Model.NumberOfPages, Is.EqualTo(3));
        }


        [Test]
        public void TotalNumberOfResults_InstantiateClass_Returns0()
        {
            Assert.That(this._Model.TotalNumberOfResults, Is.EqualTo(0));
        }
        [Test]
        public void TotalNumberOfResults_SetTo1_Returns1()
        {
            this._Model.TotalNumberOfResults = 1;
            Assert.That(this._Model.TotalNumberOfResults, Is.EqualTo(1));
        }
        [Test]
        public void TotalNumberOfResults_SetToSomethingNegative_Returns0()
        {
            this._Model.TotalNumberOfResults = -42;
            Assert.That(this._Model.TotalNumberOfResults, Is.EqualTo(0));
        }


        [Test]
        public void ResultsCounterStart_InstantiateClass_Returns0()
        {
            Assert.That(this._Model.ResultsCounterStart, Is.EqualTo(0));
        }
        [TestCase(1, 1)]
        [TestCase(2, 6)]
        [TestCase(3, 11)]
        public void ResultsCounterStart_NumberResultsPerPageIs5_GivenPageCount_ReturnsCorrectStartingPoint(int PageCount, int ExpectedStart)
        {
            this._Model.Page = PageCount;
            this._Model.NumberOfResultsPerPage = 5;
            Assert.That(this._Model.ResultsCounterStart, Is.EqualTo(ExpectedStart));
        }


        [Test]
        public void Page_TotalNumberOfResultsIs0_Returns0()
        {
            this._Model.TotalNumberOfResults = 0;
            Assert.That(this._Model.Page, Is.EqualTo(0));
        }
        [Test]
        public void Page_TotalNumberOfResultsIs10_PageNotExplicitlySet_Returns1()
        {
            this._Model.TotalNumberOfResults = 10;
            Assert.That(this._Model.Page, Is.EqualTo(1));
        }
        [Test]
        public void Page_TotalNumberOfResultsIs10_PageExplicitlySetTo2_Returns2()
        {
            this._Model.TotalNumberOfResults = 10;
            this._Model.Page = 2;
            Assert.That(this._Model.Page, Is.EqualTo(2));
        }

    }
}
