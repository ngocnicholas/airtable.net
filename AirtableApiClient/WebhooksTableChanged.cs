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
        public Dictionary<string, Field> CreatedFieldsById { get; internal set; }    // NameType is a Field?


        [JsonPropertyName("changedFieldsById")]
        [JsonInclude]
        public Dictionary<string, FieldChange> ChangedFieldsById { get; internal set; }  // // NameTypeChange is a Field?

        [JsonPropertyName("destroyedFieldIds")]
        [JsonInclude]
        public string[] DestroyedFieldIds { get; internal set; }

        [JsonPropertyName("createdRecordsById")]
        [JsonInclude]
        public Dictionary<string, Dictionary<string, CreatedRecord>> CreatedRecordsById { get; internal set; }    // Ex: where string is "rec00000000000000"
                                                                                                                    // CreatedRecord is a Record?

        [JsonPropertyName("changedRecordsById")]
        [JsonInclude]
        public Dictionary<string, ChangedRecord> ChangedRecordsById { get; internal set; }  // ChangedRecord is  RecordData?

        [JsonPropertyName("destroyedRecordIds")]
        [JsonInclude]
        public string[] DestroyedRecordIds { get; internal set; }

        [JsonPropertyName("changedViewsById")]
        [JsonInclude]
        public Dictionary<string, ChangedView> ChangedViewsById { get; set; }
    }

    public class ChangedMetadata
    {
        public Metadata Current { get; set; }
        public Metadata Previous { get; set; }

    }

    public class Metadata        // WebhooksTableCreated also uses this class. 
    {
        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name;

        [JsonPropertyName("description")]
        [JsonInclude]
        public string Description;
    }

    public class Field          // WebhooksTableCreated also uses this class. Note: AirtableRecord.Fields is completely different
    {
        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        [JsonInclude]
        public string Type { get; set; }
    }

    public class FieldChange    
    {
        [JsonPropertyName("current")]
        [JsonInclude]
        public Field Current { get; set; }

        [JsonPropertyName("previous")]
        [JsonInclude]
        public Field Previous { get; set; }
    }

    public class RecordData
    {
        [JsonPropertyName("cellValuesByFieldId")]
        [JsonInclude]
        public Dictionary<string, object> CellValuesByFieldId { get; set; } // where 'object' is Cell vakye V2 bt fieldId
    }

    public class ChangedRecord
    {
        [JsonPropertyName("current")]
        [JsonInclude] 
        public RecordData Current { get; set; }    // "fld00000000000001": "hello world"

        [JsonPropertyName("previous")]
        [JsonInclude] 
        public RecordData Previous { get; set; }   // "fld0000000000001": "hello"

        [JsonPropertyName("unchanged")]
        [JsonInclude] 
        public RecordData Unchanged { get; set; }  // "fld0000000000000": 1
    }

    
    public class CreatedRecord             // used in a Dictionary
    {
        [JsonPropertyName("cellValuesByFieldId")]
        [JsonInclude]
        public Dictionary<string, object> CellValuesByFieldId { get; set; }    // where string is "fld0000000000000"; int is the const 0

        [JsonPropertyName("createdTime")]
        [JsonInclude]
        public string CreatedTime;
    }

    public class ChangedView
    {
        public Dictionary<string, CreatedRecord> CreatedRecordsById { get; set; }
        public Dictionary<string, ChangedRecord> ChangedRecordsById { get; set; }
        public string[] DestroyedRecordIds { get; set; }
    }

}

