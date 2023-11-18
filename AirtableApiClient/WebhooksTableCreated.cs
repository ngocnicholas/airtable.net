using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class WebhooksTableCreated
    {
        [JsonPropertyName("fieldsById")]
        [JsonInclude]
        public Dictionary<string, Field> FieldsById { get; internal set; }

        [JsonPropertyName("recordsById")]
        [JsonInclude]
        public Dictionary<string, CreatedRecord> RecordsById { get; internal set; }

        [JsonPropertyName("metaData")]
        [JsonInclude]
        public Metadata Metdata { get; internal set; }
    }
}
