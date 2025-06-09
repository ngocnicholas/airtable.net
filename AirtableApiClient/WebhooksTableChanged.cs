using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class WebhooksTableChanged
    {
        [JsonPropertyName("changedMetadata")]
        [JsonInclude]
        public WebhooksChangedMetadata ChangedMetadata { get; set; }

        [JsonPropertyName("createdFieldsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksField> CreatedFieldsById { get; set; }


        [JsonPropertyName("changedFieldsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksFieldChange> ChangedFieldsById { get; set; } 

        [JsonPropertyName("destroyedFieldIds")]
        [JsonInclude]
        public string[] DestroyedFieldIds { get; set; }

        [JsonPropertyName("createdRecordsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksCreatedRecord> CreatedRecordsById { get; set; } 

        [JsonPropertyName("changedRecordsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksChangedRecord> ChangedRecordsById { get; set; }

        [JsonPropertyName("destroyedRecordIds")]
        [JsonInclude]
        public string[] DestroyedRecordIds { get; set; }

        [JsonPropertyName("changedViewsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksChangedView> ChangedViewsById { get; set; }
    }

    public class WebhooksChangedMetadata
    {
        [JsonPropertyName("current")]
        [JsonInclude]
        public WebhooksMetadata Current { get; set; }

        [JsonPropertyName("previous")]
        [JsonInclude]
        public WebhooksMetadata Previous { get; set; }

    }

    public class WebhooksMetadata        // WebhooksTableCreated also uses this class.
    {
        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        [JsonInclude]
        public string Description { get; set; }
    }

    public class WebhooksField          // WebhooksTableCreated also uses this class. Note: AirtableRecord.Fields is completely different
    {
        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        [JsonInclude]
        public string Type { get; set; }
    }

    public class WebhooksFieldChange
    {
        [JsonPropertyName("current")]
        [JsonInclude]
        public WebhooksField Current { get; set; }     // Only has Name, no Type

        [JsonPropertyName("previous")]
        [JsonInclude]
        public WebhooksField Previous { get; set; }    // Only has Name, no Type
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
        public WebhooksRecordData Current { get; set; } 

        [JsonPropertyName("previous")]
        [JsonInclude] 
        public WebhooksRecordData Previous { get; set; } 

        [JsonPropertyName("unchanged")]
        [JsonInclude] 
        public WebhooksRecordData Unchanged { get; set; } 
    }
  
    public class WebhooksCreatedRecord      // Not to confuse with AirtableRecord. This class is equivalent to RecordData with a CreatedTime.
    {    
        [JsonPropertyName("cellValuesByFieldId")]
        [JsonInclude]
        public Dictionary<string, object> CellValuesByFieldId { get; set; }    // where 'object' can be anything such as a List<object> or Cell value V2 by fieldid. 

        [JsonPropertyName("createdTime")]
        [JsonInclude]
        public DateTime CreatedTime { get; set; }
    }

    public class WebhooksChangedView
    {
        [JsonPropertyName("createdRecordsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksCreatedRecord> CreatedRecordsById { get; set; }


        [JsonPropertyName("changedRecordsById")]
        [JsonInclude]
        public Dictionary<string, WebhooksChangedRecord> ChangedRecordsById { get; set; }

        [JsonPropertyName("destroyedRecordIds")]
        [JsonInclude]
        public string[] DestroyedRecordIds { get; set; }
    }

}
