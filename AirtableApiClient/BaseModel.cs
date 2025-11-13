using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public enum PermissionLevel
    {
        none,
        read,
        comment,
        edit,
        create
    }

    public enum BaseSchemaInclude
    {
        visibleFieldIds
    }


    public class BaseModel
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        [JsonInclude]
        public string? Name { get; set; }

        [JsonPropertyName("permissionLevel")]
        [JsonInclude]
        public string? PermissionLevel { get; set; }     // none | read | comment | edit | create 
    }


    public class BaseList
    {
        [JsonPropertyName("offset")]
        [JsonInclude]
        public string? Offset { get; set; }

        [JsonPropertyName("bases")]
        [JsonInclude]
        public BaseModel[]? Bases { get; set; }
    }


    public class CreatedBase
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string? Id { get; set; }

        [JsonPropertyName("tables")]
        [JsonInclude]
        public TableModel[]? Tables { get; set; }
    }

}


