using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class WebhooksTableChanged
    {
        [JsonPropertyName("changedMetadata")]
        [JsonInclude]
        public ChangedMetadata ChangedMetadata { get; internal set; }

        [JsonPropertyName("createdFieldsById")]
        [JsonInclude]
        public Dictionary<string, Field> CreatedFieldsById { get; internal set; }


        [JsonPropertyName("changedFieldsById")]
        [JsonInclude]
        public Dictionary<string, FieldChange> ChangedFieldsById { get; internal set; } 

        [JsonPropertyName("destroyedFieldIds")]
        [JsonInclude]
        public string[] DestroyedFieldIds { get; internal set; }


        // optional <Webhooks Created Record> which is One or multiple records being created and reported upon via webhooks.
        // Each record is key with a string and contains 'createdTime' and 'cellValuesByFieldId'

        [JsonPropertyName("createdRecordsById")]
        [JsonInclude]
        public Dictionary<string, CreatedRecord> CreatedRecordsById { get; internal set; }    // Ex: where string is "rec00000000000000"


        [JsonPropertyName("changedRecordsById")]
        [JsonInclude]
        public Dictionary<string, ChangedRecord> ChangedRecordsById { get; internal set; }

        [JsonPropertyName("destroyedRecordIds")]
        [JsonInclude]
        public string[] DestroyedRecordIds { get; internal set; }

        [JsonPropertyName("changedViewsById")]
        [JsonInclude]
        public Dictionary<string, ChangedView> ChangedViewsById { get; internal set; }
    }

    public class ChangedMetadata
    {
        [JsonPropertyName("current")]
        [JsonInclude]
        public Metadata Current { get; internal set; }

        [JsonPropertyName("previous")]
        [JsonInclude]
        public Metadata Previous { get; internal set; }

    }

    public class Metadata        // WebhooksTableCreated also uses this class.
    {
        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; internal set; }

        [JsonPropertyName("description")]
        [JsonInclude]
        public string Description { get; internal set; }
    }

    public class Field          // WebhooksTableCreated also uses this class. Note: AirtableRecord.Fields is completely different
    {
        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; internal set; }

        [JsonPropertyName("type")]
        [JsonInclude]
        public string Type { get; internal set; }
    }

    public class FieldChange    
    {
        [JsonPropertyName("current")]
        [JsonInclude]
        public Field Current { get; internal set; }     // Only has Name, no Type

        [JsonPropertyName("previous")]
        [JsonInclude]
        public Field Previous { get; internal set; }    // Only has Name, no Type
    }

    public class RecordData
    {
        [JsonPropertyName("cellValuesByFieldId")]
        [JsonInclude]
        public Dictionary<string, object> CellValuesByFieldId { get; set; } // where 'object' can be anything such as a List<object> or Cell value V2 by fieldid.
    }

    public class ChangedRecord
    {
        [JsonPropertyName("current")]
        [JsonInclude] 
        public RecordData Current { get; internal set; }    // "fld00000000000001": "hello world"

        [JsonPropertyName("previous")]
        [JsonInclude] 
        public RecordData Previous { get; internal set; }   // "fld0000000000001": "hello"

        [JsonPropertyName("unchanged")]
        [JsonInclude] 
        public RecordData Unchanged { get; internal set; }  // "fld0000000000000": 1
    }
  
    public class CreatedRecord      // Not to confuse with AirtableRecord. This class is equivalent to RecordData with a CreatedTime.
    {    
        [JsonPropertyName("cellValuesByFieldId")]
        [JsonInclude]
        public Dictionary<string, object> CellValuesByFieldId { get; internal set; }    // where 'object' can be anything such as a List<object> or Cell value V2 by fieldid. 

        [JsonPropertyName("createdTime")]
        [JsonInclude]
        public string CreatedTime { get; internal set; }
    }

    public class ChangedView
    {
        [JsonPropertyName("createdRecordsById")]
        [JsonInclude]
        public Dictionary<string, CreatedRecord> CreatedRecordsById { get; internal set; }    // Ex: where string is "rec00000000000000"


        [JsonPropertyName("changedRecordsById")]
        [JsonInclude]
        public Dictionary<string, ChangedRecord> ChangedRecordsById { get; internal set; }

        [JsonPropertyName("destroyedRecordIds")]
        [JsonInclude]
        public string[] DestroyedRecordIds { get; internal set; }
    }

}
