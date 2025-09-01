using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class AirtableRecordList
    {
        [JsonPropertyName("offset")]
        [JsonInclude]
        public string? Offset { get; set; }

        [JsonPropertyName("records")]
        [JsonInclude]
        public AirtableRecord[]? Records { get; set; }
    }


    public class AirtableRecord
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string? Id { get; set; }

        [JsonPropertyName("createdTime")]
        [JsonInclude]
        public DateTime CreatedTime { get; set; }

        [JsonPropertyName("fields")]
        [JsonInclude]
        // <string, object> where string is the Field ID/Field Name (different than the record ID) and object is one Field value
        //public IDictionary<string, object> Fields { get; set; } = new Dictionary<string, object>();
        public IDictionary<string, object> Fields { get; set; } = new Dictionary<string, object>(); // Make set public for upSert operations 01/30/2023

        [JsonPropertyName("commentCount")]
        [JsonInclude]
        public int? CommentCount { get; set; }

#nullable enable
        public object? GetField(string fieldName)
            => Fields.ContainsKey(fieldName) ? Fields[fieldName] : null;

        public JsonElement? GetFieldAsJson(string fieldName)
            => GetField(fieldName) as JsonElement?;

        public T? GetField<T>(string fieldName)
        {
            JsonElement? jsonElement = GetFieldAsJson(fieldName);
            if (jsonElement is not JsonElement jsonElement_)
                return default;
            
            object? value = ParsePrimitiveValue(jsonElement_, typeof(T));
            if (typeof(T) == typeof(DateTimeOffset))
                value = jsonElement_.GetDateTimeOffset();
            
            return (T?)value;
        }
        
        /// <summary>
        /// Parse field as an enumerable (either an array, list, collection or enumerable).
        /// </summary>
        /// <param name="fieldName"></param>
        /// <typeparam name="TEnumerable"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public TEnumerable? GetField<TEnumerable, T>(string fieldName)
            where TEnumerable : class, IEnumerable<T>
        {
            JsonElement? jsonElement = GetFieldAsJson(fieldName);
            if (jsonElement is not JsonElement jsonElement_)
                return default;
            
            IEnumerable<T> enumerable = jsonElement_.EnumerateArray()
                .Select(_ => ParsePrimitiveValue( _, typeof(T)))
                .Cast<T>();
            
            return typeof(TEnumerable) switch
            {
                Type t when t == typeof(T[])
                    => enumerable.ToArray() as TEnumerable,
                Type t when t == typeof(IList<T>) || t == typeof(ICollection<T>)
                    => enumerable.ToList() as TEnumerable,
                Type t when t == typeof(IEnumerable<T>)
                    => (TEnumerable)enumerable,
                _ => throw new NotSupportedException($"Unknown enumerable type '{typeof(TEnumerable).Name}'"),
            };
        }
#nullable restore

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
        public IEnumerable<AirtableAttachment>? GetAttachmentField(string attachmentsFieldName)
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
                foreach (var rawAttachment in rawAttachments!)
                {
                    json = JsonSerializer.Serialize(rawAttachment);
                    attachments.Add(JsonSerializer.Deserialize<AirtableAttachment>(json)!);
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


#nullable enable
        private static object? ParsePrimitiveValue(JsonElement element, Type type)
        {
            // handle nullable types
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() != typeof(Nullable<>))
                    throw new NotSupportedException($"The only generic type supported is Nullable<T>");

                type = type.GenericTypeArguments.Single();
            }
            return Type.GetTypeCode(type) switch
            {
                TypeCode.Boolean => element.GetBoolean(),
                TypeCode.SByte => element.GetSByte(),
                TypeCode.Byte => element.GetByte(),
                TypeCode.Int16 => element.GetInt16(),
                TypeCode.UInt16 => element.GetUInt16(),
                TypeCode.Int32 => element.GetInt32(),
                TypeCode.UInt32 => element.GetUInt32(),
                TypeCode.Int64 => element.GetInt64(),
                TypeCode.UInt64 => element.GetUInt64(),
                TypeCode.Single => element.GetDecimal(),
                TypeCode.Double => element.GetDouble(),
                TypeCode.Decimal => element.GetDecimal(),
                TypeCode.DateTime => element.GetDateTime(),
                TypeCode.String => element.GetString(),
                _ => default,
            };
        }
    #nullable restore
    }
    

    public class AirtableRecordList<T>
    {
        [JsonPropertyName("offset")]
        [JsonInclude]
        public string? Offset { get; set; }

        [JsonPropertyName("records")]
        [JsonInclude]
        public AirtableRecord<T>[]? Records { get; set; }
    }


    public class AirtableRecord<T>
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string? Id { get; set; }

        [JsonPropertyName("createdTime")]
        [JsonInclude]
        public DateTime CreatedTime { get; set; }

        [JsonPropertyName("fields")]
        [JsonInclude]
        public T? Fields { get; set; }

        [JsonPropertyName("commentCount")]
        [JsonInclude]
        public int CommentCount { get; set; } 
    }


    public class AirtableUpSertRecordList : AirtableRecordList
    {
        [JsonPropertyName("createdRecords")]
        [JsonInclude]
        public string[]? CreatedRecords { get; set; }

        [JsonPropertyName("updatedRecords")]
        [JsonInclude]
        public string[]? UpdatedRecords { get; set; }
    }


    public class AirtableUpSertRecordList<T> : AirtableRecordList<T>
    {
        [JsonPropertyName("createdRecords")]
        [JsonInclude]
        public string[]? CreatedRecords { get; set; }

        [JsonPropertyName("updatedRecords")]
        [JsonInclude]
        public string[]? UpdatedRecords { get; set; }
    }
}
