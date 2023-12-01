using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class WebhooksTableChanged
    {
        [JsonPropertyName("changedMetadata")]
        [JsonInclude]
        public WebhooksChangedMetadata ChangedMetadata { get; internal set; }

        [JsonPropertyName("createdFieldsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksField> CreatedFieldsById { get; internal set; }


        [JsonPropertyName("changedFieldsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksFieldChange> ChangedFieldsById { get; internal set; } 

        [JsonPropertyName("destroyedFieldIds")]
        [JsonInclude]
        public string[] DestroyedFieldIds { get; internal set; }

        [JsonPropertyName("createdRecordsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksCreatedRecord> CreatedRecordsById { get; internal set; } 

        [JsonPropertyName("changedRecordsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksChangedRecord> ChangedRecordsById { get; internal set; }

        [JsonPropertyName("destroyedRecordIds")]
        [JsonInclude]
        public string[] DestroyedRecordIds { get; internal set; }

        [JsonPropertyName("changedViewsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksChangedView> ChangedViewsById { get; internal set; }
    }

    public class WebhooksChangedMetadata
    {
        [JsonPropertyName("current")]
        [JsonInclude]
        public WebhooksMetadata Current { get; internal set; }

        [JsonPropertyName("previous")]
        [JsonInclude]
        public WebhooksMetadata Previous { get; internal set; }

    }

    public class WebhooksMetadata        // WebhooksTableCreated also uses this class.
    {
        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; internal set; }

        [JsonPropertyName("description")]
        [JsonInclude]
        public string Description { get; internal set; }
    }

    public class WebhooksField          // WebhooksTableCreated also uses this class. Note: AirtableRecord.Fields is completely different
    {
        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; internal set; }

        [JsonPropertyName("type")]
        [JsonInclude]
        public string Type { get; internal set; }
    }

    public class WebhooksFieldChange
    {
        [JsonPropertyName("current")]
        [JsonInclude]
        public WebhooksField Current { get; internal set; }     // Only has Name, no Type

        [JsonPropertyName("previous")]
        [JsonInclude]
        public WebhooksField Previous { get; internal set; }    // Only has Name, no Type
    }

    public class WebhooksRecordData
    {
        [JsonPropertyName("cellValuesByFieldId")]
        [JsonInclude]
        public Dictionary<string, object> CellValuesByFieldId { get; set; } // where 'object' can be anything such as a List<object> or Cell value V2 by fieldid.
    }

    public class WebhooksChangedRecord
    {
        [JsonPropertyName("current")]
        [JsonInclude] 
        public WebhooksRecordData Current { get; internal set; } 

        [JsonPropertyName("previous")]
        [JsonInclude] 
        public WebhooksRecordData Previous { get; internal set; } 

        [JsonPropertyName("unchanged")]
        [JsonInclude] 
        public WebhooksRecordData Unchanged { get; internal set; } 
    }
  
    public class WebhooksCreatedRecord      // Not to confuse with AirtableRecord. This class is equivalent to RecordData with a CreatedTime.
    {    
        [JsonPropertyName("cellValuesByFieldId")]
        [JsonInclude]
        public Dictionary<string, object> CellValuesByFieldId { get; internal set; }    // where 'object' can be anything such as a List<object> or Cell value V2 by fieldid. 

        [JsonPropertyName("createdTime")]
        [JsonInclude]
        public DateTime CreatedTime { get; internal set; }
    }

    public class WebhooksChangedView
    {
        [JsonPropertyName("createdRecordsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksCreatedRecord> CreatedRecordsById { get; internal set; }


        [JsonPropertyName("changedRecordsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksChangedRecord> ChangedRecordsById { get; internal set; }

        [JsonPropertyName("destroyedRecordIds")]
        [JsonInclude]
        public string[] DestroyedRecordIds { get; internal set; }
    }

}
