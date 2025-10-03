using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public enum FieldType
    {
        SingleLineText,
        Email, 
        Url, 
        MultilineText, 
        Number,
        Percent, 
        Currency, 
        SingleSelect, 
        MultipleSelects, 
        SingleCollaborator,
        MultipleCollaborators, 
        MultipleRecordLinks, 
        Date, 
        DateTime, 
        PhoneNumber,
        MultipleAttachments, 
        Checkbox, 
        Formula, 
        CreatedTime, 
        Rollup,
        Count, 
        MultipleLookupValues, 
        AutoNumber, 
        Barcode,
        Rating, 
        RichText, 
        Duration, 
        LastModifiedTime, 
        Button,
        CreatedBy, 
        LastModifiedBy, 
        ExternalSyncSource, 
        AiText,
        UnknownField
    }


    // ----------------- Extension methods to convert enums to API strings ------------------------
    public static class EnumExtensions
    {
        public static string ToApiString(this FieldType field)
        {
            return field switch
            {
                FieldType.SingleLineText => "singleLineText",
                FieldType.Email => "email",
                FieldType.Url => "url",
                FieldType.MultilineText => "multilineText",
                FieldType.Number => "number",
                FieldType.Percent => "percent",
                FieldType.Currency => "currency",
                FieldType.SingleSelect => "singleSelect",
                FieldType.MultipleSelects => "multipleSelects",
                FieldType.SingleCollaborator => "singleCollaborator",
                FieldType.MultipleCollaborators => "multipleCollaborators",
                FieldType.MultipleRecordLinks => "multipleRecordLinks",
                FieldType.Date => "date",
                FieldType.DateTime => "dateTime",
                FieldType.PhoneNumber => "phoneNumber",
                FieldType.MultipleAttachments => "multipleAttachments",
                FieldType.Checkbox => "checkbox",
                FieldType.Formula => "formula",
                FieldType.CreatedTime => "createdTime",
                FieldType.Rollup => "rollup",
                FieldType.Count => "count",
                FieldType.MultipleLookupValues => "multipleLookupValues",
                FieldType.AutoNumber => "autoNumber",
                FieldType.Barcode => "barcode",
                FieldType.Rating => "rating",
                FieldType.RichText => "richText",
                FieldType.Duration => "duration",
                FieldType.LastModifiedTime => "lastModifiedTime",
                FieldType.Button => "button",
                FieldType.CreatedBy => "createdBy",
                FieldType.LastModifiedBy => "lastModifiedBy",
                FieldType.ExternalSyncSource => "externalSyncSource",
                FieldType.AiText => "aiText",
                _ => throw new ArgumentOutOfRangeException(nameof(field), field, "Unsupported FieldType")
            };
        }

    }
    //-----------------------------------------------------------------------------------------------------------

    // Generic enum converter that uses ToApiString() extension methods
    public class ApiStringEnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException($"Expected string, got {reader.TokenType}");

            string? value = reader.GetString();
            if (string.IsNullOrEmpty(value))
                throw new JsonException("Enum value cannot be null or empty");

#if NET5_0_OR_GREATER
            // Try to find matching enum value by comparing API strings
            foreach (T enumValue in Enum.GetValues<T>())
#else
        foreach (T enumValue in (T[])Enum.GetValues(typeof(T)))
#endif
            {
                if (GetApiString(enumValue).Equals(value, StringComparison.OrdinalIgnoreCase))
                    return enumValue;
            }

            throw new JsonException($"Unable to convert '{value}' to {typeof(T).Name}");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(GetApiString(value));
        }

        private static string GetApiString(T enumValue)
        {
            // Use reflection to call the ToApiString() extension method
            var method = typeof(EnumExtensions).GetMethod("ToApiString", new[] { typeof(T) });
            if (method != null)
            {
                return (string)method.Invoke(null, new object[] { enumValue })!;
            }

            // Fallback to enum name if no ToApiString method exists
            return enumValue.ToString();
        }
    }

    // Specific converters for each enum type
    public class FieldTypeConverter : ApiStringEnumConverter<FieldType> { }

    }
