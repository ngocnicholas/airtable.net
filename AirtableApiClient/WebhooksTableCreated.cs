using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class WebhooksTableCreated
    {
        [JsonPropertyName("fieldsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksField> FieldsById { get;  set; }

        [JsonPropertyName("recordsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksCreatedRecord> RecordsById { get;  set; }

        [JsonPropertyName("metadata")]
        [JsonInclude]
        public WebhooksMetadata Metadata { get;  set; }
    }
}
