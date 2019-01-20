using System;
using System.Text;
using System.Net.Http;
using System.Threading;

namespace AirtableApiClient
{
    public partial class AirtableBase : IDisposable
    {
        private const int MAX_RETRIES = 5;
        private const int MIN_RETRY_DELAY_IF_RATE_LIMITED = 1000;   // 1 seconds

        private bool noRetryIfRateLimited = false;      // Allow retries by default

        // Start with the minimum delay then increase exponentially with a base of 2.
        private int retryDelayIfRateLimited = MIN_RETRY_DELAY_IF_RATE_LIMITED;     
 
        //----------------------------------------------------------------------------
        // 
        // AirtableBase.InitRetrySendParameters
        // 
        // Called to initialize 2 re-send parameters.
        // If this method is not called, default values are used.
        // 
        //----------------------------------------------------------------------------

        public void InitRetrySendParameters(bool noRetryIfRateLimited, int retryDelayIfRateLimited)
        {
            this.noRetryIfRateLimited = noRetryIfRateLimited;
            this.retryDelayIfRateLimited = Math.Max(MIN_RETRY_DELAY_IF_RATE_LIMITED, retryDelayIfRateLimited);
        }


        private class SenderWithRetries
        {
            private const int TIMER_INFINITE = -1;

            private AirtableBase airtableBase;
            private RetryChecker retryChecker;
            private AutoResetEvent autoEvent;
            private RetryEventHandler retryEventHandler;
            private System.Threading.Timer retryTimer;
            private int retries;

            public SenderWithRetries(AirtableBase airtableBase, Uri uri, HttpMethod httpMethod) : this(airtableBase, uri, httpMethod, null)
            {
                // No json string is given so we are not Creating/Updating/Replace a record.
            }

            public SenderWithRetries(AirtableBase airtableBase, Uri uri, HttpMethod httpMethod, string jsonForCreateUpdateReplaceRecord)
            {
                this.airtableBase = airtableBase;

                // Get everything for the timer to be ready.
                this.retryChecker = new RetryChecker(airtableBase, uri, httpMethod, jsonForCreateUpdateReplaceRecord);
                this.autoEvent = new AutoResetEvent(false);
                this.retryEventHandler = new RetryEventHandler(autoEvent);

                // Use System.Threading.Timer so that the callback function will be run on a thread pool thread.
                // Due time is set to 0 so that the callback is invoked immediately.
                // Period is set to infinite so that we will control how much delay we want before invoking callback next.
                // The period will be computed in retryEventHandler.
                this.retryTimer = new System.Threading.Timer(retryChecker.ReSend, retryEventHandler, 0, TIMER_INFINITE);
                this.retries = 1;   // Initialize to 1 because we already sent the request once right after the construction of the retry timer.
            }


            //----------------------------------------------------------------------------
            // 
            // RetryManager.RetrySend
            // 
            // This method is used by the parent AiratabeBase to handle re-send of HTTP methods
            // after receiving a HttpStatuscode of 429 to Too Many Requests.
            // 
            //----------------------------------------------------------------------------

            public AirtableApiException RetrySend()
            {
                AirtableApiException error = null;
                int dueTimeDelay = airtableBase.retryDelayIfRateLimited;
                while (true)
                {
                    autoEvent.WaitOne();
                    error = retryChecker.Error;
                    if (error is AirtableTooManyRequestsException && retries < MAX_RETRIES &&
                        !airtableBase.noRetryIfRateLimited)
                    {
                        retries++;
                        // Since the timer was constructed with the period of TIMER_INFINITE earlier
                        // we now have time to adjust the due time to be longer by increasing it with
                        // an exponent of 2.
                        // Set the period to be TIMER INTINITE again to faciliate the next adjustment. 
                        retryTimer.Change(dueTimeDelay, TIMER_INFINITE);
                        dueTimeDelay *= 2;
                    }
                    else
                    {
                        // Either retries not allowed or exhausted.
                        retryTimer.Dispose();
                        retryTimer = null;
                        break;
                    }
                }
                return error;
            }

            public HttpResponseMessage Response { get { return retryEventHandler.Response; } }


            private class RetryChecker
            {
                private AirtableBase airtableBase;
                private readonly Uri uri;
                private readonly HttpMethod httpMethod;
                private string jsonForCreateUpdateReplaceRecord;

                //----------------------------------------------------------------------------
                // 
                // RetryChecker.RetryChecker
                //    constructor -- only used by parent class RetryManager
                // All the arguments for the constructor are for regenerating the HTTP method
                // because it's not allowed to re-send the same request multiple times.
                // 
                //----------------------------------------------------------------------------

                public RetryChecker(
                    AirtableBase airtableBase,
                    Uri uri,
                    HttpMethod httpMethod,
                    string jsonForCreateUpdateReplaceRecord)
                {
                    this.airtableBase = airtableBase;
                    this.uri = uri;
                    this.httpMethod = httpMethod;
                    this.jsonForCreateUpdateReplaceRecord = jsonForCreateUpdateReplaceRecord;
                }


                //----------------------------------------------------------------------------
                // 
                // RetryChecker.ReSend
                // 
                // This is the callback function for the re-send timer.
                // 
                //----------------------------------------------------------------------------
                public async void ReSend(
                    Object stateInfo)   // object with all the info to perform the re-send event
                {
                        RetryEventHandler handler = (RetryEventHandler)stateInfo;
                        var request = new HttpRequestMessage(httpMethod, uri);      // Need to generate a new request for each re-send.
                        if (jsonForCreateUpdateReplaceRecord != null)   // Create/Update/Replace records: need to provide more info reguarding the record.
                        {
                            request.Content = new StringContent(jsonForCreateUpdateReplaceRecord, Encoding.UTF8, "application/json");
                        }
                        handler.Response = await airtableBase.Client.SendAsync(request);    // Save the response to be processed later.
                        Error = await airtableBase.CheckForAirtableException(handler.Response);
                        handler.AutoEvent.Set();
                }

                public AirtableApiException Error { get; private set; }
            }   // end class RetryChecker


            private class RetryEventHandler
            {
                public AutoResetEvent AutoEvent { get; set; }
                public HttpResponseMessage Response { get; set; }   // For every event, we have one response.

                public RetryEventHandler(AutoResetEvent autoEvt)
                {
                    AutoEvent = autoEvt;
                }
            }   // end RetryEventHandler

        }   // end class RetryManager
    }   // end class AirtableBase
}
