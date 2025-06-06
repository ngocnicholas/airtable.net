using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class FieldModel : FieldConfig
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; set; }

        [JsonPropertyName("description")]
        [JsonInclude]
        public string Description { get; set; }     // optional
    }

    public abstract class FieldModelOptions<TOptions> : FieldModel
    {
        [JsonIgnore] // Always ignore during serialization
        public TOptions TypedOptions
        {
            get
            {
                // The second condition is a defensive logic: if the jsonElement is of type object, it means it has already been deserialized
                // and Deserialize it again will give us a JsonElement (meaning raw Json) again, defeating the purpose of the conversion.
                if (Options is JsonElement jsonElement && typeof(TOptions) != typeof(object))
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                        // Add other settings here if needed
                    };

                    // Note:  Options = jsonElement.Deserialize<TOptions>(Options); is wrong because Options is the property itsel, jsonOptions is the Json SerializerOptions variable
                    Options = jsonElement.Deserialize<TOptions>(jsonOptions);
                }
                return (TOptions)Options!;
            }
            set => Options = value!;
        }
    }


    public class FieldModelJsonConverter : JsonConverter<FieldModel>
    {
        private static readonly Dictionary<string, Type> TypeMap = new()
        {
            ["aiText"] = typeof(AiTextFieldModel),
            ["multipleAttachments"] = typeof(AttachmentFieldModel),
            ["autoNumber"] = typeof(AutoNumberFieldModel),
            ["externalSyncsource"] = typeof(SyncSourceFieldModel),
            ["button"] = typeof(ButtonFieldModel),
            ["count"] = typeof(CountFieldModel),
            ["createdBy"] = typeof(CreateByFieldModel),
            ["createdTime"] = typeof(CreatedTimeFieldModel),
            ["formula"] = typeof(FormulaFieldModel),
            ["multipleRecordLinks"] = typeof(LinkToAnotherRecordFieldModel),
            ["multipleLookupValues"] = typeof(LookupFieldModel),
            ["lastModifiedBy"] = typeof(LastModifiedByFieldModel),
            ["rollup"] = typeof(RollupFieldModel),
            // The fields below are for both read and write
            ["barcode"] = typeof(BarcodeField),
            ["checkbox"] = typeof(CheckboxField),
            ["singleCollaborator"] = typeof(CollaboratorField),
            ["currency"] = typeof(CurrencyField),
            ["date"] = typeof(DateField),
            ["dateTime"] = typeof(DateTimeField),
            ["duration"] = typeof(DurationField),
            ["email"] = typeof(EmailField),
            ["multilineText"] = typeof(LongTextField),
            ["multiCollaborators"] = typeof(MultipleCollaboratorField),
            ["multipleSelects"] = typeof(MultipleSelectField),
            ["number"] = typeof(NumberField),
            ["percent"] = typeof(PercentField),
            ["phoneNumber"] = typeof(PhoneNumberField),
            ["rating"] = typeof(RatingField),
            ["richText"] = typeof(RichTextField),
            ["singleLineText"] = typeof(SingleLineTextField),
            ["singleSelect"] = typeof(SingleSelectField),
            ["externSyncSource"] = typeof(SyncSourceField),
            ["url"] = typeof(UrlField)
        };


        public override FieldModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Console.WriteLine("In FieldModelJsonConverter");

            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            var root = jsonDoc.RootElement;

            var typeString = root.GetProperty("type").GetString();

            if (string.IsNullOrEmpty(typeString) || !TypeMap.TryGetValue(typeString, out var targetType))
            {
                throw new JsonException($"Unknown or unsupported field type: {typeString}");
            }

            var deserialized = JsonSerializer.Deserialize(root.GetRawText(), targetType, options);

            if (deserialized is not FieldModel typedField)
            {
                throw new JsonException($"Deserialized object is not a FieldModel: {deserialized?.GetType()}");
            }

            return typedField; // NOTE: typedField is the same object as deserialized, just with a base class type.
        }

        public override void Write(Utf8JsonWriter writer, FieldModel value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, value.GetType(), options);
        }

    }


    public class AiTextFieldModel : FieldModelOptions<AiTextOptions>           // Not Supported in Airtable
    {
        public AiTextFieldModel()
        {
            Type = "aiText";
        }
    }


    public class AiTextOptions                                      
    {
        [JsonPropertyName("prompt")]
        [JsonConverter(typeof(PromptItemConverter))]    // Attach the converter
        public List<PromptItem> Prompt { get; set; }    // heterogeneous types: optional<array of (strings | objects of the class PromptItem

        [JsonPropertyName("referencedFieldIds")]
        public List<string> ReferencedFieldIds { get; set; }
    }


    public class PromptItem         // ChatGPT's modeling
    {
        public string TextContent { get; set; }

        [JsonPropertyName("field")]
        public FieldReference FieldRef { get; set; }

        public static PromptItem FromText(string text) => new PromptItem { TextContent = text };
        public static PromptItem FromField(string fieldId) => new PromptItem { FieldRef = new FieldReference { FieldId = fieldId } };

        public bool IsText => TextContent != null;
        public bool IsField => FieldRef != null;
    }


    public class FieldRef
    {
        [JsonPropertyName("fieldId")]
        string FieldId { get; set; } 
    }


    public class FieldReference
    {
        [JsonPropertyName("fieldId")]
        public string FieldId { get; set; }
    }

    public class PromptItemConverter : JsonConverter<List<PromptItem>>
    {
        public override List<PromptItem> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var list = new List<PromptItem>();

            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Expected StartArray");

            reader.Read();  // Read the 1st item

            while (reader.TokenType != JsonTokenType.EndArray)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    string text = reader.GetString();
                    list.Add(PromptItem.FromText(text));
                    reader.Read();  // Read the next item after text
                }
                else if (reader.TokenType == JsonTokenType.StartObject)
                {
                    using var doc = JsonDocument.ParseValue(ref reader);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("field", out var fieldElem))
                    {
                        var fieldRef = fieldElem.Deserialize<FieldReference>(options);
                        list.Add(PromptItem.FromField(fieldRef.FieldId));
                    }
                    else
                    {
                        throw new JsonException("Expected 'field' object in prompt item.");
                    }
                    //Read next item after object
                    reader.Read();
                }
                else
                {
                    throw new JsonException("Unexpected token in prompt array.");
                }
            }

            return list;
        }

        public override void Write(Utf8JsonWriter writer, List<PromptItem> value, JsonSerializerOptions options)
        {
            // Attachement is READ only
            throw new NotImplementedException();
        }
    }

    public class AttachmentFieldModel : FieldModelOptions<AttachementReadOptions>
    { 
        public AttachmentFieldModel()
        {
            Type = "multipleAttachments";
        }
    }

    public class AttachementReadOptions                
    {
        [JsonPropertyName("isReversed")]
        public bool IsReversed { get; set; }
    }

    /// <summary>
    /// Auto number field
    /// </summary>
    public class AutoNumberFieldModel : FieldModel                  // Not Supported in Airtable
    {
        public AutoNumberFieldModel()
        {
            Type = "autoNumber";
        }
    }

    /// <summary>
    /// Button field
    /// </summary>
    public class ButtonFieldModel : FieldModel
    {
        public ButtonFieldModel()
        {
            Type = "button";
        }
    }

    /// <summary>
    /// Count field
    /// </summary>
    public class CountFieldModel : FieldModelOptions<CountOptions>
    {
        public CountFieldModel()
        {
            Type = "count";
        }
    }

    public class CountOptions
    {
        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }   // false when recordLinkFieldId is null, e.g. the referenced column was deleted.

        [JsonPropertyName("recordLinkFieldId")]
        public string RecordLinkFieldId { get; set; }   // optional<string | null >
    }

    /// <summary>
    /// Created By field
    /// </summary>
    public class CreateByFieldModel : FieldModel
    {
        public CreateByFieldModel()
        {
            Type = "createdBy";
        }
    }

    /// <summary>
    /// Created time field
    /// </summary>
    public class CreatedTimeFieldModel : FieldModelOptions<CreatedTimeOptions>
    {
        public CreatedTimeFieldModel()
        {
            Type = "createdTime";
        }
    }

    public class CreatedTimeOptions
    {
        [JsonPropertyName("result")]
        public CreatedTimeResult Result { get; set; }

    }

    public class CreatedTimeResult
    {
        [JsonPropertyName("date")]
        public DateField Date { get; set; }

        [JsonPropertyName("dateTime")]
        public DateTimeField DateTime { get; set; }
    }

    /// <summary>
    /// Formula field
    /// </summary>
    public class FormulaFieldModel : FieldModelOptions<FormulaOptions>
    {
        public FormulaFieldModel()
        {
            Type = "formula";
        }
    }

    public class FormulaOptions
    {
        //The formula including fields referenced by their IDs.For example, LEFT(4, { Birthday}) in the Airtable.com formula editor
        //will be returned as LEFT(4, { fldXXX}) via API.
        [JsonPropertyName("formula")]
        public string Formula { get; set; }

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("referencedFieldIds")]
        public string[] ReferencedFieldIds { get; set; }        // array of strings | null
                                                                // All fields in the record that are used in the formula.

        [JsonPropertyName("result")]
        // public object Result { get; set; }                      // Field type and options | null if invalid. See https://airtable.com/developers/web/api/field-model
        public FieldModel Result { get; set; }                 // Question: should the type be FieldModel or object???
    }


    /// <summary>
    /// Link to another record field
    /// </summary>
    public class LinkToAnotherRecordFieldModel : FieldModelOptions<LinkToAnotherRecordFieldModelOptions>
    {
        public LinkToAnotherRecordFieldModel()
        {
            Type = "multipleRecordLinks";
        }
    }

    public class LinkToAnotherRecordFieldModelOptions : LinkToAnotherRecordFieldConfigOptions
    {
        // Note: All properties are in the READ situations
        [JsonPropertyName("isReversed")]
        public bool IsReversed { get; set; }

        [JsonPropertyName("prefersSingleRecordLink")]
        public bool PrefersSingleRecordLink { get; set; }

        [JsonPropertyName("inverseLinkFieldId")]
        public string InverseLinkFieldId { get; set; } // Optional. The ID of the field in the linked table that links back to this one.
    }



    /// <summary>
    /// Lookup field
    /// </summary>
    public class LookupFieldModel : FieldModelOptions<LookupOptions>
    {
        public LookupFieldModel()
        {
            Type = "multipleLookupValues";
        }
    }

    public class LookupOptions
    {
        [JsonPropertyName("fieldIdInLinkedTable")]
        public string FieldIdInLinkedTable { get; set; }    // may be null

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("recordLinkFieldId")]
        public string RecordLinkFieldId { get; set; }       // may be null

        [JsonPropertyName("result")]
        public object Result { get; set; }                  // 	Field type and options | null. See https://airtable.com/developers/web/api/field-model for details
    }


    /// <summary>
    /// Represents information about who last modified the record.
    /// </summary>
    public class LastModifiedByFieldModel: FieldModel
    {
        public LastModifiedByFieldModel()
        {
            Type = "lastModifiedBy";
        }
    }

    /// <summary>
    /// Last modified time field
    /// </summary>
    public class LastModifiedTimeFieldModel : FieldModelOptions<LastModifiedTimeOptions>
    {
        public LastModifiedTimeFieldModel()
        {
            Type = "lastModifiedTime";
        }
    }

    public class LastModifiedTimeOptions
    {
        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("referencedFieldIds")]
        public string[] ReferencedFieldIds { get; set; }        // array of strings | null
                                                                // All fields in the record that are used in the formula.

        [JsonPropertyName("result")]
        public LastModifedTimeResult result { get; set; }                      // Either DateField or DatFiemField | null
    }

    public class LastModifedTimeResult
    {
        // null | any of the below objects
        // This will always be a date or dateTime field config.
        [JsonPropertyName("date")]
        public DateField Date { get; set; }

        [JsonPropertyName("dateTime")]
        public DateTimeField DateTime { get; set; }

    }

    /// <summary>
    /// Multi-select field
    /// </summary>
    public class MultipleSelectField : FieldModelOptions<ChoiceOptions>
    {
        public MultipleSelectField()
        {
            Type = "multipleSelects";
        }
    }

    public class ChoiceOptions
    {
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("id")]                    // opional only during Write but is always required for SyncSource
                                                    // This is not specified when creating new options,
                                                    // useful when specifing existing options (for example: reordering options,
                                                    // keeping old options and adding new ones, etc)
        public string Id { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }           // optional during Read when the select field is configured to not use colors.           
                                                    // optional duiring Write - This is not specified when creating new options,
                                                    // useful when specifing existing options (for example: reordering options,
                                                    // keeping old options and adding new ones, etc)
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }


    /// <summary>
    /// Rollup field
    /// </summary>
    public class RollupFieldModel : FieldModelOptions<RollupOptions>           // NEED EMMETT"S HELP in TESTING this field
    {
        public RollupFieldModel()
        {
            Type = "rollup";
        }
    }

    public class RollupOptions
    {
        [JsonPropertyName("fieldIdInLinkedTable")]
        public string FieldIdInLinkedTable { get; set; }    // optional

        [JsonPropertyName("recordLinkFieldId")]
        public string RecordLinkFieldId { get; set; }       // optional

        [JsonPropertyName("result")]                        // optional<Field type and options | null> Can be null is invalid
        public object Result { get; set; }                  // optional

        [JsonPropertyName("isValid")]
        public bool IsVaid { get; set; }                   // valid

        [JsonPropertyName("referencedFieldIds")]
        public string[] ReferencedFieldIds { get; set; }    // optional
    }

    /// <summary>
    /// Sync Source field
    /// </summary>   
    public class SyncSourceFieldModel : FieldModelOptions<SyncSourceOptions>
    {
        public SyncSourceFieldModel()
        {
            Type = "externalSyncSource";
        }
    }

    public class SyncSourceOptions
    {
        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }
    }
}