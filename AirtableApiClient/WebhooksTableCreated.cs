using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class WebhooksTableCreated
    {
        [JsonPropertyName("fieldsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksField>? FieldsById { get;  set; } = new Dictionary<string, WebhooksField>();

        [JsonPropertyName("recordsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksCreatedRecord>? RecordsById { get;  set; } = new Dictionary<string, WebhooksCreatedRecord>();

        [JsonPropertyName("metadata")]
        [JsonInclude]
        public WebhooksMetadata? Metadata { get;  set; } = null;
    }
}
