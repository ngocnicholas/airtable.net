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
        public string Source { get; set; }

        [JsonPropertyName("sourceMetadata")]
        [JsonInclude]
        public SourceMetadata? SourceMetadata { get; set; }
    }

    public class SourceMetadata     // Look into using an Abstract class?
    {
        [JsonPropertyName("user")]
        [JsonInclude]
        public WebhooksUser? User { get; set; }

        [JsonPropertyName("viewId")]
        [JsonInclude]
        public string? ViewId { get; set; }

        [JsonPropertyName("automationId")]
        [JsonInclude]
        public string? AutomationId { get; set; }
    }

    public class WebhooksUser
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string? Id { get; set; }

        [JsonPropertyName("email")]
        [JsonInclude]
        public string? Email { get; set; }

        [JsonPropertyName("permissionLevel")]
        [JsonInclude]
        public string? PermissionLevel { get; set; } // "none" | "read" | "comment" | "edit" | "create"

        [JsonPropertyName("name")] // optional
        [JsonInclude]
        public string? Name { get; set; }

        [JsonPropertyName("profilePicUrl")] // optional
        [JsonInclude]
        public string? ProfilePicUrl { get; set; }
    }
}
