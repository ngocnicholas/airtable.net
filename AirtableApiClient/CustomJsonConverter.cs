using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AirtableApiClient
{
    //------------------------------------------------------------

    public class FieldConfigJsonConverter : JsonConverter<FieldConfig>    // Custrom Json converter for FieldConfig due to System.Text.Json's de/serilization not able to handle polymorhism correctly
    {
        private static readonly Dictionary<string, Type> TypeMap = new()
        {
            ["aiText"] = typeof(AiTextField_Read),                      // R,   options
            ["multipleAttachments"] = typeof(AttachmentField_Read),     // R,   No
            ["autoNumber"] = typeof(AutoNumberField_Read),              // R,   No
            ["barcode"] = typeof(BarcodeField),                         // RW,  No
            ["button"] = typeof(ButtonField_Read),                      // R,   No
            ["checkbox"] = typeof(CheckboxField),                       // RW, same options
            ["singleCollaborator"] = typeof(CollaboratorField),         // RW, same options
            ["count"] = typeof(CountField_Read),                        // R,   options
            ["createdBy"] = typeof(CreatedByField_Read),                // R,   No
            ["createdTime"] = typeof(CreatedTimeField_Read),            // R,   options
            ["currency"] = typeof(CurrencyField),                       // RW, same options
            ["date"] = typeof(DateField),                               // RW,  same options, but Format is optional in Write
            ["dateTime"] = typeof(DateTimeField),                       // RW,  same options, but Format is optional in Write
            ["duration"] = typeof(DurationField),                       // RW, same options
            ["email"] = typeof(EmailField),                             // RW,  No
            ["formula"] = typeof(FormulaField_Read),                    // R,   options
            ["lastModifiedBy"] = typeof(LastModifiedByField_Read),      // R,   No
            ["lastModifiedTime"] = typeof(LastModifiedTimeField_Read),  // R,   options
            ["multipleRecordLinks"] = typeof(LinkToAnotherRecordField_Read),    // RW, Write's options are only a subset of Read's. Write is only for Create, not for update.
            ["multilineText"] = typeof(LongTextField),                  // RW,  No
            ["multipleLookupValues"] = typeof(LookupField_Read),        // R,   options
            ["multipleCollaborators"] = typeof(MultipleCollaboratorField), // RW,  same options
            ["multipleSelects"] = typeof(MultipleSelectField),          // RW,  same options, but Id is optional in Write
            ["number"] = typeof(NumberField),                           // RW,  same options
            ["percent"] = typeof(PercentField),                         // RW,  same options
            ["phoneNumber"] = typeof(PhoneField),                       // RW,  No
            ["rating"] = typeof(RatingField),                           // RW,  same options 
            ["richText"] = typeof(RichTextField),                       // RW,  No
            ["rollup"] = typeof(RollupField_Read),                      // R,   options
            ["singleLineText"] = typeof(SingleLineTextField),           // RW,  No
            ["singleSelect"] = typeof(SingleSelectField),               // RW,  Same options, but Id is optinal in Write
            ["externalSyncSource"] = typeof(SyncSourceField),           // RW,  same options
            ["url"] = typeof(UrlField),                                 // RW,  No
        };


        // For a Json converter, override Read is for Deserialization. The custom Serializer and Deserializer in this file is soly for
        // the Config type.

        // We use this custom method to deserialize responseBody of GetBaseSchema() into the C# object to be used in constructing an AirtableApiResponse
        public override FieldConfig Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions jasonSerializerOptions)
        {
            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            var root = jsonDoc.RootElement;

            string? typeString = root.GetProperty("type").GetString();

            if (string.IsNullOrEmpty(typeString))
            {
                throw new JsonException($"Unknown or unsupported field type: {typeString}");
            }

            if (!TypeMap.TryGetValue(typeString!, out var targetType))
            {
                // Build a tolerant "unknown" instance and keep original JSON
                root.TryGetProperty("id", out var idProp);
                root.TryGetProperty("name", out var nameProp);

                UnknownField_Read unknownField =  new UnknownField_Read
                {
                    Id = idProp.ValueKind == JsonValueKind.String ? idProp.GetString() : null,
                    Name = nameProp.ValueKind == JsonValueKind.String ? nameProp.GetString() : null,
                    UnknownType = typeString,
                    FieldConfigRawJson = root.GetRawText(),
                    OptionsRawJson = root.TryGetProperty("options", out var optEl) ? optEl.GetRawText() : null
               };
                return unknownField;
            }

            // Deserialize to the mapped concrete type
            var obj = root.Deserialize(targetType, jasonSerializerOptions)    // or JsonSerializer.Deserialize(root.GetRawText(), targetType, options)
                      ?? throw new JsonException($"Failed to deserialize field of type '{typeString}'.");

            // If it's a FieldOptions<T> type, eagerly deserialize the options. Otherwise TOptions won't be deserialized until it's accessed.

            var fc = (FieldConfig)obj;

            // fc is the concrete field (already deserialized to targetType)
            var t = fc.GetType();

            // find which generic base it uses (if any)
            static Type? FindGenericBase(Type cur, Type openGeneric)
            {
                for (var x = cur; x != null; x = x.BaseType)
                    if (x.IsGenericType && x.GetGenericTypeDefinition() == openGeneric)
                        return x;
                return null;
            }

            var readBase = FindGenericBase(t, typeof(ReadFieldConfig<>));
            var rwBase = FindGenericBase(t, typeof(ReadWriteFieldConfig<>));

            Type? genericBase = readBase ?? rwBase;
            if (genericBase != null)
            {
                // property name depends on which base we matched
                var propName = (readBase != null) ? "ReadOptions" : "Options";
                var prop = t.GetProperty(propName);

                // Only do the extra pass if the property is still a JsonElement
                if (prop?.GetValue(fc) is JsonElement optionsElement)
                {
                    var optionsType = genericBase.GetGenericArguments()[0];
                    var typed = JsonSerializer.Deserialize(optionsElement.GetRawText(), optionsType, jasonSerializerOptions);
                    prop.SetValue(fc, typed);
                }
            }

            return fc;
        }

        // For a Json Converter, override Write is for Serialization
        public override void Write(Utf8JsonWriter writer, FieldConfig value, JsonSerializerOptions jasonSerializerOptions)
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), jasonSerializerOptions);  // The Json output stream will go into the HTTP request body of CreateBase
        }

    }

    public static class TypeExtensions
    {
        public static bool IsSubclassOfGenericType(this Type type, Type genericType)
        {
            while (type != null && type != typeof(object))                      // Traverse the inheritance tree back to the root (type object)
            {
                var current = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (genericType == current) // the 'type' passed in is a derived class of genericType?
                    return true;
                type = type.BaseType!;
            }
            return false;
        }
    }

    public sealed class PromptItemListConverter : JsonConverter<List<PromptItem>>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            bool canConvert = typeToConvert == typeof(List<PromptItem>);
            return canConvert;
        }

        public override List<PromptItem> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Expected start of array");

            var items = new List<PromptItem>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;

                PromptItem item;

                if (reader.TokenType == JsonTokenType.String)
                {
                    // Handle string elements: "CEO of " -> PromptItem with TextContent
                    item = PromptItem.FromText(reader.GetString()!);
                }
                else if (reader.TokenType == JsonTokenType.StartObject)
                {
                    // Handle object elements
                    using var doc = JsonDocument.ParseValue(ref reader);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("field", out var fieldElem))
                    {
                        var field = fieldElem.Deserialize<FieldReference>(options);
                        item = PromptItem.FromField(field!.FieldId!);
                    }
                    else if (root.TryGetProperty("textContent", out var textElem))
                    {
                        item = PromptItem.FromText(textElem.GetString()!);
                    }
                    else
                    {
                        //Console.WriteLine($"Unexpected object structure: {root.GetRawText()}");
                        throw new JsonException("Expected 'field' or 'textContent' in prompt item.");
                    }
                }
                else
                {
                    throw new JsonException($"Unexpected token {reader.TokenType} in prompt array.");
                }

                items.Add(item);
            }

            return items;
        }

        public override void Write(Utf8JsonWriter writer, List<PromptItem> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();

            foreach (var item in value)
            {
                if (!string.IsNullOrEmpty(item.TextContent))
                {
                    writer.WriteStringValue(item.TextContent);
                }
                else if (item.Field != null)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("field");
                    JsonSerializer.Serialize(writer, item.Field, options);
                    writer.WriteEndObject();
                }
            }

            writer.WriteEndArray();
        }
    }

    //------------------------------------------------------
    public sealed class WriteFieldConfigConverter : JsonConverter<WriteFieldConfig>
    {
        public override WriteFieldConfig? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotSupportedException("Reading write models is not supported here.");

        public override void Write(Utf8JsonWriter writer, WriteFieldConfig value, JsonSerializerOptions options)
        {
            // Serialize using the *runtime* type so derived-only props (Options/WriteOptions) are included
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }
    }
}
