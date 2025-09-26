using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class TableModelList
    {
        [JsonPropertyName("tables")]
        [JsonInclude]
        public TableModel[]? Tables { get; set; }
    }

    public class TableModel
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string? Id { get; internal set; }

        [JsonPropertyName("primaryFieldId")]
        [JsonInclude]
        public string? PrimaryFieldId { get; set; }

        [JsonPropertyName("dateDependencySettings")]
        [JsonInclude]
        public DateDependencySettings? DateDependencySettings { get; set; }

        [JsonPropertyName("name")]
        [JsonInclude]
        public string? Name { get; internal set; }

        [JsonPropertyName("description")]
        [JsonInclude]
        public string? Description { get; set; } // optional<string>

        [JsonPropertyName("fields")]
        [JsonInclude]
        public FieldConfig[]? Fields { get; set; }

        [JsonPropertyName("views")]
        [JsonInclude]
        public ViewConfig[]? Views { get; set; }
    }


    public class ViewConfig
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string? Id { get; set; }

        [JsonPropertyName("type")]
        [JsonInclude]
        public string? Type { get; set; }              // "grid" | "form" | "calendar" | "gallery" | "kanban" | "timeline" | "block"

        [JsonPropertyName("name")]
        [JsonInclude]
        public string? Name { get; internal set; }


        [JsonPropertyName("visibleFieldIds")]
        [JsonInclude]
        public string[]? VisibleFieldIds { get; set; } // optional<array of strings>
    }

}
