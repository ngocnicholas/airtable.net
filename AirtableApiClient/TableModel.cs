using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class TableModelList
    {
        [JsonPropertyName("tables")]
        [JsonInclude]
        public TableModel[] Tables { get; internal set; }
    }

    public class TableModel
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("primaryFieldId")]
        [JsonInclude]
        public string PrimaryFieldId { get; internal set; }

        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; internal set; }

        [JsonPropertyName("description")]
        [JsonInclude]
        public string Description { get; internal set; } // optional<string>

        [JsonPropertyName("fields")]
        [JsonInclude]
        public FieldModel[] Fields { get; internal set; }

        [JsonPropertyName("views")]
        [JsonInclude]
        public AirtableViewModel[] Views { get; internal set; }
    }

    public class FieldModel
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("type")]
        [JsonInclude]
        public string Type { get; internal set; }        // optional<Field type> https://airtable.com/developers/web/api/model/field-type

        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; internal set; }

        [JsonPropertyName("description")]
        [JsonInclude]
        public string Description { get; internal set; } // optional<string>

        [JsonPropertyName("options")]
        [JsonInclude]
        public object Options { get; internal set; }     // optional<object>
    }

    public class AirtableViewModel
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("type")]
        [JsonInclude]
        public string Type { get; internal set; }              // "grid" | "form" | "calendar" | "gallery" | "kanban" | "timeline" | "block"

        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; internal set; }


        [JsonPropertyName("visibleFieldIds")]
        [JsonInclude]
        public string[] VisibleFieldIds { get; internal set; } // optional<array of strings>
    }

}
