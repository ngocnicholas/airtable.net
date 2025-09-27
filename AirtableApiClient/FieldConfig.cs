using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public class FieldConfig
    {
        [JsonPropertyName("id")]
        [JsonInclude]
        public string? Id { get; set; }

        [JsonPropertyName("description")]
        [JsonInclude]
        public string? Description { get; set; }     // optional

        // from original FieldConfig
        [JsonPropertyName("type")]
        [JsonConverter(typeof(FieldTypeConverter))]

        [JsonInclude]
        public FieldType Type { get; set; }       // optional<Field type> https://airtable.com/developers/web/api/model/field-type
                                               // Should this be an eum or a string?
        [JsonPropertyName("name")]
        [JsonInclude]
        public string? Name { get; set; }

        [JsonPropertyName("options")]
        [JsonInclude]
        public object? Options { get; set; }    // Don't make this property virtual because System.Text.Json does not handle serialization well for polymorphism
                                                // Options will have the type of TOptions passed in the ctor of FieldOptions<TOpetions> : FieldConfig

    }

    // This class is needed for making FieldConfig json friendly
    public abstract class FieldOptions<TOptions> : FieldConfig
    {
        [JsonIgnore] // Always ignore during serialization
        public TOptions TypedOptions
        {
            get
            {
                // With eager deserialization, this should always be the correct type
                return (TOptions)Options!;
            }
            set => Options = value!;
        }
    }

    // https://airtable.com/developers/web/api/field-model
    public class AiTextField : FieldOptions<AiTextOptions>      // READ only
    {
        public AiTextField()
        {
            Type = FieldType.AiText;
        }
    }

    public class AiTextOptions
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

    public class AttachmentField : FieldOptions<AttachementFieldReadOptions>
    {
        public AttachmentField()
        {
            Type = FieldType.MultipleAttachments;
        }
    }

    public class AttachementFieldReadOptions
    {
        [JsonPropertyName("isReversed")]
        public bool IsReversed { get; set; }
    }

    /// <summary>
    /// Auto number field
    /// </summary>
    public class AutoNumberField : FieldConfig 
    {
        public AutoNumberField()
        {
            Type = FieldType.AutoNumber;
        }
    }


    /// <summary>
    /// Barcode field
    /// </summary>
    public class BarcodeField : FieldConfig
    {
        public BarcodeField()
        {
            Type = FieldType.Barcode;
        }
    }


    /// <summary>
    /// Button field
    /// </summary>
    public class ButtonField : FieldConfig
    {
        public ButtonField()
        {
            Type = FieldType.Button;
        }
    }

    /// <summary>
    /// Checkbox field
    /// </summary>
    public class CheckboxField : FieldOptions<CheckboxOptions>
    {
        public CheckboxField()
        {
            Type = FieldType.Checkbox;
        }
    }

    public class CheckboxOptions  
    {
        [JsonPropertyName("color")]
        [JsonConverter(typeof(CheckboxColorConverter))]
        public CheckboxColor Color { get; set; }

        [JsonPropertyName("icon")]
        [JsonConverter(typeof(CheckboxIconConverter))]
        public CheckboxIcon Icon { get; set; }    // "check" | "xCheckbox" | "star" | "heart" | "thumbsUp" | "flag" | "dot"
    }

    /// <summary>
    /// Collaborator field
    /// </summary>
    public class CollaboratorField : FieldConfig  
    {
        public CollaboratorField()
        {
            Type = FieldType.SingleCollaborator;
        }
    }

    /// <summary>
    /// Count field
    /// </summary>
    public class CountField : FieldOptions<CountOptions>
    {
        public CountField()
        {
            Type = FieldType.Count;
        }
    }

    public class CountOptions
    {
        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }   // false when recordLinkFieldId is null, e.g. the referenced column was deleted.

        [JsonPropertyName("recordLinkFieldId")]
        public string? RecordLinkFieldId { get; set; }   // optional<string | null >
    }

    /// <summary>
    /// Created By field
    /// </summary>
    public class CreateByField : FieldConfig
    {
        public CreateByField()
        {
            Type = FieldType.CreatedBy;
        }
    }

    /// <summary>
    /// Created time field
    /// </summary>
    public class CreatedTimeField : FieldOptions<CreatedTimeOptions>
    {
        public CreatedTimeField()
        {
            Type = FieldType.CreatedTime;
        }
    }

    public class CreatedTimeOptions
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
    public class CurrencyField : FieldOptions<CurrencyOptions>  
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
    public class DateField : FieldOptions<DateOptions> 
    {
        public DateField()
        {
            Type = FieldType.Date;
        }
    }

    public class DateOptions 
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
        [JsonConverter(typeof(DateFormatTypeConverter))]
        public DateFormatType Format { get; set; }  // Format is optional when writing, but it must match the corresponding name if provided.

        [JsonPropertyName("name")]
        [JsonConverter(typeof(DateFormatNameConverter))]
        public DateFormatName Name { get; set; }    // "local" | "friend" | "us" | "european" | "iso" 
    }


    /// <summary>
    /// Date and time field
    /// </summary>
    public class DateTimeField : FieldOptions<DateTimeOptions>          // for Read and for Write 
    {
        public DateTimeField()
        {
            Type = FieldType.DateTime;
        }
    }

    public class DateTimeOptions
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
        [JsonConverter(typeof(TimeFormatTypeConverter))]
        public TimeFormatType Format { get; set; }                  // 	Optional for Write. "h:mma" | "HH:mm"

        [JsonPropertyName("name")]
        [JsonConverter(typeof(TimeFormatNameConverter))]
        public TimeFormatName Name { get; set; }                    // "12hour" | "24hour"
    }


    /// <summary>
    /// Duration field
    /// </summary>
    public class DurationField : FieldOptions<DurationOptions>  
    {
        public DurationField()
        {
            Type = FieldType.Duration;
        }
    }

    public class DurationOptions  
    {
        [JsonPropertyName("durationFormat")]
        [JsonConverter(typeof(DurationFormatTypeConverter))]
        public DurationFormatType DurationFormat { get; set; } //"h:mm" | "h:mm:ss" | "h:mm:ss.S" | "h:mm:ss.SS" | "h:mm:ss.SSS"
    }

    /// <summary>
    /// Email field
    /// </summary>
    public class EmailField : FieldConfig 
    {
        public EmailField()
        {
            Type = FieldType.Email;
        }
    }

    /// <summary>
    /// Formula field
    /// </summary>
    public class FormulaField : FieldOptions<FormulaOptions>
    {
        public FormulaField()
        {
            Type = FieldType.Formula;
        }
    }

    public class FormulaOptions
    {
        //The formula including fields referenced by their IDs.For example, LEFT(4, { Birthday}) in the Airtable.com formula editor
        //will be returned as LEFT(4, { fldXXX}) via API.
        [JsonPropertyName("formula")]
        public string? Formula { get; set; }

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("referencedFieldIds")]
        public string[]? ReferencedFieldIds { get; set; }        // array of strings | null
                                                                // All fields in the record that are used in the formula.

        [JsonPropertyName("result")]
        // public object Result { get; set; }                      // Field type and options | null if invalid. See https://airtable.com/developers/web/api/field-model
        public FieldConfig? Result { get; set; }                 // Question: should the type be FieldDefinition or object???
    }

    /// <summary>
    /// Represents information about who last modified the record.
    /// </summary>
    public class LastModifiedByField : FieldConfig
    {
        public LastModifiedByField()
        {
            Type = FieldType.LastModifiedBy;
        }
    }


    /// <summary>
    /// Last modified time field
    /// </summary>
    public class LastModifiedTimeField : FieldOptions<LastModifiedTimeOptions>
    {
        public LastModifiedTimeField()
        {
            Type = FieldType.LastModifiedTime;
        }
    }

    public class LastModifiedTimeOptions
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
    public class LinkToAnotherRecordField : FieldOptions<LinkToAnotherRecordFieldOptions>
    {
        // Creating "multipleRecordLinks" fields is supported but updating options for existing "multipleRecordLinks" fields is not supported.
        public LinkToAnotherRecordField()
        {
            Type = FieldType.MultipleRecordLinks;
        }
    }


    public class LinkToAnotherRecordFieldOptions
    {
        // Note: All properties are in the READ situations
        [JsonPropertyName("isReversed")]
        public bool IsReversed { get; set; }

        [JsonPropertyName("prefersSingleRecordLink")]
        public bool PrefersSingleRecordLink { get; set; }

        [JsonPropertyName("inverseLinkFieldId")]
        public string? InverseLinkFieldId { get; set; } // Optional. The ID of the field in the linked table that links back to this one.

        // Note: Only the 2 properties below are in the CreateBase situation, not even in the UpdateBase situation)
        [JsonPropertyName("linkedTableId")]
        public string? LinkedTableId { get; set; }   // The ID of the table this field links to

        [JsonPropertyName("viewIdForRecordSelection")]
        public string? ViewIdForRecordSelection { get; set; } // Optional. The ID of the view in the linked table to use when showing a list of records to select from.    }

    }


    /// <summary>
    /// Long text field (multi-line)
    /// </summary>    
    public class LongTextField : FieldConfig
    {
        public LongTextField()
        {
            Type = FieldType.MultilineText;
        }
    }


    /// <summary>
    /// Lookup field
    /// </summary>
    public class LookupField : FieldOptions<LookupOptions>
    {
        public LookupField()
        {
            Type = FieldType.MultipleLookupValues;
        }
    }

    public class LookupOptions
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

    //-------------------------------------------------------------------------

    public class MultipleCollaboratorField : FieldOptions<object> 
    {
        public MultipleCollaboratorField()
        {
            Type = FieldType.MultipleCollaborators;
        }
    }


    //---------------------------------------------------------

    /// <summary>
    /// Multi-select field
    /// </summary>
    public class MultipleSelectField : FieldOptions<ChoiceOptions>
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
        [JsonConverter(typeof(ChoiceColorConverter))]
        public ChoiceColor? Color { get; set; }     // optional during Read when the select field is configured to not use colors.           
                                                    // optional duiring Write - This is not specified when creating new options,
                                                    // useful when specifing existing options (for example: reordering options,
                                                    // keeping old options and adding new ones, etc)
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

//--------------------------------------------

    /// <summary>
    /// Number field
    /// </summary>
    public class NumberField : FieldOptions<PrecisionOptions> 
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
    public class PercentField : FieldOptions<PrecisionOptions> 
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
    public class PhoneField : FieldConfig 
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
    public class RatingField : FieldOptions<RatingOptions> 
    {
        public RatingField()
        {
            Type = FieldType.Rating;
        }

    }

    public class RatingOptions 
    {
        [JsonPropertyName("color")]
        [JsonConverter(typeof(RatingColorConverter))]
        public RatingColor Color { get; set; }       // TODO: make an EMUM for this

        [JsonPropertyName("icon")]
        [JsonConverter(typeof(RatingIconConverter))]
        public RatingIcon Icon { get; set; }

        [JsonPropertyName("max")]
        public int Max { get; set; }            // The maximum value for the rating, from 1 to 10 inclusive.
    }

//--------------------------------------------

    /// <summary>
    /// Rich text field with formatting
    /// </summary>
    public class RichTextField : FieldConfig 
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
    public class RollupField : FieldOptions<RollupOptions>           // NEED EMMETT"S HELP in TESTING this field
    {
        public RollupField()
        {
            Type = FieldType.Rollup;
        }
    }

    public class RollupOptions
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
    public class SingleLineTextField : FieldConfig 
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
    public class SingleSelectField : FieldOptions<ChoiceOptions>
    { 
        public SingleSelectField()
        {
            Type = FieldType.SingleSelect;
        }
    }

    //---------------------------------------
    public class SyncSourceField: FieldOptions<ChoiceOptions>
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
    public class UrlField : FieldConfig 
    {
        public UrlField()
        {
            Type = FieldType.Url;
        }
    }
}