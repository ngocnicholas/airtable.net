using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class AirtableRecordList
    {
        [JsonPropertyName("offset")]
        [JsonInclude]
        public string Offset { get; internal set; }

        [JsonPropertyName("records")]
        [JsonInclude]
        public AirtableRecord[] Records { get; internal set; }
    }


    public class AirtableRecord
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("createdTime")]
        [JsonInclude]
        public DateTime CreatedTime { get; internal set; }

        [JsonPropertyName("fields")]
        [JsonInclude]
        public Dictionary<string, object> Fields { get; internal set; } = new Dictionary<string, object>();

        public object
        GetField(string fieldName)
        {
            if (Fields.ContainsKey(fieldName))
            {
                return Fields[fieldName];
            }
            return null;
        }


        //----------------------------------------------------------------------------
        // 
        // AirtableRecord.GetAttachmentField
        // 
        // This method does not communicate with Airtable. 
        // It only helps the user to entangle an Attachments field given the name of the Attachments field.
        // Special care is taken to make sure the field for the input argument is actually has attachments.
        //
        // It returns a list of AirtableAttachment(s) in this field.
        // If there is no such field or there are no attachments in this field, it will return null.
        // 
        //----------------------------------------------------------------------------
        public IEnumerable<AirtableAttachment> GetAttachmentField(string attachmentsFieldName)
        {
            var attachmentField = GetField(attachmentsFieldName);
            if (attachmentField == null)
            {
                return null;
            }

            //
            // At this point, attachmentField is an array of nested objects representing the attachment list.
            // Take advantage of the serialization and deserialization of JsonConvert
            // to take care of the AirtableAttachment construction for us.
            //

            var attachments = new List<AirtableAttachment>();
            try
            {
                var json = JsonSerializer.Serialize(attachmentField);
                var rawAttachments = JsonSerializer.Deserialize<IEnumerable<Dictionary<string, object>>>(json);

                foreach (var rawAttachment in rawAttachments)
                {
                    json = JsonSerializer.Serialize(rawAttachment);
                    attachments.Add(JsonSerializer.Deserialize<AirtableAttachment>(json));
                }
            }
            catch (Exception error)
            {
                throw new ArgumentException("Field '" + attachmentsFieldName + "' is not an Attachments field." + 
                    Environment.NewLine +
                    "It has caused the exception: " +  error.Message);
            }
            return attachments;
        }
    }


    public class AirtableRecordList<T>
    {
        [JsonPropertyName("offset")]
        [JsonInclude]
        public string Offset { get; internal set; }

        [JsonPropertyName("records")]
        [JsonInclude]
        public AirtableRecord<T>[] Records { get; internal set; }
    }


    public class AirtableRecord<T>
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; internal set; }

        [JsonPropertyName("createdTime")]
        [JsonInclude]
        public DateTime CreatedTime { get; internal set; }

        [JsonPropertyName("fields")]
        [JsonInclude]
        public T Fields { get; internal set; }
    }
}
