using System.Text.Json.Serialization;

namespace AirtableApiClient
{ 
    public class TableConfig            // used in the request body of  API CreateBase
    {
        [JsonPropertyName("name")]
        [JsonInclude]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        [JsonInclude]
        public string? Description { get; set; }

        // Important NOTE: IFieldConfig[], not FieldConfig[], so that it can hold any write-capable field,
        // whether it’s a pure write type (FieldConfig descendants) or a read-write type (FieldModelConfig)
        [JsonPropertyName("fields")]
        [JsonInclude]
        public IFieldConfig[] Fields { get; set; } = Array.Empty<IFieldConfig>();
    }
}
