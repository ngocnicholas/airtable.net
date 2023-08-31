using System.Text.Json.Serialization;


namespace AirtableApiClient
{
    public class WebhooksAction
    {
        public WebhooksAction(string source)
        {
            Source = source;
        }

        [JsonPropertyName("source")]
        [JsonInclude]
        public string Source { get; internal set; }

        [JsonPropertyName("sourceMetadata")]
        [JsonInclude]
        public SourceMetaData SourceMetadata { get; internal set; }
    }

    public class SourceMetaData     // Look into using an Abstract class?
    {
        [JsonPropertyName("user")]
        [JsonInclude]
        public WebhooksUser User { get; internal set; }

        [JsonPropertyName("viewId")]
        [JsonInclude]
        public string ViewId { get; internal set; }

        [JsonPropertyName("automationId")]
        [JsonInclude]
        public string AutomationId { get; internal set; }
    }

    public class WebhooksUser
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("email")]
        [JsonInclude]
        public string Email { get; internal set; }

        [JsonPropertyName("permissionLevel")]
        [JsonInclude]
        public string PermissionLevel { get; internal set; } // "none" | "read" | "comment" | "edit" | "create"

        [JsonPropertyName("name")] // optional
        [JsonInclude]
        public string Name { get; internal set; }

        [JsonPropertyName("profilePicUrl")] // optional
        [JsonInclude]
        public string ProfilePicUrl { get; internal set; }
    }
}
