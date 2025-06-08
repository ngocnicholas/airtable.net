using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public enum FieldTypeEnum
    {
        singleLineText, email, url, multilineText, number, 
        percent, currency, singleSelect, multipleSelects, singleCollaborator, 
        multipleCollaborators, multipleRecordLinks, date, dateTime, phoneNumber, 
        multipleAttachments, checkbox, formula, createdTime, rollup, 
        count, lookup, multipleLookupValues, autoNumber, barcode, 
        rating, richText, duration, lastModifiedTime, button, 
        createdBy, lastModifiedBy, externalSyncSource, aiText
    }

    public class FieldModel
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string Id { get; set; }

        [JsonPropertyName("description")]
        [JsonInclude]
        public string Description { get; set; }     // optional

        // from original FieldConfig
        [JsonPropertyName("type")]
        [JsonInclude]
        public string Type { get; set; }       // optional<Field type> https://airtable.com/developers/web/api/model/field-type
                                               // Should this be an eum or a string?
        [JsonPropertyName("name")]
        [JsonInclude]
        public string Name { get; set; }

        [JsonPropertyName("options")]
        [JsonInclude]
        public object? Options { get; set; }    // Don't make this property virtual because System.Text.Json does not handle serialization well for polymorphism

    }
    public abstract class FieldModelOptions<TOptions> : FieldModel
    {
        [JsonIgnore] // Always ignore during serialization
        public TOptions TypedOptions
        {
            get
            {
                if (Options is JsonElement jsonElement && typeof(TOptions) != typeof(object))
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    Options = jsonElement.Deserialize<TOptions>(jsonOptions);
                }
                return (TOptions)Options!;
            }
            set => Options = value!;
        }
    }

//------------------------------------------------------------

    public class FieldModelJsonConverter : JsonConverter<FieldModel>
    {
        private static readonly Dictionary<string, Type> TypeMap = new()
        {
            ["aiText"] = typeof(AiTextFieldModel),                      // R,   options
            ["multipleAttachments"] = typeof(AttachmentFieldModel),     // RW,  R options only
            ["autoNumber"] = typeof(AutoNumberFieldModel),              // R,   No
            ["barcode"] = typeof(BarcodeField),                         // RW,  No
            ["button"] = typeof(ButtonFieldModel),                      // R,   No
            ["checkbox"] = typeof(CheckboxField),                       // RW, same options
            ["singleCollaborator"] = typeof(CollaboratorField),         // RW, same options
            ["count"] = typeof(CountFieldModel),                        // R,   options
            ["createdBy"] = typeof(CreateByFieldModel),                 // R,   No
            ["createdTime"] = typeof(CreatedTimeFieldModel),            // R,   options
            ["currency"] = typeof(CurrencyField),                       // RW, same options
            ["date"] = typeof(DateField),                               // RW,  same options, but Format is optional in Write
            ["dateTime"] = typeof(DateTimeField),                       // RW,  same options, but Format is optional in Write
            ["duration"] = typeof(DurationField),                       // RW, same options
            ["email"] = typeof(EmailField),                             // RW,  No
            ["formula"] = typeof(FormulaFieldModel),                    // R,   options
            ["lastModifiedBy"] = typeof(LastModifiedByFieldModel),      // R,   No
            ["lastModifiedTim"] = typeof(LastModifiedTimeFieldModel),   // R,   options
            ["multipleRecordLinks"] = typeof(LinkToAnotherRecordFieldModel),    // RW, Write's options are only a subset of Read's. Write is only for Create, not for update.
            ["multilineText"] = typeof(LongTextField),                  // RW,  No
            ["multipleLookupValues"] = typeof(LookupFieldModel),        // R,   options
            ["multiCollaborators"] = typeof(MultipleCollaboratorField), // RW,  same options
            ["multipleSelects"] = typeof(MultipleSelectField),          // RW,  same options, but Id is optional in Write
            ["number"] = typeof(NumberField),                           // RW,  same options
            ["percent"] = typeof(PercentField),                         // RW,  same options
            ["phoneNumber"] = typeof(PhoneField),                       // RW,  No
            ["rating"] = typeof(RatingField),                           // RW,  same options 
            ["richText"] = typeof(RichTextField),                       // RW,  No
            ["rollup"] = typeof(RollupFieldModel),                      // R,   options
            ["singleLineText"] = typeof(SingleLineTextField),           // RW,  No
            ["singleSelect"] = typeof(SingleSelectField),               // RW,  Same options, but Id is optinal in Write
            ["externSyncSource"] = typeof(SyncSourceField),             // RW,  same options
            ["url"] = typeof(UrlField),                                 // RW,  No
        };

        public override FieldModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var jsonDoc = JsonDocument.ParseValue(ref reader);
            var root = jsonDoc.RootElement;

            var typeString = root.GetProperty("type").GetString();

            if (string.IsNullOrEmpty(typeString) || !TypeMap.TryGetValue(typeString, out var targetType))
            {
                throw new JsonException($"Unknown or unsupported field type: {typeString}");
            }

            //var deserialized = JsonSerializer.Deserialize(root.GetRawText(), targetType, options);
            // This is the key fix ↓
            var deserialized = (FieldModel?)JsonSerializer.Deserialize(root, targetType, options);

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

    //-------------------------end FieldModelJsonConverter-----------------------------------------------

    public class AiTextFieldModel : FieldModelOptions<AiTextOptions>
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

    //-------------------------- end of AiTextField

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
    public class AutoNumberFieldModel : FieldModel 
    {
        public AutoNumberFieldModel()
        {
            Type = "autoNumber";
        }
    }


    /// <summary>
    /// Barcode field
    /// </summary>
    public class BarcodeField : FieldModel
    {
        public BarcodeField()
        {
            Type = "barcode";
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
    /// Checkbox field
    /// </summary>
    public class CheckboxField : FieldModelOptions<CheckboxOptions>
    {
        public CheckboxField()
        {
            Type = "checkbox";
        }
    }

    public class CheckboxOptions  
    {
        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }    // "check" | "xCheckbox" | "star" | "heart" | "thumbsUp" | "flag" | "dot"
    }

    /// <summary>
    /// Collaborator field
    /// </summary>
    public class CollaboratorField : FieldModel  
    {
        public CollaboratorField()
        {
            Type = "singleCollaborator";
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
    /// Currency field
    /// </summary>
    public class CurrencyField : FieldModelOptions<CurrencyOptions>  
    {
        public CurrencyField()
        {
            Type = "currency";
        }
    }

    public class CurrencyOptions 
    {
        [JsonPropertyName("precision")]
        public int Precision { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }
    }
    //-----------------------------------------

    /// <summary>
    /// Date field
    /// </summary>
    public class DateField : FieldModelOptions<DateOptions> 
    {
        public DateField()
        {
            Type = "date";
        }
    }

    public class DateOptions 
    {
        [JsonPropertyName("dateFormat")]
        public DateFormat DateFormat { get; set; }
    }

    //---------------------------

    public class DateFormat  
    {

        [JsonPropertyName("format")]        // "l" | "LL" | "M/D/YYYY" | "D/M/YYYY" | "YYYY-MM-DD"
                                            // format is always provided when reading.
                                            // (l for local, LL for friendly, M/D/YYYY for us, D/M/YYYY for european, YYYY-MM-DD for iso)
        public string Format { get; set; }  // Format is optional when writing, but it must match the corresponding name if provided.

        [JsonPropertyName("name")]
        public string Name { get; set; }    // "local" | "friend" | "us" | "european" | "iso" 
    }


    /// <summary>
    /// Date and time field
    /// </summary>
    public class DateTimeField : FieldModelOptions<DateTimeOptions>          // for Read and for Write 
    {
        public DateTimeField()
        {
            Type = "dateTime";
        }
    }

    public class DateTimeOptions
    { 
        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }        // See https://airtable.com/developers/web/api/model/timezone for details

        [JsonPropertyName("dateFormat")]
        public DateFormat DateFormat { get; set; }      // "h:mma" | "HH:mm"

        [JsonPropertyName("timeFormat")]
        public TimeFormat TimeFormat { get; set; }      // "h:mma" | "HH:mm"
    }

    public class TimeFormat 
    {
        [JsonPropertyName("format")]                        //format is always provided when reading. (l for local, LL for friendly, M/D/YYYY for us, D/M/YYYY for european, YYYY-MM-DD for iso)
        public string Format { get; set; }                  // 	Optional for Write. "h:mma" | "HH:mm"

        [JsonPropertyName("name")]
        public string Name { get; set; }                    // "12hour" | "24hour"
    }


    /// <summary>
    /// Duration field
    /// </summary>
    public class DurationField : FieldModelOptions<DurationOptions>  
    {
        public DurationField()
        {
            Type = "duration";
        }
    }

    public class DurationOptions  
    {
        [JsonPropertyName("durationFormat")]
        public string DurationFormat { get; set; } //"h:mm" | "h:mm:ss" | "h:mm:ss.S" | "h:mm:ss.SS" | "h:mm:ss.SSS"
    }

    /// <summary>
    /// Email field
    /// </summary>
    public class EmailField : FieldModel 
    {
        public EmailField()
        {
            Type = "email";
        }
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
    /// Represents information about who last modified the record.
    /// </summary>
    public class LastModifiedByFieldModel : FieldModel
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
    /// Link to another record field
    /// </summary>
    /// <summary>
    /// Link to another record field
    /// </summary>
    public class LinkToAnotherRecordFieldModel : FieldModelOptions<LinkToAnotherRecordFieldModelOptions>
    {
        // Creating "multipleRecordLinks" fields is supported but updating options for existing "multipleRecordLinks" fields is not supported.
        public LinkToAnotherRecordFieldModel()
        {
            Type = "multipleRecordLinks";
        }
    }


    public class LinkToAnotherRecordFieldModelOptions
    {
        // Note: All properties are in the READ situations
        [JsonPropertyName("isReversed")]
        public bool IsReversed { get; set; }

        [JsonPropertyName("prefersSingleRecordLink")]
        public bool PrefersSingleRecordLink { get; set; }

        [JsonPropertyName("inverseLinkFieldId")]
        public string InverseLinkFieldId { get; set; } // Optional. The ID of the field in the linked table that links back to this one.

        // Note: Only the 2 properties below are in the CreateBase situation, not even in the UpdateBase situation)
        [JsonPropertyName("linkedTableId")]
        public string LinkedTableId { get; set; }   // The ID of the table this field links to

        [JsonPropertyName("viewIdForRecordSelection")]
        public string ViewIdForRecordSelection { get; set; } // Optional. The ID of the view in the linked table to use when showing a list of records to select from.    }

    }


    /// <summary>
    /// Long text field (multi-line)
    /// </summary>    
    public class LongTextField : FieldModel
    {
        public LongTextField()
        {
            Type = "multilineText";
        }
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

//-------------------------------------------------------------------------

    public class MultipleCollaboratorField : FieldModel 
    {
        public MultipleCollaboratorField()
        {
            Type = "multipleCollaborators";
        }
    }

    //---------------------------------------------------------

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
//--------------------------------------------

    /// <summary>
    /// Number field
    /// </summary>
    public class NumberField : FieldModelOptions<PrecisionOptions> 
    {
        public NumberField()
        {
            Type = "number";
        }
    }

    public class PrecisionOptions 
    {
        [JsonPropertyName("precision")]
        public int Precision { get; set; }  // Indicates the number of digits shown to the right of the decimal point for this field. (0-8 inclusive)
    }


    /// <summary>
    /// Percent field
    /// </summary>
    public class PercentField : FieldModelOptions<PrecisionOptions> 
    {
        public PercentField()
        {
            Type = "percent";
        }
    }

    //---------------------------------------------

    /// <summary>
    /// Phone number field
    /// </summary>
    public class PhoneField : FieldModel 
    {
        public PhoneField()
        {
            Type = "phoneNumber";
        }
    }

    //-----------------------------

    /// <summary>
    /// Rating field
    /// </summary>
    public class RatingField : FieldModelOptions<RatingOptions> 
    {
        public RatingField()
        {
            Type = "rating";
        }

    }

    public class RatingOptions 
    {
        [JsonPropertyName("color")]
        public string Color { get; set; }       // TODO: make an EMUM for this

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("max")]
        public int Max { get; set; }            // The maximum value for the rating, from 1 to 10 inclusive.
    }

//--------------------------------------------

    /// <summary>
    /// Rich text field with formatting
    /// </summary>
    public class RichTextField : FieldModel 
    {
        public RichTextField()
        {
            Type = "richText";
        }
    }

    //-----------------------------------------------

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
    //-----------------------------------------

    /// <summary>
    /// Single line text field
    /// </summary>
    public class SingleLineTextField : FieldModel 
    {
        public SingleLineTextField()
        {
            Type = "singleLineText";
        }
    }
    //-------------------------------------

    /// <summary>
    /// Single select field
    /// </summary>
    public class SingleSelectField : FieldModelOptions<ChoiceOptions>
    { 
        public SingleSelectField()
        {
            Type = "singleSelect";
        }
    }

    //---------------------------------------
    public class SyncSourceField: FieldModelOptions<ChoiceOptions>
    {
        public SyncSourceField()
        {
            Type = "externalSyncSource";
        }
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

    //------------------------------------

    /// <summary>
    /// URL field
    /// </summary>
    public class UrlField : FieldModel 
    {
        public UrlField()
        {
            Type = "url";
        }
    }
}