using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class FieldConfig
    {
        [JsonPropertyName("type")]
        [JsonInclude]
        public string Type { get; set; }       // optional<Field type> https://airtable.com/developers/web/api/model/field-type
                                               // Should this be an eum or a string?
        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; set; }

        [JsonPropertyName("options")]
        [JsonInclude]
        public object Options { get; set; }  // Don't make this property virtual because System.Text.Json does not handle serialization well for polymorphism

    }


    public abstract class FieldConfigOptions<TOptions> : FieldConfig
    {
        [JsonIgnore] // Always ignore during serialization
        public TOptions TypedOptions
        {
            get
            {
                if (Options is JsonElement jsonElement)
                {
                    Options = jsonElement.Deserialize<TOptions>();
                }

                return (TOptions)Options!;
            }
            set => Options = value!;
        }
    }


    /// <summary>
    /// Attachment field                                        
    /// </summary>
    public class AttachmentFieldConfig : FieldConfig 
    {
        public AttachmentFieldConfig()
        {
            Type = "multipleAttachments";
        }
    }


    /// <summary>
    /// Link to another record field
    /// </summary>
    public class LinkToAnotherRecordFieldConfig : FieldConfigOptions<LinkToAnotherRecordFieldConfigOptions>   // for Read and for Write
    {
        public LinkToAnotherRecordFieldConfig()
        {
            Type = "multipleRecordLinks";
        }
    }

    public class LinkToAnotherRecordFieldConfigOptions                    // Can only be tested when we do UPDATE of a base
    {
        // Note: Only the 2 properties below are in the WRITE situation.

        [JsonPropertyName("linkedTableId")]
        public string LinkedTableId { get; set; }   // The ID of the table this field links to

        [JsonPropertyName("viewIdForRecordSelection")]
        public string ViewIdForRecordSelection { get; set; } // Optional. The ID of the view in the linked table to use when showing a list of records to select from.
    }

}

