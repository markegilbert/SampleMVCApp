using NUnit.Framework;
using SampleApp.Config;

namespace UnitTests
{
    [TestFixture]
    public class SampleAppConfigTests
    {

        [Test]
        public void InstantiateClass_ConnectionStringsIsInitialized()
        {
            SampleAppConfig config = new SampleAppConfig();
            Assert.That(config.ConnectionStrings, Is.Not.Null);
        }


    }
}
