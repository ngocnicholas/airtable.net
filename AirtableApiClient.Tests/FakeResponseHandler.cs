using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace AirtableApiClient.Tests
{
    internal class FakeResponseHandler : DelegatingHandler
    {
        private readonly Dictionary<string, MethodAndResponse> _FakeResponses = new Dictionary<string, MethodAndResponse>();

        internal class MethodAndResponse
        {
            public HttpMethod Method { get; set; } = HttpMethod.Get;
            public HttpResponseMessage? Response { get; set; }
            public string? BodyText { get; set; }
        }

        internal void AddFakeResponse(string uri, HttpMethod method, HttpResponseMessage response, string? bodyText=null)
        {
            if (_FakeResponses.ContainsKey(uri))        // Uri is obsolete --> Remove it, so that we can add the new one with the same key but different value.
            {
                _FakeResponses.Remove(uri);
            }
            _FakeResponses.Add(uri, new MethodAndResponse { Method = method, Response = response, BodyText = bodyText });
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (_FakeResponses.ContainsKey(request.RequestUri.AbsoluteUri))
            {
                if (_FakeResponses[request.RequestUri.AbsoluteUri].Method == request.Method)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        throw new TaskCanceledException("A task was canceled.");
                    }                    
                    string? bodyText = null;
                    string? dictBodyText = _FakeResponses[request.RequestUri.AbsoluteUri].BodyText;
                    if (dictBodyText != null)
                    { 
                        bodyText = await request.Content.ReadAsStringAsync();
                    }


                    if (dictBodyText == null || bodyText == dictBodyText)
                    {
                        var response = _FakeResponses[request.RequestUri.AbsoluteUri].Response;
                        if (response == null)
                        {
                            throw new InvalidOperationException("Response cannot be null.");
                        }
                        return response;
                    }
                }
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request };
        }

#if false
        private static string GetRequestBody()
        {
            var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
            bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
            var bodyText = bodyStream.ReadToEnd();
            return bodyText;
        }
#endif
    }
}
