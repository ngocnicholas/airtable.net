using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;

namespace AirtableApiClient
{
    internal class HttpClientWithRetries : IDisposable
    {
        public const int MAX_RETRIES = 3;
        public const int MIN_RETRY_DELAY_MILLISECONDS_IF_RATE_LIMITED = 2000;   // 1 second
        public bool ShouldNotRetryIfRateLimited { get; set; }

        private int retryDelayMilliSecondsIfRateLimited;
        public int RetryDelayMilliSecondsIfRateLimited
        {
            get { return retryDelayMilliSecondsIfRateLimited; }
            set
            {
                if (value < MIN_RETRY_DELAY_MILLISECONDS_IF_RATE_LIMITED)
                {
                    throw new ArgumentException(
                        String.Format("Retry Delay cannot be less than {0} ms.", MIN_RETRY_DELAY_MILLISECONDS_IF_RATE_LIMITED),
                        "RetryDelayMilliSecondsIfRateLimited");
                }
                retryDelayMilliSecondsIfRateLimited = value;
            }
        }

        private HttpMethod method;
        private string content;
        private Uri requestUri;
        private readonly HttpClient client;


        //----------------------------------------------------------------------------
        // 
        // HttpClientWithRetries.HttpClientWithRetries
        //    constructor
        // 
        //----------------------------------------------------------------------------

        public HttpClientWithRetries(DelegatingHandler delegatingHandler, string apiKey)
        {
            // Allow retries by default.
            ShouldNotRetryIfRateLimited = false;

            // Start with the minimum delay then increase exponentially with a base of 2.
            RetryDelayMilliSecondsIfRateLimited = MIN_RETRY_DELAY_MILLISECONDS_IF_RATE_LIMITED;

            if (delegatingHandler == null)
            {
                client = new HttpClient();                      // for communicating with Airtable
            }
            else
            {
                client = new HttpClient(delegatingHandler);     // for communicating with the specified handler
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }


        //----------------------------------------------------------------------------
        // 
        // HttpClientWithRetries.Dispose
        // 
        //----------------------------------------------------------------------------

        public void Dispose()
        {
            client.Dispose();
        }


        //----------------------------------------------------------------------------
        // 
        // HttpClientWithRetries.SendAsync
        //      This method has preforms retries with exponential back off if the generic 
        // SendAsync returns a HttpStatusCode of 429 for Too Many Request.
        // 
        //----------------------------------------------------------------------------

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            method = request.Method;
            requestUri = request.RequestUri;
            content = null;

            // Store content so that we can regenerate HttpRequestMessage in case
            // of retries.
            if (request.Content != null)
            {
                content = await request.Content.ReadAsStringAsync();
            }

            int dueTimeDelay = RetryDelayMilliSecondsIfRateLimited;
            int retries = 0;

            HttpResponseMessage response = await client.SendAsync(request);

            while (response.StatusCode == (HttpStatusCode)429 &&
                retries < MAX_RETRIES &&
                !ShouldNotRetryIfRateLimited)
            {
                await Task.Delay(dueTimeDelay);
                var requestRegenerated = RegenerateRequest();
                response = await client.SendAsync(requestRegenerated);
                retries++;
                dueTimeDelay *= 2;
            }

            return response;
        }


        //----------------------------------------------------------------------------
        // 
        // HttpClientWithRetries.RegenerateRequest
        //  A new HttpRequestMessage needs to be generated for each retry
        // because the same message cannot be used more than once.
        // 
        //----------------------------------------------------------------------------

        private HttpRequestMessage RegenerateRequest()
        {
            var request = new HttpRequestMessage(method, requestUri);
            if (content != null)
            {
                request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            }
            return request;
        }
    }   // end class
}   // end namespace
