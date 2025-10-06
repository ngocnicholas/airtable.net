using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public interface IField { }
    public interface IReadField : IField { }   // can appear in server responses
    public interface IWriteField : IField { }   // can be sent in CreateBase

    public class FieldConfig : IField
    {
        [JsonPropertyName("id")][JsonInclude] public string? Id { get; set; }
        [JsonPropertyName("description")][JsonInclude] public string? Description { get; set; }

        // Option 1: enum (preferred)
        [JsonPropertyName("type")]
        [JsonConverter(typeof(FieldTypeConverter))]
        [JsonInclude]
        public FieldType Type { get; set; }

        [JsonPropertyName("name")][JsonInclude] public string? Name { get; set; }
    }

    public abstract class WriteFieldConfig : FieldConfig, IWriteField { }

    public abstract class ReadFieldConfig : FieldConfig, IReadField { }

    // Generic base for read-options fields
    public abstract class ReadFieldConfig<TReadOptions> : FieldConfig, IReadField
    {
        [JsonPropertyName("options")]
        public TReadOptions? ReadOptions { get; set; }
    }

    /// write-only, with options
    public abstract class WriteFieldConfig<TWriteOptions> : WriteFieldConfig
    {
        [JsonPropertyName("options")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TWriteOptions? WriteOptions { get; set; }
    }

    // read–write, no options (inherits the writeable base!)
    public abstract class ReadWriteFieldConfig : WriteFieldConfig, IReadField { }

    // If a field is RW with the **same** options type in both directions:
    public abstract class ReadWriteFieldConfig<TOptions> : ReadWriteFieldConfig
    {
        [JsonPropertyName("options")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TOptions? Options { get; set; }
    }

    public sealed class AiTextField_Read : ReadFieldConfig<AiTextReadOptions>       // R only, with options
    { 
        public AiTextField_Read() { Type = FieldType.AiText; } 
    }

    public class AiTextReadOptions
    {
        [JsonPropertyName("prompt")]
        [JsonConverter(typeof(PromptItemListConverter))]    // attribute for its custom converter
        public List<PromptItem>? Prompt { get; set; }       // heterogeneous types: optional<array of (strings | objects of the class PromptItem)>

        [JsonPropertyName("referencedFieldIds")]
        public List<string>? ReferencedFieldIds { get; set; }   // A list of Field IDs referred to in all the PromptItems of Prompt.
                                                                // In the example below, the field ID of "Name" will be in this List.
    }

    public class FieldReference
    {
        [JsonPropertyName("fieldId")]
        public string? FieldId { get; set; }
    }

    public class PromptItem
    {
        /*
         * Example of a prompt which has three PromptItems:
         * CEO of [field ID of "Name"] Return the first, middle, and last name.
         * Prompt[0] has TextContent = "CEO of"
         * Prompt[1] has the FieldRef = field ID of "Name"
         * Prompt[2] has TextContent = "Return the first, middle, and last name."
         * */
        [JsonPropertyName("textContent")]
        public string? TextContent { get; set; }

        [JsonPropertyName("field")]
        public FieldReference? Field { get; set; }

        public static PromptItem FromText(string text) => new PromptItem { TextContent = text };
        public static PromptItem FromField(string fieldId) => new PromptItem { Field = new FieldReference { FieldId = fieldId } };
    }

    //-------------------------- end of AiTextField


    // Attachment RW, but has read options only
    public sealed class AttachmentField_Read : ReadFieldConfig<AttachmentFieldReadOptions>
    {
        public AttachmentField_Read() => Type = FieldType.MultipleAttachments;
    }

    public sealed class AttachmentFieldReadOptions
    {
        [JsonPropertyName("isReversed")]
        public bool IsReversed { get; set; }
    }

    // WRITE shape (no options allowed in payload)
    public sealed class AttachmentField_Write : WriteFieldConfig
    {
        public AttachmentField_Write() => Type = FieldType.MultipleAttachments;
    }

    /// <summary>
    /// Auto number field
    /// </summary>
    public class AutoNumberField_Read : ReadFieldConfig
    {
        public AutoNumberField_Read()
        {
            Type = FieldType.AutoNumber;
        }
    }


    /// <summary>
    /// Barcode field
    /// </summary>
    public class BarcodeField : ReadWriteFieldConfig
    {
        public BarcodeField()
        {
            Type = FieldType.Barcode;
        }
    }


    /// <summary>
    /// Button field
    /// </summary>
    public class ButtonField_Read : ReadFieldConfig
    {
        public ButtonField_Read()
        {
            Type = FieldType.Button;
        }
    }

    public class CheckboxField : ReadWriteFieldConfig<CheckboxOptions>  // RW, with same options
    {
        public CheckboxField()
        {
            Type = FieldType.Checkbox;
        }
    }

    public class CheckboxOptions
    {
        [JsonPropertyName("color")]
        public string? Color { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }    // "check" | "xCheckbox" | "star" | "heart" | "thumbsUp" | "flag" | "dot"
    }

    /// <summary>
    /// Collaborator field
    /// </summary>
    public class CollaboratorField : ReadWriteFieldConfig
    {
        public CollaboratorField()
        {
            Type = FieldType.SingleCollaborator;
        }
    }


    /// <summary>
    /// Count field
    /// </summary>
    public class CountField_Read : ReadFieldConfig<CountFieldReadOptions>
    {
        public CountField_Read()
        {
            Type = FieldType.Count;
        }
    }

    public class CountFieldReadOptions
    {
        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }   // false when recordLinkFieldId is null, e.g. the referenced column was deleted.

        [JsonPropertyName("recordLinkFieldId")]
        public string? RecordLinkFieldId { get; set; }   // optional<string | null >
    }

    /// <summary>
    /// Created By field
    /// </summary>
    public class CreatedByField_Read : ReadFieldConfig
    {
        public CreatedByField_Read()
        {
            Type = FieldType.CreatedBy;
        }
    }

    /// <summary>
    /// Created time field
    /// </summary>
    public class CreatedTimeField_Read : ReadFieldConfig<CreatedTimeFieldReadOptions>
    {
        public CreatedTimeField_Read()
        {
            Type = FieldType.CreatedTime;
        }
    }

    public class CreatedTimeFieldReadOptions
    {
        [JsonPropertyName("result")]
        public CreatedTimeResult? Result { get; set; }

    }

    public class CreatedTimeResult
    {
        [JsonPropertyName("date")]
        public DateField? Date { get; set; }

        [JsonPropertyName("dateTime")]
        public DateTimeField? DateTime { get; set; }
    }

    /// <summary>
    /// Currency field
    /// </summary>
    public class CurrencyField : ReadWriteFieldConfig<CurrencyOptions>
    {
        public CurrencyField()
        {
            Type = FieldType.Currency;
        }
    }

    public class CurrencyOptions
    {
        private int _precision;

        [JsonPropertyName("precision")]
        public int Precision
        {
            get => _precision;
            set => _precision = value < 0 ? 0 : value > 7 ? 7 : value;  // Indicates the number of digits shown to right of the decimal point (0-7 inclusive)
        }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
    }
    //-----------------------------------------

    /// <summary>
    /// Date field
    /// </summary>
    public class DateField : ReadWriteFieldConfig<DateFieldOptions>  // RW, with Options but Write's format is optional
    {
        public DateField()
        {
            Type = FieldType.Date;
        }
    }

    public class DateFieldOptions
    {
        [JsonPropertyName("dateFormat")]
        public DateFormat? DateFormat { get; set; }
    }

    //---------------------------

    public class DateFormat
    {

        [JsonPropertyName("format")]        // "l" | "LL" | "M/D/YYYY" | "D/M/YYYY" | "YYYY-MM-DD"
                                            // format is always provided when reading.
                                            // (l for local, LL for friendly, M/D/YYYY for us, D/M/YYYY for european, YYYY-MM-DD for iso)
        public string? Format { get; set; }  // Format is optional when writing, but it must match the corresponding name if provided.

        [JsonPropertyName("name")]
        public string? Name { get; set; }    // "local" | "friend" | "us" | "european" | "iso" 
    }


    /// <summary>
    /// Date and time field
    /// </summary>
    public class DateTimeField : ReadWriteFieldConfig<DateTimeFieldOptions>          // RW, with Options but Write's format is optional
    {
        public DateTimeField()
        {
            Type = FieldType.DateTime;
        }
    }

    public class DateTimeFieldOptions
    {
        [JsonPropertyName("timeZone")]
        public string? TimeZone { get; set; }        // See https://airtable.com/developers/web/api/model/timezone for details

        [JsonPropertyName("dateFormat")]
        public DateFormat? DateFormat { get; set; }      // "h:mma" | "HH:mm"

        [JsonPropertyName("timeFormat")]
        public TimeFormat? TimeFormat { get; set; }      // "h:mma" | "HH:mm"
    }

    public class TimeFormat
    {
        [JsonPropertyName("format")]                        //format is always provided when reading. (l for local, LL for friendly, M/D/YYYY for us, D/M/YYYY for european, YYYY-MM-DD for iso)
        public string? Format { get; set; }                  // 	Optional for Write. "h:mma" | "HH:mm"

        [JsonPropertyName("name")]
        public string? Name { get; set; }                    // "12hour" | "24hour"
    }


    /// <summary>
    /// Duration field
    /// </summary>
    public class DurationField : ReadWriteFieldConfig<DurationOptions>
    {
        public DurationField()
        {
            Type = FieldType.Duration;
        }
    }

    public class DurationOptions
    {
        [JsonPropertyName("durationFormat")]
        public string? DurationFormat { get; set; } //"h:mm" | "h:mm:ss" | "h:mm:ss.S" | "h:mm:ss.SS" | "h:mm:ss.SSS"
    }

    /// <summary>
    /// Email field
    /// </summary>
    public class EmailField : ReadWriteFieldConfig
    {
        public EmailField()
        {
            Type = FieldType.Email;
        }
    }

    /// <summary>
    /// Formula field
    /// </summary>
    public class FormulaField_Read : ReadFieldConfig<FormulaFieldReadOptions>
    {
        public FormulaField_Read()
        {
            Type = FieldType.Formula;
        }
    }

    public class FormulaFieldReadOptions
    {
        //The formula including fields referenced by their IDs.For example, LEFT(4, { Birthday}) in the Airtable.com formula editor
        //will be returned as LEFT(4, { fldXXX}) via API.
        [JsonPropertyName("formula")]
        public string? Formula { get; set; }

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("referencedFieldIds")]
        public string[]? ReferencedFieldIds { get; set; }       // array of strings | null
                                                                // All fields in the record that are used in the formula.

        [JsonPropertyName("result")]
        // public object Result { get; set; }                   // Field type and options | null if invalid. See https://airtable.com/developers/web/api/field-model
        public FieldConfig? Result { get; set; }                // Question: should the type be FieldDefinition or object???
    }

    /// <summary>
    /// Represents information about who last modified the record.
    /// </summary>
    public class LastModifiedByField_Read : ReadFieldConfig  // Read only, no options
    {
        public LastModifiedByField_Read()
        {
            Type = FieldType.LastModifiedBy;
        }
    }


    /// <summary>
    /// Last modified time field
    /// </summary>
    public class LastModifiedTimeField_Read : ReadFieldConfig<LastModifiedTimeFieldReadOptions>
    {
        public LastModifiedTimeField_Read()
        {
            Type = FieldType.LastModifiedTime;
        }
    }

    public class LastModifiedTimeFieldReadOptions
    {
        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("referencedFieldIds")]
        public string[]? ReferencedFieldIds { get; set; }       // array of strings | null
                                                                // All fields in the record that are used in the formula.

        [JsonPropertyName("result")]
        public LastModifedTimeResult? Result { get; set; }      // Either DateField or DatFiemField | null
    }

    public class LastModifedTimeResult
    {
        // null | any of the below objects
        // This will always be a date or dateTime field config.
        [JsonPropertyName("date")]
        public DateField? Date { get; set; }

        [JsonPropertyName("dateTime")]
        public DateTimeField? DateTime { get; set; }

    }

    /// <summary>
    /// Link to another record field
    /// </summary>
    /// <summary>
    /// Link to another record field
    /// </summary>
    public class LinkToAnotherRecordField_Read : ReadFieldConfig<LinkToAnotherRecordFieldReadOptions>
    {
        // Creating "multipleRecordLinks" fields is supported but updating options for existing "multipleRecordLinks" fields is not supported.
        public LinkToAnotherRecordField_Read()
        {
            Type = FieldType.MultipleRecordLinks;
        }
    }


    public class LinkToAnotherRecordFieldReadOptions
    {
        // Note: All properties are in the READ situations
        [JsonPropertyName("isReversed")]
        public bool IsReversed { get; set; }

        [JsonPropertyName("prefersSingleRecordLink")]
        public bool PrefersSingleRecordLink { get; set; }

        [JsonPropertyName("inverseLinkFieldId")]
        public string? InverseLinkFieldId { get; set; } // Optional. The ID of the field in the linked table that links back to this one.

        [JsonPropertyName("linkedTableId")]
        public string? LinkedTableId { get; set; }   // The ID of the table this field links to

        [JsonPropertyName("viewIdForRecordSelection")]
        public string? ViewIdForRecordSelection { get; set; } // Optional. The ID of the view in the linked table to use when showing a list of records to select from.    }

    }
    public class LinkToAnotherRecordField_Write : WriteFieldConfig<LinkToAnotherRecordFieldWriteOptions>
    {
        // Creating "multipleRecordLinks" fields is supported but updating options for existing "multipleRecordLinks" fields is not supported.
        public LinkToAnotherRecordField_Write()
        {
            Type = FieldType.MultipleRecordLinks;
        }
    }

    public class LinkToAnotherRecordFieldWriteOptions
    {
        // Note: All properties are in the Write situations
        // Note: Only the 2 properties below are in the CreateBase situation, not even in the UpdateBase situation)
        [JsonPropertyName("linkedTableId")]
        public string? LinkedTableId { get; set; }   // The ID of the table this field links to

        [JsonPropertyName("viewIdForRecordSelection")]
        public string? ViewIdForRecordSelection { get; set; } // Optional. The ID of the view in the linked table to use when showing a list of records to select from.    }

    }

    /// <summary>
    /// Long text field (multi-line)
    /// </summary>    
    public class LongTextField : ReadWriteFieldConfig
    {
        public LongTextField()
        {
            Type = FieldType.MultilineText;
        }
    }

    /// <summary>
    /// Lookup field
    /// </summary>
    public class LookupField_Read : ReadFieldConfig<LookupFieldReadOptions>
    {
        public LookupField_Read()
        {
            Type = FieldType.MultipleLookupValues;
        }
    }

    public class LookupFieldReadOptions
    {
        [JsonPropertyName("fieldIdInLinkedTable")]
        public string? FieldIdInLinkedTable { get; set; }    // may be null

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("recordLinkFieldId")]
        public string? RecordLinkFieldId { get; set; }       // may be null

        [JsonPropertyName("result")]
        public FieldConfig? Result { get; set; }             //	Field type and options | null. See https://airtable.com/developers/web/api/field-model for details
    }

    public class MultipleCollaboratorField : ReadWriteFieldConfig<object> 
    {
        public MultipleCollaboratorField()
        {
            Type = FieldType.MultipleCollaborators;
        }
    }

    /// <summary>
    /// Multi-select field
    /// </summary>
    public class MultipleSelectField : ReadWriteFieldConfig<ChoiceOptions>  // RW with options but the Write's option is more flexible
    {
        public MultipleSelectField()
        {
            Type = FieldType.MultipleSelects;
        }
    }

    public class ChoiceOptions
    {
        [JsonPropertyName("choices")]
        public List<Choice>? Choices { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("id")]                    // opional only during Write but is always required for SyncSource
                                                    // This is not specified when creating new options,
                                                    // useful when specifing existing options (for example: reordering options,
                                                    // keeping old options and adding new ones, etc)
        public string? Id { get; set; }

        [JsonPropertyName("color")]
        public string? Color { get; set; }     // optional during Read when the select field is configured to not use colors.           
                                                    // optional duiring Write - This is not specified when creating new options,
                                                    // useful when specifing existing options (for example: reordering options,
                                                    // keeping old options and adding new ones, etc)
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    /// <summary>
    /// Number field
    /// </summary>
    public class NumberField : ReadWriteFieldConfig<PrecisionOptions> 
    {
        public NumberField()
        {
            Type = FieldType.Number;
        }
    }

    public class PrecisionOptions 
    {
        private int _precision;

        [JsonPropertyName("precision")]
        public int Precision                // Indicates the number of digits shown to the right of the decimal point for this field. (0-8 inclusive)
        {
            get => _precision;
            set => _precision = value < 0 ? 0 : value > 8 ? 8 : value;  // 0-8 inclusive per API
        }
    }


    /// <summary>
    /// Percent field
    /// </summary>
    public class PercentField : ReadWriteFieldConfig<PrecisionOptions> 
    {
        public PercentField()
        {
            Type = FieldType.Percent;
        }
    }
    //---------------------------------------------

    /// <summary>
    /// Phone number field
    /// </summary>
    public class PhoneField : ReadWriteFieldConfig 
    {
        public PhoneField()
        {
            Type = FieldType.PhoneNumber;
        }
    }

    //-----------------------------

    /// <summary>
    /// Rating field
    /// </summary>
    public class RatingField : ReadWriteFieldConfig<RatingOptions> 
    {
        public RatingField()
        {
            Type = FieldType.Rating;
        }

    }

    public class RatingOptions 
    {
        [JsonPropertyName("color")]
        public string? Color { get; set; }
        [JsonPropertyName("icon")]
        public string? Icon { get; set; }

        private int _max;

        [JsonPropertyName("max")]
        public int Max
        {
            get => _max;
            set => _max = value < 1 ? 1 : value > 10 ? 10 : value;  // The maximum value for the rating, from 1 to 10 inclusive.
        }

    }

    //--------------------------------------------

    /// <summary>
    /// Rich text field with formatting
    /// </summary>
    public class RichTextField : ReadWriteFieldConfig
    {
        public RichTextField()
        {
            Type = FieldType.RichText;
        }
    }

    //-----------------------------------------------

    /// <summary>
    /// Rollup field
    /// </summary>
    public class RollupField_Read : ReadFieldConfig<RollupFieldReadOptions>           // NEED EMMETT"S HELP in TESTING this field
    {
        public RollupField_Read()
        {
            Type = FieldType.Rollup;
        }
    }

    public class RollupFieldReadOptions
    {
        [JsonPropertyName("fieldIdInLinkedTable")]
        public string? FieldIdInLinkedTable { get; set; }    // optional

        [JsonPropertyName("recordLinkFieldId")]
        public string? RecordLinkFieldId { get; set; }       // optional

        [JsonPropertyName("result")]                        // optional<Field type and options | null> Can be null is invalid
        public object? Result { get; set; }                  // optional

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }                   // valid

        [JsonPropertyName("referencedFieldIds")]
        public string[]? ReferencedFieldIds { get; set; }    // optional
    }

    //-----------------------------------------

    /// <summary>
    /// Single line text field
    /// </summary>
    public sealed class SingleLineTextField : ReadWriteFieldConfig   // Read and Write but no options
    {
        public SingleLineTextField()
        {
            Type = FieldType.SingleLineText;
        }
    }

    //-------------------------------------

    /// <summary>
    /// Single select field
    /// </summary>
    public class SingleSelectField : ReadWriteFieldConfig<ChoiceOptions>
    { 
        public SingleSelectField()
        {
            Type = FieldType.SingleSelect;
        }
    }

    //---------------------------------------
    public class SyncSourceField: ReadWriteFieldConfig<ChoiceOptions>       // Emmett: Read only???
    {
        public SyncSourceField()
        {
            Type = FieldType.ExternalSyncSource;
        }
    }

    //------------------------------------

    /// <summary>
    /// URL field
    /// </summary>
    public class UrlField : ReadWriteFieldConfig
    {
        public UrlField()
        {
            Type = FieldType.Url;
        }
    }

    public class UnknownField_Read : FieldConfig, IReadField
    {
        public UnknownField_Read()
        {
            Type = FieldType.UnknownField;
        }

        // The discriminator we saw (could be null/missing)
        public string? UnknownType { get; set; }

        // Raw JSON of the entire field object so we can re-emit it exactly
        public string FieldConfigRawJson { get; set; } = "";

        // Optional convenience: parsed "options" if present
        public string? OptionsRawJson { get; set; }

    }
}