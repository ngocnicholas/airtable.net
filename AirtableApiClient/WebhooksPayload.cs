using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class PayloadList
    {
        [JsonPropertyName("payloads")]
        [JsonInclude]
        public WebhooksPayload[]? Payloads;

        [JsonPropertyName("cursor")]
        [JsonInclude]
        public int Cursor;

        [JsonPropertyName("mightHaveMore")]
        [JsonInclude]
        public bool MightHaveMore;
    }

    public class WebhooksPayload
    {
        [JsonPropertyName("timestamp")]
        [JsonInclude]
        public DateTime Timestamp { get; set; }    // The time the action occurred.

        [JsonPropertyName("baseTransactionNumber")]
        [JsonInclude]
        public int BaseTransactionNumber { get; set; }

        [JsonPropertyName("payloadFormat")]
        [JsonInclude]
        public string? PayloadFormat { get; set; }

        [JsonPropertyName("actionMetadata")]
        [JsonInclude]
        public WebhooksAction? ActionMetadata { get; set; }

        [JsonPropertyName("changedTablesById")]
        [JsonInclude]
        public Dictionary<string, WebhooksTableChanged>? ChangedTablesById { get; set; }

        [JsonPropertyName("createdTablesById")]
        [JsonInclude]
        public Dictionary<string, WebhooksTableCreated>? CreatedTablesById { get; set; }

        [JsonPropertyName("destroyedTableIds")]
        [JsonInclude]
        public string[]? DestroyedTableIds { get; set; }

        // Only error responses have the "error" and "code" fields.
        [JsonPropertyName("error")]
        [JsonInclude]
        public bool? Error { get; set; }

        [JsonPropertyName("code")]          // "INVALID_FILTERS" | "INVALID_HOOK" | "INTERNAL_ERROR" or more may be introduced in the future
        [JsonInclude]
        public string? Code { get; set; }
    }

}
