using System.Text.Json.Serialization;

namespace AirtableApiClient
{
     public class WebhooksNotification
    {
        public WebhooksNotification()
        {

        }

        [JsonPropertyName("success")]
        [JsonInclude]
        public bool Success { get; internal set; }

        [JsonPropertyName("completionTimestamp")]
        [JsonInclude]
        public string completionTimestamp { get; internal set; }

        [JsonPropertyName("durationMs")]
        [JsonInclude]
        public float DurationMs { get; internal set; }     // The roundtrip duration of the network call.

        [JsonPropertyName("retryNumber")]
        [JsonInclude]
        public int RetryNumber { get; internal set; }

        [JsonPropertyName("error")]
        [JsonInclude]
        public Error Error { get; set; }    // This field only exist in case of error

        [JsonPropertyName("willBeRetried")]
        [JsonInclude]
        public bool WillBeRetried { get; internal set; }    // This field only exist in case of error


    }

    #if false   // Should make this a derived class: No
    public class WebhooksNotificationError : WebhooksNotification
    {
        [JsonPropertyName("error")]
        [JsonInclude]
        public Error Error { get; set; }

        [JsonPropertyName("willBeRetried")]
        [JsonInclude]
        public bool WillBeRetried { get; internal set; }
    }
    #endif

    public class Error
    {
        [JsonPropertyName("message")]
        [JsonInclude]
        public string Message { get; internal set; }
    }

}
