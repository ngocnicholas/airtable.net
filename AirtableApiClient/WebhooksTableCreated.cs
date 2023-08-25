using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class WebhooksTableCreated
    {
        [JsonPropertyName("fieldsById")]
        [JsonInclude]
        public Dictionary<string, Field> FieldsById { get; set; }

        [JsonPropertyName("recordsById")]
        [JsonInclude]
        public Dictionary<string, Record> RecordsById { get; set; }

        [JsonPropertyName("metaData")]
        [JsonInclude]
        public Metadata Metdata { get; set; }

    }

    public class Record     // Not the same as AirtableRecord
    {
        [JsonPropertyName("createdTime")]
        [JsonInclude]
        public string CreatedTime;

        [JsonPropertyName("cellValuesByFieldId")]
        [JsonInclude]
        public Dictionary<string, object> CellValuesByFieldId; //  Cell Value V2 is the 'object' in the Dictionary
                                                                // Same as RecordData used in WebhooksTableChanged
    }
}
