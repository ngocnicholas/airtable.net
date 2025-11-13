using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public enum FieldEnum
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
        public static string ToApiString(this FieldEnum field)
        {
            return field switch
            {
                FieldEnum.SingleLineText => "singleLineText",
                FieldEnum.Email => "email",
                FieldEnum.Url => "url",
                FieldEnum.MultilineText => "multilineText",
                FieldEnum.Number => "number",
                FieldEnum.Percent => "percent",
                FieldEnum.Currency => "currency",
                FieldEnum.SingleSelect => "singleSelect",
                FieldEnum.MultipleSelects => "multipleSelects",
                FieldEnum.SingleCollaborator => "singleCollaborator",
                FieldEnum.MultipleCollaborators => "multipleCollaborators",
                FieldEnum.MultipleRecordLinks => "multipleRecordLinks",
                FieldEnum.Date => "date",
                FieldEnum.DateTime => "dateTime",
                FieldEnum.PhoneNumber => "phoneNumber",
                FieldEnum.MultipleAttachments => "multipleAttachments",
                FieldEnum.Checkbox => "checkbox",
                FieldEnum.Formula => "formula",
                FieldEnum.CreatedTime => "createdTime",
                FieldEnum.Rollup => "rollup",
                FieldEnum.Count => "count",
                FieldEnum.MultipleLookupValues => "multipleLookupValues",
                FieldEnum.AutoNumber => "autoNumber",
                FieldEnum.Barcode => "barcode",
                FieldEnum.Rating => "rating",
                FieldEnum.RichText => "richText",
                FieldEnum.Duration => "duration",
                FieldEnum.LastModifiedTime => "lastModifiedTime",
                FieldEnum.Button => "button",
                FieldEnum.CreatedBy => "createdBy",
                FieldEnum.LastModifiedBy => "lastModifiedBy",
                FieldEnum.ExternalSyncSource => "externalSyncSource",
                FieldEnum.AiText => "aiText",
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
    public class FieldEnumConverter : ApiStringEnumConverter<FieldEnum> { }

}
