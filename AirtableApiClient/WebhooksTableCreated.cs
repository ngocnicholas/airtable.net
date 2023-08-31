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
        public Dictionary<string, Record> RecordsById { get; internal set; }

        [JsonPropertyName("metaData")]
        [JsonInclude]
        public Metadata Metdata { get; internal set; }

    }

    public class Record     // Not the same as AirtableRecord
    {
        [JsonPropertyName("createdTime")]
        [JsonInclude]
        public string CreatedTime { get; internal set; }

        [JsonPropertyName("cellValuesByFieldId")]
        [JsonInclude]
        public Dictionary<string, object> CellValuesByFieldId { get; internal set; } //  Cell Value V2 is the 'object' in the Dictionary
                                                                // Same as RecordData used in WebhooksTableChanged
    }
}
