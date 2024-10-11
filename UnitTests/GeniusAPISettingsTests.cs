using System;
using NUnit.Framework;
using SampleApp.Config;


namespace UnitTests
{
    [TestFixture]
    public class GeniusAPISettingsTests
    {
        private GeniusAPISettings _Settings;

        [SetUp]
        public void SetUp()
        {
            this._Settings = new GeniusAPISettings();
        }

        [Test]
        public void ClientAccessToken_OnInstantiation_IsEmpty()
        {
            Assert.That(this._Settings.ClientAccessToken, Is.Not.Null, "ClientAccessToken should not have been null");
            Assert.That(this._Settings.ClientAccessToken, Is.Empty, "ClientAccessToken should have been empty");
        }
        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void ClientAccessToken_AssignedInvalidValue_ReturnsEmpty(String TestValue)
        {
            this._Settings.ClientAccessToken = TestValue;

            Assert.That(this._Settings.ClientAccessToken, Is.Not.Null, "ClientAccessToken should not have been null");
            Assert.That(this._Settings.ClientAccessToken, Is.Empty, "ClientAccessToken should have been empty");
        }
    }
}
