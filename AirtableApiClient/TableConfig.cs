using System.Text.Json.Serialization;

namespace AirtableApiClient
{ 
    public class TableConfig            // used in the request body of  API CreateBase
    {
        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        [JsonInclude]
        public string Description { get; set; }

        [JsonPropertyName("fields")]
        [JsonInclude]
        public FieldModel[] Fields { get; set; }
    }
}
