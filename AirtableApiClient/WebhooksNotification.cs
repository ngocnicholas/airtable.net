using System;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
     public class WebhooksNotification
    {
        [JsonPropertyName("success")]
        [JsonInclude]
        public bool Success { get; set; }

        [JsonPropertyName("completionTimestamp")]
        [JsonInclude]
        public DateTime completionTimestamp { get; set; }  // The time of the most recent notification.

        [JsonPropertyName("durationMs")]
        [JsonInclude]
        public float DurationMs { get; set; }     // The roundtrip duration of the network call.

        [JsonPropertyName("retryNumber")]
        [JsonInclude]
        public int RetryNumber { get; set; }

        [JsonPropertyName("error")]
        [JsonInclude]
        public Error? Error { get; set; }                    // This field only exists in case of error

        [JsonPropertyName("willBeRetried")]
        [JsonInclude]
        public bool WillBeRetried { get; set; }    // This field only exists in case of error
    }

    public class Error
    {
        [JsonPropertyName("message")]
        [JsonInclude]
        public string? Message { get; set; }
    }

}
