using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    // Adapted from https://dev.to/n_develop/mocking-the-httpclient-in-net-core-with-nsubstitute-k4j
    public class MockHttpMessageHandler: HttpMessageHandler
    {
        private readonly String _Response;
        private readonly HttpStatusCode _StatusCode;

        public String RequestBody { get; private set; }
        public Uri RequestUri { get; private set; }
        public int NumberOfCalls { get; private set; }

        public MockHttpMessageHandler(String Response, HttpStatusCode StatusCode)
        {
            this._Response = Response;
            this._StatusCode = StatusCode;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage Request, CancellationToken CancellationToken)
        {
            this.NumberOfCalls++;

            if (Request.Content != null) // Could be a GET-request without a body
            {
                this.RequestBody = await Request.Content.ReadAsStringAsync();
            }

            this.RequestUri = Request.RequestUri;

            return new HttpResponseMessage
            {
                StatusCode = this._StatusCode,
                Content = new StringContent(this._Response)
            };
        }
    }
}
