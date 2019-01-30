using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

namespace AirtableApiClient
{
    internal class RetryHandler
    {
        private HttpMethod method;
        private string content;
        private Uri requestUri;
        private readonly HttpClient Client;
        private AirtableBase airtableBase;

        public RetryHandler(AirtableBase airtableBase, HttpClient client)
        {
            Client = client;
            this.airtableBase = airtableBase;
        }


        public async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request)
        {
            method = request.Method;
            requestUri = request.RequestUri;
            content = null;
            if (request.Content != null)
            {
                content = await request.Content.ReadAsStringAsync();
            }
            int dueTimeDelay = airtableBase.RetryDelayIfRateLimited;
            int retries = 0;

            HttpResponseMessage response = await Client.SendAsync(request);

            while (response.StatusCode == (HttpStatusCode)429 &&
                retries < AirtableBase.MAX_RETRIES &&
                !airtableBase.NoRetryIfRateLimited)
            {
                await Task.Delay(dueTimeDelay);
                var requestRegenerated = RegenerateRequest();
                response = await Client.SendAsync(requestRegenerated);
                retries++;
                dueTimeDelay *= 2;
            }

            return response;
        }


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
