using System;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class Webhooks
    {
        [JsonPropertyName("webhooks")]
        [JsonInclude]
        public Webhook[] Hooks { get; set; }
    }
    public class Webhook
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; set; }

        [JsonPropertyName("areNotificationsEnabled")]
        [JsonInclude]
        public bool AreNotificationsEnabled { get; set; }

        [JsonPropertyName("cursorForNextPayload")]
        [JsonInclude]
        public int CursorForNextPayload { get; set; }

        [JsonPropertyName("isHookEnabled")]
        [JsonInclude]
        public bool IsHookEnabled { get; set; }

        [JsonPropertyName("lastSuccessfulNotificationTime")]
        [JsonInclude]
        public DateTime? LastSuccessfulNotificationTime { get; set; }

        [JsonPropertyName("notificationUrl")]
        [JsonInclude]
        public string NotificationUrl { get; set; }

        [JsonPropertyName("expirationTime")]
        [JsonInclude]
        public DateTime? ExpirationTime { get; set; }
        //The time when the webhook expires and is disabled in the ISO format.The webhook will not expire if this is null (in the case User API keys are used)

        [JsonPropertyName("lastNotificationResult")]
        [JsonInclude]
        public WebhooksNotification LastNotificationResult { get; set; }

        [JsonPropertyName("specification")]
        [JsonInclude]
        public WebhooksSpecification Specification { get; set; }
    }

    //-------------------------------- Webhooks Specification ----------------------------------------
    public class WebhooksSpecification
	{
        [JsonPropertyName("options")]
        [JsonInclude]
        public WebhooksOptions Options { get; set; }
    }


    public class WebhooksOptions
    {
        [JsonPropertyName("filters")]
        [JsonInclude]
        public WebhooksFilters Filters { get; set; }

        [JsonPropertyName("includes")]
        [JsonInclude]
        public WebhooksIncludes Includes { get; set; }

    }

    public class WebhooksFilters
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

    public class WebhooksIncludes
    {
        [JsonPropertyName("includeCellValuesInFieldIds")]
        [JsonInclude]
        public object IncludeCellValuesInFieldIds { get; set; }     // optional<array of strings | "all">

        [JsonPropertyName("includePreviousCellValues")]
        [JsonInclude]
        public bool IncludePreviousCellValues { get; set; }         // If true, include the previous cell value in the payload.

        [JsonPropertyName("includePreviousFieldDefinitions")]
        [JsonInclude]
        public bool IncludePreviousFieldDefinitions { get; set; }   // If true, include the previous field definition in the payload.
    }

    public class CreateWebhookResponse  // Used in AirtableCreaateWebhookResponse
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; set; }

        [JsonPropertyName("macSecretBase64")]
        [JsonInclude] 
        public string MacSecretBase64 {  get; set;} 

        [JsonPropertyName("expirationTime")]
        [JsonInclude]
        public DateTime? ExpirationTime {  get; set; }
    }


    public class NotificationExpirationTime // Used in RefreshWebhook
    {
        [JsonPropertyName("expirationTime")]
        [JsonInclude]
        public DateTime? ExpirationTime { get; set; }
    }
}
 