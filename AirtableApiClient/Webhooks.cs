using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class Webhook
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("areNotificationsEnabled")]
        [JsonInclude]
        public bool AreNotificationsEnabled { get; set; }

        [JsonPropertyName("cursorForNextPayload")]
        [JsonInclude]
        public int CursorForNextPayload { get; internal set; }

        [JsonPropertyName("isHookEnabled")]
        [JsonInclude]
        public bool IsHookEnabled { get; set; }

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
        public string[] DataTypes { get; set; }

        [JsonPropertyName("changeTypes")]
        [JsonInclude]
        public string[] ChangeTypes { get; } 

        [JsonPropertyName("fromSources")]
        [JsonInclude]
        public string[] FromSources { get; set; }

        [JsonPropertyName("sourceOptions")]
        [JsonInclude]
        public SourceOptions SourceOptions { get; set; }

        [JsonPropertyName("watchDataInFieldIds")]
        [JsonInclude]
        public string[] WatchDataInFieldIds { get; set; }

        [JsonPropertyName("watchSchemasOfFieldIds")]
        [JsonInclude]
        public string[] WatchSchemasOfFieldIds { get; set; }

    }

    /*public enum DataTypes
    {
        tableData,
        tableFields,
        tableMetadata

    public enum ChangeTypes
    {
        add,
        remove,
        update
    }

    public enum FromSources
    {
        client,
        publicApi,
        formSubmission,
        automation,
        system,
        sync,
        anonymousUser,
        unknown
    }
    } */

    public class SourceOptions
    {
        [JsonPropertyName("formSubmission")]
        [JsonInclude]
        public FormSubmission FormSubmission;
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

        [JsonPropertyName("icludePreviousFieldDefinitions")]
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

    public class Webhooks
    {
        [JsonPropertyName("webhooks")]
        [JsonInclude]
        public Webhook[] Hooks { get; set; }
    }

    public class NotificationExpirationTime
    {
        [JsonPropertyName("expirationTime")]
        [JsonInclude]
        public string ExpirationTime { get; internal set; }

    }

    public class WebhooksNotification
    {
        [JsonPropertyName("success")]
        [JsonInclude]
        public bool Success { get; internal set; }

        [JsonPropertyName("completionTimestamp")]
        [JsonInclude]
        public string completionTimestamp { get; set; }

        [JsonPropertyName("durationMs")]
        [JsonInclude]
        public int durationMs { get; set; }

        [JsonPropertyName("retryNumber")]
        [JsonInclude]
        public int RetryNumber { get; set; }
    }

    public class WebhooksNotificationError : WebhooksNotification
    {
        [JsonPropertyName("error")]
        [JsonInclude]
        public Error Error { get; set; }

        [JsonPropertyName("willBeRetried")]
        [JsonInclude]
        public bool WillBeRetried { get; set; }
    }

    public class Error
    {
        [JsonPropertyName("message")]
        [JsonInclude]
        public string Message { get; internal set; }
    }


    //---------------------------------------------------------------------------------------------------------------
    public class Payload
    {
        [JsonPropertyName("timeStamp")]
        [JsonInclude]
        public string TimeStamp { get; internal set; }

        [JsonPropertyName("baseTransactionNumber")]
        [JsonInclude]
        public int BaseTransactionNumber { get; internal set; }

        [JsonPropertyName("payloadFormat")]
        [JsonInclude]
        public string PayloadFormat { get; internal set; }

        [JsonPropertyName("actionMetadata")]
        [JsonInclude]
        public WebhooksAction ActionMetadata { get; internal set; }
    }

    public class ErrorPayload
    {
        [JsonPropertyName("error")]
        [JsonInclude]
        public bool Error { get; internal set; }

        [JsonPropertyName("code")]
        [JsonInclude]
        public string[] Code { get; internal set; }

        [JsonPropertyName("timeStamp")]
        [JsonInclude]
        public string TimeStamp { get; internal set; }

        [JsonPropertyName("baseTransactionNumber")]
        [JsonInclude]
        public int BaseTransactionNumber { get; internal set; }

        [JsonPropertyName("payloadFormat")]
        [JsonInclude]
        public string PayloadFormat { get; internal set; }  // "v0"

        [JsonPropertyName("actionMetadata")]
        [JsonInclude]
        public WebhooksAction ActionMetadata { get; internal set; }

        [JsonPropertyName("changedTablesById")]
        [JsonInclude]
        public WebhooksTableChanged ChangedTablesById { get; set; }

        [JsonPropertyName("createdTablesById")]
        [JsonInclude]
        public WebhooksTableCreated CreatedTablesById { get; set; }

        [JsonPropertyName("destroyedTableIds")]
        [JsonInclude]
        public string[] DestroyedTableIds { get; set; }
    }

 /*   public enum Code
    {
        INVALID_FILTERS,
        INVALID_HOOK
    } */

    //---------------------------------- Webhooks Table Changed ------------------
    public class WebhooksTableChanged
    {
        [JsonPropertyName("changedMetadata")]
        [JsonInclude]
        public ChangedMetadata ChangedMetadata { get; internal set; }

        [JsonPropertyName("createdFieldsById")]
        [JsonInclude]
        public Dictionary<string, NameType> CreatedFieldsById { get; internal set; }


        [JsonPropertyName("changedFieldsById")]
        [JsonInclude]
        public Dictionary<string, NameTypeChange> ChangedFieldsById { get; internal set; }

        [JsonPropertyName("destroyedFieldIds")]
        [JsonInclude]
        public string[] DestroyedFieldIds { get; internal set; }

        [JsonPropertyName("createdRecordsById")]
        [JsonInclude]
        public Dictionary<string, Dictionary<string, CreatedRecord>> CreatedRecordsById { get; internal set; }    // where string is "rec00000000000000"

        [JsonPropertyName("changedRecordsById")]
        [JsonInclude]
        public Dictionary<string, RecordChange> ChangedRecordsById { get; internal set; }

        [JsonPropertyName("destroyedRecordIds")]
        [JsonInclude]
        public string[] DestroyedRecordIds { get; internal set; }

        [JsonPropertyName("changedViewsById")]
        [JsonInclude]
        public Dictionary<string, ChangedView> ChangedViewsById { get; set; }
    }

    public class ChangedMetadata
    {
        public NameDescription Current { get; set; }
        public NameDescription Previous { get; set; }

    }

    public class NameDescription
    {
        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name;

        [JsonPropertyName("description")]
        [JsonInclude]
        public string Description;
    }

    public class NameType
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class NameTypeChange
    {
        public NameType Current { get; set; }
        public NameType Previous { get; set; }
    }


    public class CellValuesByFieldId
    {
        public Dictionary<string, object> FieldValues { get; set; }
    }

    public class RecordChange
    {
        /*
"               cellValuesByFieldId": {
                 "fld00000000000001": "hello world" // Different ones => represented by an 'object' type in Dictionary<string, object> FieldValues;
                }
        */
        public CellValuesByFieldId Current { get; set; }    // "fld00000000000001": "hello world"

        public CellValuesByFieldId Previous { get; set; }   // "fld0000000000001": "hello"
        public CellValuesByFieldId Unchanged { get; set; }  // "fld0000000000000": 1
    }

    public class CreatedRecord             // used in a Dictionary
    {
        [JsonPropertyName("cellValuesByFieldId")]
        [JsonInclude]
        public CellValuesByFieldId CellValuesByFieldId;    // where string is "fld0000000000000"; int is the const 0

        [JsonPropertyName("createdTime")]
        [JsonInclude]
        public string CreatedTime;
    }

    public class ChangedView
    {
        public Dictionary<string, CreatedRecord> CreatedRecordsById { get; set; }
        public Dictionary<string, RecordChange> ChangedRecordsById { get; set; }
        public string[] DestroyedRecordIds { get; set; }
    }

    //---------------------- WebhooksTableCreated -----------------------------
    public class WebhooksTableCreated
    {
        [JsonPropertyName("fieldsById")]
        [JsonInclude]
        public Dictionary<string, FieldsById> FieldsById {  get; set; }

        [JsonPropertyName("recordsById")]
        [JsonInclude]
        public Dictionary<string, RecordsById> RecordsById {  get; set; }

        [JsonPropertyName("metaData")]
        [JsonInclude]
        public MetaData MetaData { get; set; }

    }

    public class MetaData
    {
        [JsonPropertyName("description")]
        [JsonInclude]
        public string Description { get; set; } // string | null

        [JsonPropertyName("name")]
        [JsonInclude] 
        public string Name { get; set; }
    }

    public class FieldsById  {
        public string[] Type { get; set; }
        string Name { get; set; }
    }

    public class RecordsById
    {
        [JsonPropertyName("createdTime")]
        [JsonInclude]
        public string CreatedTime;

        [JsonPropertyName("cellValuesByFieldId")]
        [JsonInclude]
        public Dictionary<string, string> cellValuesByFieldId; // NGOC: the cell value should be Cell Value V2 by fieldId???
    }
 /*   public enum FieldType
    {
        singleLineText,
        email,
        url,
        multilineText,
        number,
        percent,
        currency
        //singleSelect" | "multipleSelects" | "singleCollaborator" | "multipleCollaborators" | "multipleRecordLinks" | "date" | "dateTime" | "phoneNumber" | "multipleAttachments" | "checkbox" | "formula" | "createdTime" | "rollup" | "count" | "lookup" | "multipleLookupValues" | "autoNumber" | "barcode" | "rating" | "richText" | "duration" | "lastModifiedTime" | "button" | "createdBy" | "lastModifiedBy" | "externalSyncSource" | "aiText"
    } */


    //-----------------------------------------------------------------------------------------------------
    public class PayloadList
    {
        [JsonPropertyName("payloads")]
        [JsonInclude]
        public Payload[] Payloads;

        [JsonPropertyName("cursor")]
        [JsonInclude]
        public int Cursor;

        [JsonPropertyName("mightHaveMore")]
        [JsonInclude]
        public bool MightHaveMore;
    }

    //-----------------------------------------------------------------------------------------------------
    public class WebhooksAction
    {
        public WebhooksAction(string source)
        {
            Source = source;
        }

        [JsonPropertyName("source")]
        [JsonInclude]
        public string Source { get; internal set; }
    }

    public class WebhooksActionClient : WebhooksAction
    {
        public WebhooksActionClient() : base("client") { /* Empty body */  }

        [JsonPropertyName("sourceMetaData")]
        [JsonInclude]
        public SourceMetaData SourceMetaData { get; set; }
    }

    public class WebhooksActionPublicApi : WebhooksAction
    {
        public WebhooksActionPublicApi() : base("publicApi") { /* Empty body */  }

        [JsonPropertyName("sourceMetaData")]
        [JsonInclude]
        public SourceMetaData SourceMetaData { get; set; }
    }

    public class WebhooksActionFormSubmission : WebhooksAction
    {
        public WebhooksActionFormSubmission(): base ("formSubmission") { /* Empty body */ }
        public SourceMetaData SourceMetaData { get; set; }
    }

    public class WebhooksActionautomation : WebhooksAction
    {
        public WebhooksActionautomation() : base("automation") { /* Empty body */ }
        public SourceMetaData SourceMetaData { get; set; }
    }

    public class WebhooksActionSystem : WebhooksAction
    {
        public WebhooksActionSystem() : base("system") { /* Empty body */ }
    }

    public class WebhooksActionSync : WebhooksAction
    {
        public WebhooksActionSync() : base("sync") { /* Empty body */ }
    }

    public class WebhooksActionAnonymousUser : WebhooksAction
    {
        public WebhooksActionAnonymousUser() : base("anonymousUser") { /* Empty body */ }
    }

    public class SourceMetaData
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
        public string[] PermissionLevel { get; internal set; } // "none" | "read" | "comment" | "edit" | "create"

        [JsonPropertyName("name")] // optional
        [JsonInclude]
        public string Name { get; internal set; }   

        [JsonPropertyName("profilePicUrl")] // optional
        [JsonInclude]
        public string ProfilePicUrl { get; internal set; }
    }

}
