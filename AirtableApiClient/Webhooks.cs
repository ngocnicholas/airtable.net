using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class Webhooks
    {
        [JsonPropertyName("webhooks")]
        [JsonInclude]
        public Webhook[] Hooks { get; internal set; }
    }
    public class Webhook
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("areNotificationsEnabled")]
        [JsonInclude]
        public bool AreNotificationsEnabled { get; internal set; }

        [JsonPropertyName("cursorForNextPayload")]
        [JsonInclude]
        public int CursorForNextPayload { get; internal set; }

        [JsonPropertyName("isHookEnabled")]
        [JsonInclude]
        public bool IsHookEnabled { get; internal set; }

        [JsonPropertyName("lastSuccessfulNotificationTime")]
        [JsonInclude]
        public string LastSuccessfulNotificationTime { get; internal set; }

        [JsonPropertyName("notificationUrl")]
        [JsonInclude]
        public string NotificationUrl { get; internal set; }

        [JsonPropertyName("expirationTime")]
        [JsonInclude]
        public string ExpirationTime { get; internal set; }

        [JsonPropertyName("lastNotificationResult")]
        [JsonInclude]
        public WebhooksNotification LastNotificationResult { get; internal set; }

        [JsonPropertyName("specification")]
        [JsonInclude]
        public WebhooksSpecification Specification { get; internal set; }
    }

    //-------------------------------- Webhooks Specification ----------------------------------------
    public class WebhooksSpecification
	{
        [JsonPropertyName("options")]
        [JsonInclude]
        public Options Options { get; set; }
    }


    public class Options
    {
        [JsonPropertyName("filters")]
        [JsonInclude]
        public Filters Filters { get; set; }

        [JsonPropertyName("includes")]
        [JsonInclude]
        public Includes Includes { get; set; }

    }

    public class Filters
    {
        [JsonPropertyName("recordChangeScope")]
        [JsonInclude]
        public string RecordChangeScope { get; set; }


        [JsonPropertyName("dataTypes")]
        [JsonInclude]
        public string[] DataTypes { get; set; } // array of ("tableData" | "tableFields" | "tableMetadata")

        [JsonPropertyName("changeTypes")]
        [JsonInclude]
        public string[] ChangeTypes { get; set; }   // optional<array of ("add" | "remove" | "update")>

        [JsonPropertyName("fromSources")]
        [JsonInclude]
        public string[] FromSources { get; set; }   // optional<array of ("client" | "publicApi" | "formSubmission" | "automation" | "system" | "sync" | "anonymousUser" | "unknown")>

        [JsonPropertyName("sourceOptions")]
        [JsonInclude]
        public SourceOptions SourceOptions { get; set; }    // optional<object>

        [JsonPropertyName("watchDataInFieldIds")]   
        [JsonInclude]
        public string[] WatchDataInFieldIds { get; set; }   // optional<array of strings>

        [JsonPropertyName("watchSchemasOfFieldIds")]
        [JsonInclude]
        public string[] WatchSchemasOfFieldIds { get; set; }    // optional<array of strings>

    }

    public class SourceOptions
    {
        [JsonPropertyName("formSubmission")]
        [JsonInclude]
        public FormSubmission FormSubmission { get; set; }
    }

    public class FormSubmission
    {
        [JsonPropertyName("viewId")]
        [JsonInclude]
        public string ViewId { get; set; }
    }

    public class Includes
    {
        [JsonPropertyName("includeCellValuesInFieldIds")]
        [JsonInclude]
        public string[] IncludeCellValuesInFieldIds { get; set; }   // optional<array of strings | "all">

        [JsonPropertyName("includePreviousCellValues")]
        [JsonInclude]
        public bool IncludePreviousCellValues { get; set; }         // If true, include the previous cell value in the payload.

        [JsonPropertyName("includePreviousFieldDefinitions")]
        [JsonInclude]
        public bool IncludePreviousFieldDefinitions { get; set; }   // If true, include the previous field definition in the payload.
    }

    public class CreateWebhookResponse
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("macSecretBase64")]
        [JsonInclude] 
        public string MacSecretBase64 {  get; internal set;} 

        [JsonPropertyName("expirationTime")]
        [JsonInclude]
        public string ExpirationTime {  get; internal set; }
    }


    public class NotificationExpirationTime
    {
        [JsonPropertyName("expirationTime")]
        [JsonInclude]
        public string ExpirationTime { get; internal set; }
    }
}
 