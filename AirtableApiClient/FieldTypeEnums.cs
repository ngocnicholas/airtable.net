using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public enum FieldTypeEnum
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
        public static string ToApiString(this FieldTypeEnum field)
        {
            return field switch
            {
                FieldTypeEnum.SingleLineText => "singleLineText",
                FieldTypeEnum.Email => "email",
                FieldTypeEnum.Url => "url",
                FieldTypeEnum.MultilineText => "multilineText",
                FieldTypeEnum.Number => "number",
                FieldTypeEnum.Percent => "percent",
                FieldTypeEnum.Currency => "currency",
                FieldTypeEnum.SingleSelect => "singleSelect",
                FieldTypeEnum.MultipleSelects => "multipleSelects",
                FieldTypeEnum.SingleCollaborator => "singleCollaborator",
                FieldTypeEnum.MultipleCollaborators => "multipleCollaborators",
                FieldTypeEnum.MultipleRecordLinks => "multipleRecordLinks",
                FieldTypeEnum.Date => "date",
                FieldTypeEnum.DateTime => "dateTime",
                FieldTypeEnum.PhoneNumber => "phoneNumber",
                FieldTypeEnum.MultipleAttachments => "multipleAttachments",
                FieldTypeEnum.Checkbox => "checkbox",
                FieldTypeEnum.Formula => "formula",
                FieldTypeEnum.CreatedTime => "createdTime",
                FieldTypeEnum.Rollup => "rollup",
                FieldTypeEnum.Count => "count",
                FieldTypeEnum.MultipleLookupValues => "multipleLookupValues",
                FieldTypeEnum.AutoNumber => "autoNumber",
                FieldTypeEnum.Barcode => "barcode",
                FieldTypeEnum.Rating => "rating",
                FieldTypeEnum.RichText => "richText",
                FieldTypeEnum.Duration => "duration",
                FieldTypeEnum.LastModifiedTime => "lastModifiedTime",
                FieldTypeEnum.Button => "button",
                FieldTypeEnum.CreatedBy => "createdBy",
                FieldTypeEnum.LastModifiedBy => "lastModifiedBy",
                FieldTypeEnum.ExternalSyncSource => "externalSyncSource",
                FieldTypeEnum.AiText => "aiText",
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
    public class FieldTypeEnumConverter : ApiStringEnumConverter<FieldTypeEnum> { }

}
