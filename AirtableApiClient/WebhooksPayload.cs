using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class PayloadList
    {
        [JsonPropertyName("payloads")]
        [JsonInclude]
        public WebhooksPayload[] Payloads;

        [JsonPropertyName("cursor")]
        [JsonInclude]
        public int Cursor;

        [JsonPropertyName("mightHaveMore")]
        [JsonInclude]
        public bool MightHaveMore;
    }

    public class WebhooksPayload
    {
        [JsonPropertyName("timeStamp")]
        [JsonInclude]
        public string TimeStamp { get; internal set; }

        [JsonPropertyName("baseTransactionNumber")]
        [JsonInclude]
        public int BaseTransactionNumber { get; internal set; }

        [JsonPropertyName("payloadFormat")]
        [JsonInclude]
        public string PayloadFormat { get; internal set; }

        [JsonPropertyName("actionMetadata")]
        [JsonInclude]
        public WebhooksAction ActionMetadata { get; internal set; }
    }

    public class ErrorPayload
    {
        [JsonPropertyName("error")]
        [JsonInclude]
        public bool Error { get; internal set; }

        [JsonPropertyName("code")]          // "INVALID_FILTERS" | "INVALID_HOOK" | "INTERNAL_ERROR"
        [JsonInclude]
        public string[] Code { get; internal set; }

        [JsonPropertyName("timeStamp")]
        [JsonInclude]
        public string TimeStamp { get; internal set; }

        [JsonPropertyName("baseTransactionNumber")]
        [JsonInclude]
        public int BaseTransactionNumber { get; internal set; }

        [JsonPropertyName("payloadFormat")]
        [JsonInclude]
        public string PayloadFormat { get; internal set; }  // Only "v0" for now

        [JsonPropertyName("actionMetadata")]
        [JsonInclude]
        public WebhooksAction ActionMetadata { get; internal set; }

        [JsonPropertyName("changedTablesById")]
        [JsonInclude]
        public Dictionary<string, WebhooksTableChanged> ChangedTablesById { get; set; }

        [JsonPropertyName("createdTablesById")]
        [JsonInclude]
        public Dictionary<string, WebhooksTableCreated> CreatedTablesById { get; set; }

        [JsonPropertyName("destroyedTableIds")]
        [JsonInclude]
        public string[] DestroyedTableIds { get; set; }
    }

}
