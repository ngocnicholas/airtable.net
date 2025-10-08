using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    // public interface served as markers since C# does not allow multiple inheritance of classes
    // but a class can inherite from multiple interfaces.

    public interface IFieldModel { }    // for marking the READ fields, appearing in server responses
    public interface IFieldConfig { }   // for marking the WRITE fields, to be fuse in CreateBase

    //------------------------------------------------------------------

    //public class FieldType : IField
    public class FieldType
    {
        [JsonPropertyName("id")][JsonInclude] 
        public string? Id { get; set; }

        [JsonPropertyName("description")][JsonInclude] 
        public string? Description { get; set; }

        // Option 1: enum (preferred)
        [JsonPropertyName("type")]
        [JsonConverter(typeof(FieldTypeEnumConverter))]
        [JsonInclude]
        public FieldTypeEnum Type { get; set; }

        [JsonPropertyName("name")][JsonInclude] 
        public string? Name { get; set; }
    }

    //---------------------------------------------------------------------------

    public abstract class FieldConfig : FieldType, IFieldConfig { }

    // Generic base for write-options fields
    public abstract class FieldConfig<TWriteOptions> : FieldConfig
    {
        [JsonPropertyName("options")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TWriteOptions? WriteOptions { get; set; }
    }

    //-------------------------------------------------------------------------
    public abstract class FieldModel : FieldType, IFieldModel { }

    // Generic base for read-options fields
    public abstract class FieldModel<TReadOptions> : FieldModel
    {
        [JsonPropertyName("options")]
        public TReadOptions? ReadOptions { get; set; }
    }

    //------------------------------------------------------------------------------

    // read–write, no options (inherits the WRITE base! and the READ interface)
    // NOTE: FieldModelConfig must derive from class FieldModel (not classFieldConfig)
    // so that the custom FieldModelJsonConverter for FieldModle will work for FieldModelConfig also.
    public abstract class FieldModelConfig : FieldModel, IFieldConfig { }

    // If a field is RW with the **same** options type in both directions:
    public abstract class FieldModelConfig<TOptions> : FieldModelConfig
    {
        [JsonPropertyName("options")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TOptions? Options { get; set; }
    }

    //-------------------------------------------------------------------------

    public sealed class AiTextFieldModel : FieldModel<AiTextReadOptions>       // R only, with options
    { 
        public AiTextFieldModel() { Type = FieldTypeEnum.AiText; } 
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
    public sealed class AttachmentFieldModel : FieldModel<AttachmentFieldReadOptions>
    {
        public AttachmentFieldModel() => Type = FieldTypeEnum.MultipleAttachments;
    }

    public sealed class AttachmentFieldReadOptions
    {
        [JsonPropertyName("isReversed")]
        public bool IsReversed { get; set; }
    }

    // WRITE shape (no options allowed in payload)
    public sealed class AttachmentFieldConfig : FieldConfig
    {
        public AttachmentFieldConfig() => Type = FieldTypeEnum.MultipleAttachments;
    }

    /// <summary>
    /// Auto number field
    /// </summary>
    public class AutoNumberFieldModel : FieldModel
    {
        public AutoNumberFieldModel()
        {
            Type = FieldTypeEnum.AutoNumber;
        }
    }


    /// <summary>
    /// Barcode field
    /// </summary>
    public class BarcodeField : FieldModelConfig
    {
        public BarcodeField()
        {
            Type = FieldTypeEnum.Barcode;
        }
    }


    /// <summary>
    /// Button field
    /// </summary>
    public class ButtonFieldModel : FieldModel
    {
        public ButtonFieldModel()
        {
            Type = FieldTypeEnum.Button;
        }
    }

    public class CheckboxField : FieldModelConfig<CheckboxOptions>  // RW, with same options
    {
        public CheckboxField()
        {
            Type = FieldTypeEnum.Checkbox;
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
    public class CollaboratorField : FieldModelConfig
    {
        public CollaboratorField()
        {
            Type = FieldTypeEnum.SingleCollaborator;
        }
    }

    /// <summary>
    /// Count field
    /// </summary>
    public class CountFieldModel : FieldModel<CountFieldModelOptions>
    {
        public CountFieldModel()
        {
            Type = FieldTypeEnum.Count;
        }
    }

    public class CountFieldModelOptions
    {
        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }   // false when recordLinkFieldId is null, e.g. the referenced column was deleted.

        [JsonPropertyName("recordLinkFieldId")]
        public string? RecordLinkFieldId { get; set; }   // optional<string | null >
    }

    /// <summary>
    /// Created By field
    /// </summary>
    public class CreatedByFieldModel : FieldModel
    {
        public CreatedByFieldModel()
        {
            Type = FieldTypeEnum.CreatedBy;
        }
    }

    /// <summary>
    /// Created time field
    /// </summary>
    public class CreatedTimeFieldModel : FieldModel<CreatedTimeFieldModelOptions>
    {
        public CreatedTimeFieldModel()
        {
            Type = FieldTypeEnum.CreatedTime;
        }
    }

    public class CreatedTimeFieldModelOptions
    {
        [JsonPropertyName("result")]
        public FieldModel? Result { get; set; } //optional but always be either DateField or DateTimeField

    }

    /// <summary>
    /// Currency field
    /// </summary>
    public class CurrencyField : FieldModelConfig<CurrencyOptions>
    {
        public CurrencyField()
        {
            Type = FieldTypeEnum.Currency;
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
    public class DateField : FieldModelConfig<DateFieldOptions>  // RW, with Options but Write's format is optional
    {
        public DateField()
        {
            Type = FieldTypeEnum.Date;
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
    public class DateTimeField : FieldModelConfig<DateTimeFieldOptions>          // RW, with Options but Write's format is optional
    {
        public DateTimeField()
        {
            Type = FieldTypeEnum.DateTime;
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
    public class DurationField : FieldModelConfig<DurationOptions>
    {
        public DurationField()
        {
            Type = FieldTypeEnum.Duration;
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
    public class EmailField : FieldModelConfig
    {
        public EmailField()
        {
            Type = FieldTypeEnum.Email;
        }
    }

    /// <summary>
    /// Formula field
    /// </summary>
    public class FormulaFieldModel : FieldModel<FormulaFieldModelOptions>
    {
        public FormulaFieldModel()
        {
            Type = FieldTypeEnum.Formula;
        }
    }

    public class FormulaFieldModelOptions
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
        public FieldModel? Result { get; set; }
    }

    /// <summary>
    /// Represents information about who last modified the record.
    /// </summary>
    public class LastModifiedByFieldModel : FieldModel  // Read only, no options
    {
        public LastModifiedByFieldModel()
        {
            Type = FieldTypeEnum.LastModifiedBy;
        }
    }

    //-----------------------------------------------------------------------------
    /// <summary>
    /// Last modified time field
    /// </summary>
    public class LastModifiedTimeFieldModel : FieldModel<LastModifiedTimeFieldModelOptions>
    {
        public LastModifiedTimeFieldModel()
        {
            Type = FieldTypeEnum.LastModifiedTime;
        }
    }

    public class LastModifiedTimeFieldModelOptions
    {
        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("referencedFieldIds")]
        public string[]? ReferencedFieldIds { get; set; }       // array of strings | null
                                                                // All fields in the record that are used in the formula.
        [JsonPropertyName("result")]
        public FieldModel? Result { get; set; }      // null | Always be a DateField or DateFielField
    }

    //---------------------------------------------------------------------------

    /// <summary>
    /// Link to another record field
    /// </summary>
    /// <summary>
    /// Link to another record field
    /// </summary>
    public class LinkToAnotherRecordFieldModel : FieldModel<LinkToAnotherRecordFieldModelOptions>
    {
        // Creating "multipleRecordLinks" fields is supported but updating options for existing "multipleRecordLinks" fields is not supported.
        public LinkToAnotherRecordFieldModel()
        {
            Type = FieldTypeEnum.MultipleRecordLinks;
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
        public string? InverseLinkFieldId { get; set; } // Optional. The ID of the field in the linked table that links back to this one.

        [JsonPropertyName("linkedTableId")]
        public string? LinkedTableId { get; set; }   // The ID of the table this field links to

        [JsonPropertyName("viewIdForRecordSelection")]
        public string? ViewIdForRecordSelection { get; set; } // Optional. The ID of the view in the linked table to use when showing a list of records to select from.    }

    }

    //......................................................

    public class LinkToAnotherRecordFieldConfig : FieldConfig<LinkToAnotherRecordFieldConfigOptions>
    {
        // Creating "multipleRecordLinks" fields is supported but updating options for existing "multipleRecordLinks" fields is not supported.
        public LinkToAnotherRecordFieldConfig()
        {
            Type = FieldTypeEnum.MultipleRecordLinks;
        }
    }

    public class LinkToAnotherRecordFieldConfigOptions
    {
        // Note: All properties are in the Write situations
        // Note: Only the 2 properties below are in the CreateBase situation, not even in the UpdateBase situation)
        [JsonPropertyName("linkedTableId")]
        public string? LinkedTableId { get; set; }   // The ID of the table this field links to

        [JsonPropertyName("viewIdForRecordSelection")]
        public string? ViewIdForRecordSelection { get; set; } // Optional. The ID of the view in the linked table to use when showing a list of records to select from.    }

    }
    //--------------------------------------------------------------------------------------------

    /// <summary>
    /// Long text field (multi-line)
    /// </summary>    
    public class LongTextField : FieldModelConfig
    {
        public LongTextField()
        {
            Type = FieldTypeEnum.MultilineText;
        }
    }

    //--------------------------------------------------------------------------------------
    /// <summary>
    /// Lookup field
    /// </summary>
    public class LookupFieldModel : FieldModel<LookupFieldModelOptions>
    {
        public LookupFieldModel()
        {
            Type = FieldTypeEnum.MultipleLookupValues;
        }
    }

    public class LookupFieldModelOptions
    {
        [JsonPropertyName("fieldIdInLinkedTable")]
        public string? FieldIdInLinkedTable { get; set; }    // may be null

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("recordLinkFieldId")]
        public string? RecordLinkFieldId { get; set; }       // may be null

        [JsonPropertyName("result")]
        public FieldModel? Result { get; set; } 

    }

    //--------------------------------------------------------------------------------

    public class MultipleCollaboratorField : FieldModelConfig<object> 
    {
        public MultipleCollaboratorField()
        {
            Type = FieldTypeEnum.MultipleCollaborators;
        }
    }

    /// <summary>
    /// Multi-select field
    /// </summary>
    public class MultipleSelectField : FieldModelConfig<ChoiceOptions>  // RW with options but the Write's option is more flexible
    {
        public MultipleSelectField()
        {
            Type = FieldTypeEnum.MultipleSelects;
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

    //----------------------------------------------------------------------

    /// <summary>
    /// Number field
    /// </summary>
    public class NumberField : FieldModelConfig<PrecisionOptions> 
    {
        public NumberField()
        {
            Type = FieldTypeEnum.Number;
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
    public class PercentField : FieldModelConfig<PrecisionOptions> 
    {
        public PercentField()
        {
            Type = FieldTypeEnum.Percent;
        }
    }
    //---------------------------------------------

    /// <summary>
    /// Phone number field
    /// </summary>
    public class PhoneField : FieldModelConfig 
    {
        public PhoneField()
        {
            Type = FieldTypeEnum.PhoneNumber;
        }
    }

    //-----------------------------

    /// <summary>
    /// Rating field
    /// </summary>
    public class RatingField : FieldModelConfig<RatingOptions> 
    {
        public RatingField()
        {
            Type = FieldTypeEnum.Rating;
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
    public class RichTextField : FieldModelConfig
    {
        public RichTextField()
        {
            Type = FieldTypeEnum.RichText;
        }
    }

    //-----------------------------------------------

    /// <summary>
    /// Rollup field
    /// </summary>
    public class RollupFieldModel : FieldModel<RollupFieldModelOptions>           // NEED EMMETT"S HELP in TESTING this field
    {
        public RollupFieldModel()
        {
            Type = FieldTypeEnum.Rollup;
        }
    }

    public class RollupFieldModelOptions
    {
        [JsonPropertyName("fieldIdInLinkedTable")]
        public string? FieldIdInLinkedTable { get; set; }    // optional

        [JsonPropertyName("recordLinkFieldId")]
        public string? RecordLinkFieldId { get; set; }       // optional

        [JsonPropertyName("result")]                        // optional<FieldModel | null> Can be null is invalid
        public FieldModel? Result { get; set; }            

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }                   // valid

        [JsonPropertyName("referencedFieldIds")]
        public string[]? ReferencedFieldIds { get; set; }    // optional
    }

    //-----------------------------------------

    /// <summary>
    /// Single line text field
    /// </summary>
    public sealed class SingleLineTextField : FieldModelConfig   // Read and Write but no options
    {
        public SingleLineTextField()
        {
            Type = FieldTypeEnum.SingleLineText;
        }
    }

    //-------------------------------------

    /// <summary>
    /// Single select field
    /// </summary>
    public class SingleSelectField : FieldModelConfig<ChoiceOptions>
    { 
        public SingleSelectField()
        {
            Type = FieldTypeEnum.SingleSelect;
        }
    }

    //---------------------------------------
    public class SyncSourceField: FieldModelConfig<ChoiceOptions>       // Should be Read only according to feedback from Airtable
    {
        public SyncSourceField()
        {
            Type = FieldTypeEnum.ExternalSyncSource;
        }
    }

    //------------------------------------

    /// <summary>
    /// URL field
    /// </summary>
    public class UrlField : FieldModelConfig
    {
        public UrlField()
        {
            Type = FieldTypeEnum.Url;
        }
    }

    //---------------------------------------------

    public class UnknownFieldModel : FieldModel
    {
        public UnknownFieldModel()
        {
            Type = FieldTypeEnum.UnknownField;
        }

        // The discriminator we saw (could be null/missing)
        public string? UnknownType { get; set; }

        // Raw JSON of the entire field object so we can re-emit it exactly
        public string FieldConfigRawJson { get; set; } = "";

        // Optional convenience: parsed "options" if present
        public string? OptionsRawJson { get; set; }

    }

    // Pick one of the below.  We don't need both
#if true    
    ///
    /// Extension methods (ergonomic, discoverable)
    /// Usage:
    /// var opts = tables[0].Fields[18].RequireOptions<LookupFieldModelOptions>();
    /// or 
    /// if (tables[0].Fields[18].TryGetOptions<LookupFieldModelOptions>(out var o)) { use o }
    /// 

    public static class FieldModelExtensions    // Extension methods (ergonomic, discoverable)
    {
        public static TOpts RequireOptions<TOpts>(this FieldModel f) where TOpts : class
        {
            if (f is FieldModel<TOpts> m && m.ReadOptions is { } o) return o;
            throw new InvalidOperationException($"Not a {typeof(TOpts).Name} field or options missing.");
        }

        public static bool TryGetOptions<TOpts>(this FieldModel f, out TOpts? options) where TOpts : class
        {
            if (f is FieldModel<TOpts> m && m.ReadOptions is { } o) { options = o; return true; }
            options = null; return false;
        }
    }
#else   
    ///
    /// Static helper on FieldModel (simple)
    /// Usage:
    /// var opts = FieldModel.RequireOptions<LookupFieldModelOptions>(tables[0].Fields[18]);
    /// 

    public abstract class FieldModel : FieldBase, IFieldModel
    {
        public static TOpts RequireOptions<TOpts>(FieldModel f) where TOpts : class
        {
            if (f is FieldModel<TOpts> m && m.ReadOptions is { } o) return o;
            throw new InvalidOperationException($"Not a {typeof(TOpts).Name} field or options missing.");
        }
    }
#endif
}