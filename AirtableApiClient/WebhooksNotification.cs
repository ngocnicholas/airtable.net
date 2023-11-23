using System;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
     public class WebhooksNotification
    {
        [JsonPropertyName("success")]
        [JsonInclude]
        public bool Success { get; internal set; }

        [JsonPropertyName("completionTimestamp")]
        [JsonInclude]
        public DateTime completionTimestamp { get; internal set; }

        [JsonPropertyName("durationMs")]
        [JsonInclude]
        public float DurationMs { get; internal set; }     // The roundtrip duration of the network call.

        [JsonPropertyName("retryNumber")]
        [JsonInclude]
        public int RetryNumber { get; internal set; }

        [JsonPropertyName("error")]
        [JsonInclude]
        public Error Error { get; set; }                    // This field only exists in case of error

        [JsonPropertyName("willBeRetried")]
        [JsonInclude]
        public bool WillBeRetried { get; internal set; }    // This field only exists in case of error
    }

    public class Error
    {
        [JsonPropertyName("message")]
        [JsonInclude]
        public string Message { get; internal set; }
    }

}
