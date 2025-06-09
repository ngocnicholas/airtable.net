using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class TableModelList
    {
        [JsonPropertyName("tables")]
        [JsonInclude]
        public TableModel[] Tables { get; set; }
    }

    public class TableModel
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("primaryFieldId")]
        [JsonInclude]
        public string PrimaryFieldId { get; set; }

        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; internal set; }

        [JsonPropertyName("description")]
        [JsonInclude]
        public string Description { get; set; } // optional<string>

        // -------------- NOTE: Need to add dateDependencySettings property here later -------------------------

        [JsonPropertyName("fields")]
        [JsonInclude]
        public FieldDefinition[] Fields { get; set; }

        [JsonPropertyName("views")]
        [JsonInclude]
        public AirtableViewModel[] Views { get; set; }
    }


    public class AirtableViewModel
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        [JsonInclude]
        public string Type { get; set; }              // "grid" | "form" | "calendar" | "gallery" | "kanban" | "timeline" | "block"

        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; internal set; }


        [JsonPropertyName("visibleFieldIds")]
        [JsonInclude]
        public string[] VisibleFieldIds { get; set; } // optional<array of strings>
    }

}
