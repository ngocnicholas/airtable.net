using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AirtableApiClient
{
    public class AirtableRecordList
    {
        [JsonProperty("offset")]
        public string Offset { get; internal set; }

        [JsonProperty("records")]
        public AirtableRecord[] Records { get; internal set; }
    }


    public class AirtableRecord
    {
        [JsonProperty("id")]
        public string Id { get; internal set; }

        [JsonProperty("createdTime")]
        public DateTime CreatedTime { get; internal set; }

        [JsonProperty("fields")]
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
                var json = JsonConvert.SerializeObject(attachmentField);
                var rawAttachments = JsonConvert.DeserializeObject<IEnumerable<Dictionary<string, object>>>(json);

                foreach (var rawAttachment in rawAttachments)
                {
                    json = JsonConvert.SerializeObject(rawAttachment);
                    attachments.Add(JsonConvert.DeserializeObject<AirtableAttachment>(json));
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
}
