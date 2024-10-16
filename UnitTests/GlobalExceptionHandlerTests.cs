using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using SampleApp.ErrorHandling;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace UnitTests
{
    [TestFixture]
    public class GlobalExceptionHandlerTests
    {
        private GlobalExceptionHandler? _Handler;
        private ILogger<GlobalExceptionHandler>? _Logger;
        private ArgumentNullException _ArgumentNullException;
        private HttpContext? _HttpContextMock;
        private HttpClient? _HttpClientMock;


        [SetUp]
        public void SetUp()
        {
            this._Logger = Substitute.For<ILogger<GlobalExceptionHandler>>();
            this._HttpContextMock = Substitute.For<HttpContext>();
            this._ArgumentNullException = null;
        }

        [Test]
        public void InstantiateClass_LoggerIsNull_ExceptionThrown()
        {
            this._ArgumentNullException = Assert.Throws<ArgumentNullException>(() => new GlobalExceptionHandler(null));
            Assert.That(this._ArgumentNullException.Message.Contains("'Logger'"), "The exception didn't mention the correct parameter");
        }

        [Test]
        public async Task TryHandleAsync_LoggerIsInvokedCorrectly()
        {
            this._Handler = new GlobalExceptionHandler(this._Logger);
            Exception TestException = new Exception("Some Exception Message");

            await this._Handler.TryHandleAsync(this._HttpContextMock, TestException, new CancellationToken());

            this._Logger.Received(1).Log(LogLevel.Error, 
                                        Arg.Any<EventId>(),
                                        Arg.Any<object>(),
                                        TestException,
                                        Arg.Any<Func<object, Exception, string>>());
        }
    }

}
