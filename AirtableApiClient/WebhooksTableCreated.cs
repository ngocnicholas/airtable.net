using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class WebhooksTableCreated
    {
        [JsonPropertyName("fieldsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksField> FieldsById { get; internal set; }

        [JsonPropertyName("recordsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksCreatedRecord> RecordsById { get; internal set; }

        [JsonPropertyName("metadata")]
        [JsonInclude]
        public WebhooksMetadata Metadata { get; internal set; }
    }
}
